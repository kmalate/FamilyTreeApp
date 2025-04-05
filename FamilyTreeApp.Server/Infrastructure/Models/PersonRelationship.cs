namespace FamilyTreeApp.Server.Infrastructure.Models
{
    public class PersonRelationship
    {
        public int PersonId { get; set; }        
        /// Comma delimited list of IDs ex: 1,2,3
        public string? SpousePersonIds { get; set; }
        public int? MotherPersonId { get; set; }
        public int? FatherPersonId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
    }
}
