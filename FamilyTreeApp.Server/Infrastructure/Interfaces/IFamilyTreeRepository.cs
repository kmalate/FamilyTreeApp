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

        /// <summary>
        /// Updates Person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        Task UpdatePersonAsync(Person person);

        /// <summary>
        /// Add new person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        Task<Person> AddPersonAsync(Person person);

        /// <summary>
        /// Delete person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        Task DeletePersonAsync(Person person);

        /// <summary>
        /// Updates Relationship
        /// </summary>
        /// <param name="relationship"></param>
        /// <returns></returns>        
        Task UpdateRelationshipAsync(Relationship relationship);

        /// <summary>
        /// Add new relationship
        /// </summary>
        /// <param name="relationship"></param>
        /// <returns></returns>
        Task<Relationship> AddRelationShipAsync(Relationship relationship);

        /// <summary>
        /// Delete relationship
        /// </summary>
        /// <param name="relationship"></param>
        /// <returns></returns>
        Task DeleteRelationShipAsync(Relationship relationship);
        /// <summary>
        /// Get all relationships for a person
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<IEnumerable<Relationship>> GetSinglePersonRelationshipsAsync(int personId);

        /// <summary>
        /// Get person by Id List
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        Task<IEnumerable<Person>> GetAllPersonsByIdListAsync(int[] idList);

        /// <summary>
        /// Get single person by id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<Person?> GetPersonByIdAsync(int personId);
    }
}
