using System.ComponentModel.DataAnnotations;

namespace api.Entities
{
    public class Group
    {
        public Group()
        {
        }
        public Group(string groupName)
        {
            Groupname = groupName;
        }
        [Key]
        public string Groupname { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}