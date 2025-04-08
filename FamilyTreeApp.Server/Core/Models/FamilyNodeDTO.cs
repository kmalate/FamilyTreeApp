namespace FamilyTreeApp.Server.Core.Models
{
    public class FamilyNodeDTO
    {
        public string Id { get; set; } = string.Empty;
        public IEnumerable<string>? Pids { get; set; } = new List<string>();
        public string Mid { get; set; } = string.Empty;
        public string Fid { get; set; } = string.Empty;
        public required string Name { get; set; }
        public string Gender { get; set; } = string.Empty;
    }
}
