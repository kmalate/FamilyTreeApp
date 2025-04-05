using AutoMapper;
using FamilyTreeApp.Server.Core.Interfaces;
using FamilyTreeApp.Server.Core.Models;
using FamilyTreeApp.Server.Core.Services;
using FamilyTreeApp.Server.Infrastructure.Interfaces;
using FamilyTreeApp.Server.Infrastructure.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilyTreeApp.Server.UnitTests.Services
{
    [TestFixture]
    public class FamilyTreeServiceTests
    {
        private Mock<IFamilyTreeRepository> _familyTreeRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private IFamilyTreeService _familyTreeService;

        [SetUp]
        public void SetUp()
        {
            _familyTreeRepositoryMock = new Mock<IFamilyTreeRepository>();
            _mapperMock = new Mock<IMapper>();
            _familyTreeService = new FamilyTreeService(_familyTreeRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetFamilyTreeNodesAsync_ReturnsFamilyNodes()
        {
            // Arrange
            var personRelationships = new List<PersonRelationship>
            {
                new PersonRelationship
                {
                    PersonId = 1,
                    SpousePersonIds = "2,3",
                    MotherPersonId = 4,
                    FatherPersonId = 5,
                    FirstName = "John",
                    LastName = "Doe",
                    Gender = "Male"
                }
            };

            var familyNodeDTOs = new List<FamilyNodeDTO>
            {
                new FamilyNodeDTO
                {
                    Id = 1,
                    Pids = new List<int> { 2, 3 },
                    Mid = 4,
                    Fid = 5,
                    Name = "John Doe",
                    Gender = "Male"
                }
            };

            _familyTreeRepositoryMock
                .Setup(repo => repo.GetPersonRelationshipListAsync())
                .ReturnsAsync(personRelationships);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<FamilyNodeDTO>>(personRelationships))
                .Returns(familyNodeDTOs);

            // Act
            var result = await _familyTreeService.GetFamilyTreeNodesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<FamilyNodeDTO>>(result);
            Assert.AreEqual(familyNodeDTOs, result);
        }
    }
}
