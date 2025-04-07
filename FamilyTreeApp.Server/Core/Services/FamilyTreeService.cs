using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AutoMapper;
using FamilyTreeApp.Server.Core.Interfaces;
using FamilyTreeApp.Server.Core.Models;
using FamilyTreeApp.Server.Infrastructure.Interfaces;
using FamilyTreeApp.Server.Infrastructure.Models;

namespace FamilyTreeApp.Server.Core.Services
{
    public class FamilyTreeService : IFamilyTreeService
    {
        private readonly IFamilyTreeRepository _familyTreeRepository;
        private readonly IMapper _mapper;

        private async Task<Dictionary<string, string>> AddNewPersonRelationshipAsync(FamilyNodeDTO addNodesData)
        {
            // new person
            var nameSplit = addNodesData.Name.Split(" ");
            var newPerson = new Person
            {
                FirstName = nameSplit.Length > 0 ? nameSplit[0] : string.Empty,
                LastName = nameSplit.Length > 1 ? nameSplit[1] : string.Empty,
                Gender = addNodesData.Gender
            };
            
            newPerson = await _familyTreeRepository.AddPersonAsync(newPerson);
            // new relationship/s
            var addRelationshipsTaskList = new List<Task>();
            // spouses
            if (addNodesData.Pids != null && addNodesData.Pids.Any())
            {
                var partners = await _familyTreeRepository.GetAllPersonsByIdListAsync(addNodesData.Pids.ToArray());

                if (partners.Any())
                {                   
                    foreach (var partner in partners)
                    {
                        var newRelationship = new Relationship
                        {
                            PersonId1 = newPerson.PersonId,
                            PersonId2 = partner.PersonId,
                            RelationshipType = "spouse"
                        };
                        addRelationshipsTaskList.Add(_familyTreeRepository.AddRelationShipAsync(newRelationship));
                    }
                }                
            }
            // father
            if (addNodesData.Fid != null)
            {
                var father = await _familyTreeRepository.GetPersonByIdAsync(addNodesData.Fid.Value);

                if (father != null)
                {
                    var newRelationship = new Relationship
                    {
                        PersonId1 = newPerson.PersonId,
                        PersonId2 = father.PersonId,
                        RelationshipType = "father-child"
                    };
                    addRelationshipsTaskList.Add(_familyTreeRepository.AddRelationShipAsync(newRelationship));
                }
            }

            // mother
            if (addNodesData.Mid != null)
            {
                var mother = await _familyTreeRepository.GetPersonByIdAsync(addNodesData.Mid.Value);
                if (mother != null)
                {
                    var newRelationship = new Relationship
                    {
                        PersonId1 = newPerson.PersonId,
                        PersonId2 = mother.PersonId,
                        RelationshipType = "mother-child"
                    };
                    addRelationshipsTaskList.Add(_familyTreeRepository.AddRelationShipAsync(newRelationship));
                }
            }

            await Task.WhenAll(addRelationshipsTaskList);
            return new Dictionary<string, string>
            {
                { addNodesData.Id, newPerson.PersonId.ToString() }
            };
        }

        public FamilyTreeService(IFamilyTreeRepository familyTreeRepository,
            IMapper mapper)
        {
            _familyTreeRepository = familyTreeRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<FamilyNodeDTO>> GetFamilyTreeNodesAsync()
        {
            var personRelationships = await _familyTreeRepository.GetPersonRelationshipListAsync();
            return _mapper.Map<IEnumerable<FamilyNodeDTO>>(personRelationships);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Dictionary<string, string>>> UpdateFamilyTreeNodesAsync(UpdateNodeArgsDTO updateNodeArgs)
        {
            var oldIdNewId = new List<Dictionary<string, string>>();
            // Add
            if (updateNodeArgs.AddNodesData.Length > 0)
            {
                var addTaskList = new List<Task<Dictionary<string, string>>>();
                foreach (var node in updateNodeArgs.AddNodesData)
                {
                    addTaskList.Add(AddNewPersonRelationshipAsync(node));
                }

                Task.WaitAll(addTaskList.ToArray());
                foreach (var task in addTaskList)
                {
                    var result = task.Result;
                    if (result != null)
                    {
                        oldIdNewId.Add(result);
                    }
                }
            }

            // Update
            if (updateNodeArgs.UpdateNodesData.Length > 0)
            {
                var updateTaskList = new List<Task>();
                foreach (var node in updateNodeArgs.UpdateNodesData)
                {
                    var person = await _familyTreeRepository.GetPersonByIdAsync(int.Parse(node.Id));
                    if (person != null)
                    {
                        var nameSplit = node.Name.Split(" ");
                        person.FirstName = nameSplit[0];
                        person.LastName = nameSplit[1];
                        person.Gender = node.Gender;
                        updateTaskList.Add(_familyTreeRepository.UpdatePersonAsync(person));
                    }
                    
                }

                Task.WaitAll([.. updateTaskList]);
            }
            
            return oldIdNewId.AsEnumerable();
        }
    }
}
