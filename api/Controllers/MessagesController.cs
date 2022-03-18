namespace api.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {

        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public MessagesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();
            if (username == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You can't message yourself");

            var sender = await this.unitOfWork.userRepository.GetUserByUsernameAsync(username);
            var recipient = await this.unitOfWork.userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
            if (recipient == null) return NotFound();

            var newMessage = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            this.unitOfWork.messageRepository.AddMessage(newMessage);
            if (await this.unitOfWork.Complete()) return Ok(this.mapper.Map<MessageDto>(newMessage));

            return BadRequest("Failed to send a message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();
            var messages = await this.unitOfWork.messageRepository.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalItemCount, messages.TotalPages);
            return messages;
        }
        /*Old method: message thread with API*/
        // [HttpGet("thread/{recipientUsername}")]
        // public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string recipientUsername)
        // {
        //     var currentUsername = User.GetUserName();
        //     return Ok(await this.unitOfWork.messageRepository.GetMessageThread(currentUsername, recipientUsername));
        // }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();

            var message = await this.unitOfWork.messageRepository.GetMessage(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username) return Unauthorized();

            if (message.Sender.UserName == username) message.SenderDeleted = true;
            if (message.Recipient.UserName == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted) this.unitOfWork.messageRepository.DeleteMessage(message);

            if (await this.unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting message");
        }
    }
}