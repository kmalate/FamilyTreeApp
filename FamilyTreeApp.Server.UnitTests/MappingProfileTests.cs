using AutoMapper;
using FamilyTreeApp.Server.Core.Mapping;
using FamilyTreeApp.Server.Core.Models;
using FamilyTreeApp.Server.Infrastructure.Models;
using NUnit.Framework;
using System.Collections.Generic;

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
            Assert.AreEqual(1, familyNodeDTO.Id);
            Assert.AreEqual(new List<int> { 2, 3 }, familyNodeDTO.Pids);
            Assert.AreEqual(4, familyNodeDTO.Mid);
            Assert.AreEqual(5, familyNodeDTO.Fid);
            Assert.AreEqual("John Doe", familyNodeDTO.Name);
            Assert.AreEqual("Male", familyNodeDTO.Gender);
        }
    }
}
