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

        /// <summary>
        /// Updates the Family Database Objects
        /// </summary>
        /// <param name="updateNodeArgs"></param>
        /// <returns></returns>
        Task<IEnumerable<Dictionary<string, string>>> UpdateFamilyTreeNodesAsync(UpdateNodeArgsDTO updateNodeArgs);
    }
}
