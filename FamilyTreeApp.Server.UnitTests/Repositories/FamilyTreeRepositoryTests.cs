using FamilyTreeApp.Server.Infrastructure;
using FamilyTreeApp.Server.Infrastructure.Models;
using FamilyTreeApp.Server.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FamilyTreeApp.Server.UnitTests
{
    public class FamilyTreeRepositoryTests
    {
        private FamilyTreeRepository _repository;
        private AppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);
            _repository = new FamilyTreeRepository(_context);

            // Seed the in-memory database with test data
            _context.PersonRelationships.AddRange(
                new PersonRelationship { PersonId = 1, FirstName = "John", LastName = "Doe" },
                new PersonRelationship { PersonId = 2, FirstName = "Jane", LastName = "Doe" }
            );

            _context.Person.Add(new Person { PersonId = 1, FirstName = "John", LastName = "Smith", Gender = "Male" });
            _context.Relationship.Add(new Relationship { RelationshipId = 1, PersonId1 = 1, PersonId2 = 2, RelationshipType = "Spouse" });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }        

        [Test]
        public async Task GetPersonRelationshipListAsync_ReturnsAllPersonRelationships()
        {
            // Act
            var result = await _repository.GetPersonRelationshipListAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetSinglePersonRelationshipsAsync_ReturnsRelationshipsForPerson()
        {
            // Act
            var result = await _repository.GetSinglePersonRelationshipsAsync(1);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().RelationshipType, Is.EqualTo("Spouse"));
        }

        [Test]
        public async Task UpdatePersonAsync_UpdatesPersonDetails()
        {
            // Arrange
            var person = new Person { PersonId = 1, FirstName = "John", LastName = "Smith", Gender = "Male" };

            // Act
            await _repository.UpdatePersonAsync(person);
            var updatedPerson = await _context.Person.FindAsync(1);

            // Assert
            Assert.That(updatedPerson.LastName, Is.EqualTo("Smith"));
        }

        [Test]
        public async Task UpdateRelationshipAsync_UpdatesRelationshipDetails()
        {
            // Arrange
            var relationship = new Relationship { RelationshipId = 1, PersonId1 = 1, PersonId2 = 2, RelationshipType = "Parent" };

            // Act
            await _repository.UpdateRelationshipAsync(relationship);
            var updatedRelationship = await _context.Relationship.FindAsync(1);

            // Assert
            Assert.That(updatedRelationship.RelationshipType, Is.EqualTo("Parent"));
        }

        [Test]
        public async Task AddPersonAsync_AddsNewPerson()
        {
            // Arrange
            var person = new Person { PersonId = 3, FirstName = "Alice", LastName = "Johnson", Gender = "Female" };

            // Act
            var result = await _repository.AddPersonAsync(person);
            var addedPerson = await _context.Person.FindAsync(3);

            // Assert
            Assert.That(addedPerson, Is.Not.Null);
            Assert.That(addedPerson.FirstName, Is.EqualTo("Alice"));
        }

        [Test]
        public async Task AddRelationShipAsync_AddsNewRelationship()
        {
            // Arrange
            var relationship = new Relationship { RelationshipId = 2, PersonId1 = 1, PersonId2 = 2, RelationshipType = "Sibling" };

            // Act
            var result = await _repository.AddRelationShipAsync(relationship);
            var addedRelationship = await _context.Relationship.FindAsync(2);

            // Assert
            Assert.That(addedRelationship, Is.Not.Null);
            Assert.That(addedRelationship.RelationshipType, Is.EqualTo("Sibling"));
        }

        [Test]
        public async Task DeletePersonAsync_DeletesPerson()
        {
            // Arrange
            var person = await _context.Person.FindAsync(1);

            // Act
            await _repository.DeletePersonAsync(person);
            var deletedPerson = await _context.Person.FindAsync(1);

            // Assert
            Assert.That(deletedPerson, Is.Null);
        }

        [Test]
        public async Task DeleteRelationShipAsync_DeletesRelationship()
        {
            // Arrange
            var relationship = await _context.Relationship.FindAsync(1);

            // Act
            await _repository.DeleteRelationShipAsync(relationship);
            var deletedRelationship = await _context.Relationship.FindAsync(1);

            // Assert
            Assert.That(deletedRelationship, Is.Null);
        }

        [Test]
        public async Task GetAllPersonsByIdListAsync_ReturnsCorrectPersons()
        {
            // Arrange
            var idList = new int[] { 1 };

            // Act
            var result = await _repository.GetAllPersonsByIdListAsync(idList);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.PersonId == 1), Is.True);
        }
    }
}
