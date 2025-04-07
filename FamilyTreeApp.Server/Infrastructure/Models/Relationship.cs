using System.ComponentModel.DataAnnotations;

namespace FamilyTreeApp.Server.Infrastructure.Models
{
    public class Relationship
    {
        [Key]
        public int RelationshipId { get; set; }
        public int? PersonId1 { get; set; } = null;
        public int? PersonId2 { get; set; } = null;
        public string RelationshipType { get; set; } = string.Empty;
    }
}
