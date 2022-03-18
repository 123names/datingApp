using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities
{
    [Table("UserPhotos")]
    public class UserPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public bool IsApproved { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}