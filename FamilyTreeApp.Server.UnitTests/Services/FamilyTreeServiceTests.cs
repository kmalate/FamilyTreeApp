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
                    Pids = new List<string> { "2", "3" },
                    Mid = "4",
                    Fid = "5",
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
            var args = new UpdateNodeArgsDTO
            {
                UpdateNodesData =
                [
                    new () { Id = "1", Name = "John Doe", Pids = ["2"], Gender= "male" }
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
                    new () { Id = "oldId1", Name = "John Doe", Pids = ["2"], Gender= "male" }
                ],
            };

            _familyTreeRepositoryMock.Setup(repo => repo.AddPersonAsync(It.IsAny<Person>()))
                .ReturnsAsync(new Person { PersonId = 1 });

            // Act
            var result = await _familyTreeService.UpdateFamilyTreeNodesAsync(args);

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.AddPersonAsync(It.IsAny<Person>()), Times.Once);

            var expected = new Dictionary<string, string>
            {
                { "oldId1", "1" }
            };

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task UpdatePersonRelationships_Remove_Person_If_RemoveNodeId_Is_Set()
        {
            // Arrange
            var args = new UpdateNodeArgsDTO
            {
                RemoveNodeId = 1,
                UpdateNodesData = [],
                AddNodesData = []
            };

            _familyTreeRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.IsAny<int>()))
               .ReturnsAsync(new Person { PersonId = 1 });
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Relationship[] {
                    new Relationship { PersonId1 = 1, PersonId2 = 2, RelationshipType = "spouse" },
                    new Relationship { PersonId1 = 1, PersonId2 = 2, RelationshipType = "father-son" }
                });

            // Act
            await _familyTreeService.UpdateFamilyTreeNodesAsync(args);

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.DeletePersonAsync(It.IsAny<Person>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Exactly(2));
        }


        [Test]
        public async Task UpdatePersonRelationships_No_Changes_On_Pids_Should_Not_Make_Changes()
        {
            // Arrange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Relationship[] {
                    new Relationship { PersonId1 = 1, PersonId2 = 2, RelationshipType = "spouse" },
                    new Relationship { PersonId1 = 1, PersonId2 = 2, RelationshipType = "father-son" }
                });
            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1", Pids = ["2"] };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string>());

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.IsAny<int>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
        }

        [Test]
        public async Task UpdatePersonRelationships_Add_New_Pids_If_Not_Exist_Empty_Spouse_Relationships()
        {
            //Arange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync([
                    new Relationship { PersonId1 = 1, PersonId2 = 2, RelationshipType = "father-son" }
                ]);

            _familyTreeRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.IsAny<int>())).
                ReturnsAsync(new Person { PersonId = 2, FirstName = "Jane", LastName = "Doe" });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1", Pids = ["2"] };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string> { { "_vT2m", "1" } });
            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.IsAny<int>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
        }

        [Test]
        public async Task UpdatePersonRelationships_Remove_Spouse_If_Not_Exist_In_Pids()
        {
            // Arrange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Relationship[] {
                    new Relationship { PersonId1 = 1, PersonId2 = 2, RelationshipType = "spouse" }
                });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1"};

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string> { { "_vT2m", "1" } });

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.IsAny<int>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Once);
        }

        [Test]
        public async Task UpdatePersonRelationships_Add_New_Pid_Additional_Spouse()
        {
            //Arange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Relationship[] {
                    new Relationship { PersonId1 = 1, PersonId2 = 2, RelationshipType = "spouse" }
                });

            _familyTreeRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.Is<int>(v => v == 2))).
                ReturnsAsync(new Person { PersonId = 2, FirstName = "Jane", LastName = "Doe" });
            _familyTreeRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.Is<int>(v => v == 3))).
                ReturnsAsync(new Person { PersonId = 3, FirstName = "Joan", LastName = "Doe" });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1", Pids = ["2","3"] };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string>());

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.IsAny<int>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
        }

        [Test]
        public async Task UpdatePersonRelationships_Add_New_Pid_Additional_Spouse_Use_NewId_Instead_Of_The_OldId()
        {
            //Arange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync([]);

            _familyTreeRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.Is<int>(v => v == 2))).
                ReturnsAsync(new Person { PersonId = 2, FirstName = "Jane", LastName = "Doe" });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1", Pids = ["_vT2m"] };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string> { { "_vT2m", "2" } });

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.IsAny<int>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.Is<Relationship>(
                    rel => rel.PersonId2 == 2)), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
        }

        [Test]
        public async Task UpdatePersonRelationships_No_Changes_Father_Relationship()
        {
            //Arange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Relationship[] {
                    new Relationship { PersonId1 = 1, PersonId2 = 4, RelationshipType = "father-child" }
                });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1", Fid = "4" };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string> { { "_vT2m", "2" } });

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.IsAny<int>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
        }

        [Test]
        public async Task UpdatePersonRelationships_Add_New_Father_If_Not_Exist()
        {
            //Arange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync([]);

            _familyTreeRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.Is<int>(v => v == 4))).
                ReturnsAsync(new Person { PersonId = 4, FirstName = "Joe", LastName = "Doe" });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1", Fid = "4" };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string> { { "_vT2m", "2" } });
            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.IsAny<int>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
        }

        [Test]
        public async Task UpdatePersonRelationships_Add_New_Father_If_Not_Exist_Use_NewId()
        {
            //Arange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync([]);

            _familyTreeRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.Is<int>(v => v == 4))).
                ReturnsAsync(new Person { PersonId = 4, FirstName = "Joe", LastName = "Doe" });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1", Fid = "_vT2m" };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string> { { "_vT2m", "4" } });
            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.Is<int>(v => v == 4)), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
        }

        [Test]
        public async Task UpdatePersonRelationships_Remove_Father_It_No_Fid()
        {
            //Arange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Relationship[] {
                    new Relationship { PersonId1 = 1, PersonId2 = 4, RelationshipType = "father-child" }
                });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1" };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string> { { "_vT2m", "2" } });
            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.Is<int>(v => v == 4)), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Once);
        }


        [Test]
        public async Task UpdatePersonRelationships_No_Changes_Mother_Relationship()
        {
            // Arrange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Relationship[] {
            new Relationship { PersonId1 = 1, PersonId2 = 5, RelationshipType = "mother-child" }
                });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1", Mid = "5" };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string>());

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.IsAny<int>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
        }

        [Test]
        public async Task UpdatePersonRelationships_Add_New_Mother_If_Not_Exist()
        {
            // Arrange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync([]);

            _familyTreeRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.Is<int>(v => v == 5)))
                .ReturnsAsync(new Person { PersonId = 5, FirstName = "Jane", LastName = "Doe" });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1", Mid = "5" };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string>());

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.IsAny<int>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
        }

        [Test]
        public async Task UpdatePersonRelationships_Add_New_Mother_If_Not_Exist_Use_NewId()
        {
            // Arrange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync([]);

            _familyTreeRepositoryMock.Setup(repo => repo.GetPersonByIdAsync(It.Is<int>(v => v == 5)))
                .ReturnsAsync(new Person { PersonId = 5, FirstName = "Jane", LastName = "Doe" });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1", Mid = "_vT2m" };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string> { { "_vT2m", "5" } });

            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.Is<int>(v => v == 5)), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Once);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
        }

        [Test]
        public async Task UpdatePersonRelationships_Remove_Mother_It_No_Mid()
        {
            // Arrange
            _familyTreeRepositoryMock.Setup(repo => repo.GetSinglePersonRelationshipsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Relationship[] {
            new Relationship { PersonId1 = 1, PersonId2 = 5, RelationshipType = "mother-child" }
                });

            var familyNode = new FamilyNodeDTO { Name = "John Doe", Id = "1" };

            // Act
            await _familyTreeService.UpdatePersonRelationships(familyNode, new Dictionary<string, string> { { "_vT2m", "1" } });
            // Assert
            _familyTreeRepositoryMock.Verify(r => r.GetPersonByIdAsync(It.Is<int>(v => v == 4)), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.AddRelationShipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.UpdateRelationshipAsync(It.IsAny<Relationship>()), Times.Never);
            _familyTreeRepositoryMock.Verify(r => r.DeleteRelationShipAsync(It.IsAny<Relationship>()), Times.Once);
        }        
    }
}
