using System;
using System.Collections.Generic;
using api.Entities;
using datingApp.api.Extensions;

namespace datingApp.api.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime UserCreatedOn { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        // one to many relationship: one user have many user photos
        public ICollection<UserPhoto> UserPhotos { get; set; }
        // many to many relationship: many users can like one user, this user can like many other users
        public ICollection<UserLike> LikedUsers { get; set; }
        public ICollection<UserLike> LikedByUsers { get; set; }

        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }
    }
}