using System.ComponentModel.DataAnnotations;

namespace FamilyTreeApp.Server.Infrastructure.Models
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;  
        public byte[]? Img { get; set; } = null;
    }
}
