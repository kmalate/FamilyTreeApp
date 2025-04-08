namespace FamilyTreeApp.Server.Core.Models
{
    public class UpdateNodeArgsDTO
    {
        public int? RemoveNodeId { get; set; } = null;
        public FamilyNodeDTO[] UpdateNodesData { get; set; } = [];
        public FamilyNodeDTO[] AddNodesData { get; set; } = [];
    }
}