using AutoMapper;
using FamilyTreeApp.Server.Core.Interfaces;
using FamilyTreeApp.Server.Core.Models;
using FamilyTreeApp.Server.Core.Services;
using FamilyTreeApp.Server.Infrastructure.Interfaces;
using FamilyTreeApp.Server.Infrastructure.Models;
using Moq;

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
                    Id = "1",
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
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<FamilyNodeDTO>>());
            Assert.That(result, Is.EqualTo(familyNodeDTOs));
        }

        [Test]
        public async Task UpdateFamilyTreeNodesAsync_Should_Update_If_UpdateNodesData_Has_Value()
        {
            // Arrange
            var args = new UpdateNodeArgsDTO {
                UpdateNodesData =
                [
                    new () { Id = "1", Name = "John Doe", Pids = [2], Gender= "male" } 
                ],
                AddNodesData = []              
            };

            _familyTreeRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Person { PersonId = 1 });

            // Act
            var result = await _familyTreeService.UpdateFamilyTreeNodesAsync(args);

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.UpdatePersonAsync(It.IsAny<Person>()), Times.Once);
        }

        [Test]
        public async Task UpdateFamilyTreeNodesAsync_Should_Add_if_AddNodesData_Has_Value_And_Return_OldId_NewId()
        {
            // Arrange
            var args = new UpdateNodeArgsDTO
            {
                UpdateNodesData = [],
                AddNodesData =
                [
                    new () { Id = "oldId1", Name = "John Doe", Pids = [2], Gender= "male" }
                ],
            };

            _familyTreeRepositoryMock.Setup(repo => repo.AddPersonAsync(It.IsAny<Person>()))
                .ReturnsAsync(new Person { PersonId = 1 });

            _familyTreeRepositoryMock.Setup(respo => respo.GetAllPersonsByIdListAsync(It.IsAny<int[]>()))
                .ReturnsAsync(new List<Person> { new Person { PersonId = 2 } });

            // Act
            var result = await _familyTreeService.UpdateFamilyTreeNodesAsync(args);

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.AddPersonAsync(It.IsAny<Person>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Once);

             var expected = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> { { "oldId1", "1" } }
            };

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
