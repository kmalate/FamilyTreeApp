using FamilyTreeApp.Server.Infrastructure.Interfaces;
using FamilyTreeApp.Server.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyTreeApp.Server.Infrastructure.Repositories
{
    public class FamilyTreeRepository : IFamilyTreeRepository
    {
        private readonly AppDbContext _context;
        public FamilyTreeRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Relationship>> GetSinglePersonRelationshipsAsync(int personId)
        {
            var relationships = await _context.Relationship
                .Where(r => r.PersonId1 != null && r.PersonId1 == personId)
                .ToListAsync();
            return relationships;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PersonRelationship>> GetPersonRelationshipListAsync()
        {
            return await _context.PersonRelationships.ToListAsync();
        }

        /// <inheritdoc />
        public Task UpdatePersonAsync(Person person)
        {            
            var selectedPerson = _context.Person.Find(person.PersonId);
            if (selectedPerson != null)
            {
                selectedPerson.FirstName = person.FirstName;
                selectedPerson.LastName = person.LastName;
                selectedPerson.Gender = person.Gender;
                return _context.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task UpdateRelationshipAsync(Relationship relationship)
        {
            var selectedRelationship = _context.Relationship.Find(relationship.RelationshipId);
            if (selectedRelationship != null)
            {
                selectedRelationship.PersonId1 = relationship.PersonId1;
                selectedRelationship.PersonId2 = relationship.PersonId2;
                selectedRelationship.RelationshipType = relationship.RelationshipType;
                return _context.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<Person> AddPersonAsync(Person person)
        {
            _context.Person.Add(person);
            return _context.SaveChangesAsync().ContinueWith(t => person);
        }

        /// <inheritdoc />
        public Task<Relationship> AddRelationShipAsync(Relationship relationship)
        {
            _context.Relationship.Add(relationship);
            return _context.SaveChangesAsync().ContinueWith(t => relationship);
        }

        /// <inheritdoc />
        public Task DeletePersonAsync(Person person)
        {
            _context.Person.Remove(person);
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task DeleteRelationShipAsync(Relationship relationship)
        {
            _context.Relationship.Remove(relationship); 
            return _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Person>> GetAllPersonsByIdListAsync(int[] idList)
        {
            var persons = await _context.Person.Where(p => idList.Contains(p.PersonId)).ToArrayAsync();
            return persons;                
        }

        /// <inheritdoc />
        public async Task<Person?> GetPersonByIdAsync(int personId)
        {
            var person = await _context.Person.FindAsync(personId);
            return person;
        }
    }
}
