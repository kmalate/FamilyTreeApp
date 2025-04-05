using FamilyTreeApp.Server.Infrastructure.Models;

namespace FamilyTreeApp.Server.Infrastructure.Interfaces
{
    public interface IFamilyTreeRepository
    {
        /// <summary>
        /// Get a list of all persons with their relationships.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<PersonRelationship>> GetPersonRelationshipListAsync();
    }
}
