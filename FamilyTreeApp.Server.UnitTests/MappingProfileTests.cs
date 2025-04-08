using AutoMapper;
using FamilyTreeApp.Server.Core.Mapping;
using FamilyTreeApp.Server.Core.Models;
using FamilyTreeApp.Server.Infrastructure.Models;

namespace FamilyTreeApp.Server.UnitTests.Mapping
{
    [TestFixture]
    public class MappingProfileTests
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
        }

        [Test]
        public void MappingProfile_ConfigurationIsValid()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            config.AssertConfigurationIsValid();
        }

        [Test]
        public void Map_PersonRelationshipToFamilyNodeDTO_MapsCorrectly()
        {
            // Arrange
            var personRelationship = new PersonRelationship
            {
                PersonId = 1,
                SpousePersonIds = "2,3",
                MotherPersonId = 4,
                FatherPersonId = 5,
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male"
            };

            // Act
            var familyNodeDTO = _mapper.Map<FamilyNodeDTO>(personRelationship);

            // Assert
            Assert.Multiple(() =>
            {                
                Assert.That(familyNodeDTO.Id.ToString(), Is.EqualTo("1"));
                Assert.That(familyNodeDTO.Pids, Is.EqualTo(new List<string> { "2","3" }));
                Assert.That(familyNodeDTO.Mid, Is.EqualTo("4"));
                Assert.That(familyNodeDTO.Fid, Is.EqualTo("5"));
                Assert.That(familyNodeDTO.Name, Is.EqualTo("John Doe"));
                Assert.That(familyNodeDTO.Gender, Is.EqualTo("Male"));
            });
        }
    }
}
