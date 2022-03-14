using System;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs;
using api.Entities;
using api.Extensions;
using api.Interfaces;
using AutoMapper;
using datingApp.api.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace api.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMapper mapper;
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;
        private readonly IHubContext<PresenceHub> presenceHub;
        private readonly PresenceTracker tracker;

        public MessageHub(IMessageRepository messageRepository, IMapper mapper, IUserRepository userRepository, IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            this.presenceHub = presenceHub;
            this.tracker = tracker;
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await this.messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup(Context.ConnectionId);
            await Clients.Group(group.Groupname).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUserName();
            if (username == createMessageDto.RecipientUsername.ToLower()) throw new HubException("You can't message yourself");

            var sender = await this.userRepository.GetUserByUsernameAsync(username);
            var recipient = await this.userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
            if (recipient == null) throw new HubException("User not found"); ;

            var newMessage = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await this.messageRepository.GetMessageGroup(groupName);
            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                newMessage.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await this.tracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null)
                {
                    await this.presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }
            this.messageRepository.AddMessage(newMessage);
            if (await this.messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", this.mapper.Map<MessageDto>(newMessage));
            };
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await this.messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

            if (group == null)
            {
                group = new Group(groupName);
                this.messageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);

            if (await this.messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup(string connectionId)
        {
            var group = await this.messageRepository.GetGroupForConnection(connectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            this.messageRepository.RemoveConnection(connection);
            if (await this.messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to remove from group");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}