using FamilyTreeApp.Server.Infrastructure;
using FamilyTreeApp.Server.Infrastructure.Models;
using FamilyTreeApp.Server.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
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
    }
}
