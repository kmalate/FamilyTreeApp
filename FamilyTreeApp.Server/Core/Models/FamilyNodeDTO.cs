namespace FamilyTreeApp.Server.Core.Models
{
    public class FamilyNodeDTO
    {
        public int Id { get; set; }
        public IEnumerable<int>? Pids { get; set; } = new List<int>();
        public int? Mid { get; set; }
        public int? Fid { get; set; }
        public required string Name { get; set; }
        public string Gender { get; set; } = string.Empty;
    }
}
