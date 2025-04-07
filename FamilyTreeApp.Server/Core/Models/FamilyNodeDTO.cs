namespace FamilyTreeApp.Server.Core.Models
{
    public class FamilyNodeDTO
    {
        public string Id { get; set; } = string.Empty;
        public IEnumerable<int>? Pids { get; set; } = new List<int>();
        public int? Mid { get; set; }
        public int? Fid { get; set; }
        public required string Name { get; set; }
        public string Gender { get; set; } = string.Empty;
    }
}
