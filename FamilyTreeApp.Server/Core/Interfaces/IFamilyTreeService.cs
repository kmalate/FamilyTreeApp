using FamilyTreeApp.Server.Core.Models;

namespace FamilyTreeApp.Server.Core.Interfaces
{
    public interface IFamilyTreeService
    {
        /// <summary>
        /// Get a list of all Family Nodes.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<FamilyNodeDTO>> GetFamilyTreeNodesAsync();
    }
}
