using System;
using System.Collections.Generic;
using api.Entities;
using Microsoft.AspNetCore.Identity;

namespace datingApp.api.Entities
{
    public class AppUser : IdentityUser<int>
    {
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
        // many to many relationship with join table AppUserRole
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}