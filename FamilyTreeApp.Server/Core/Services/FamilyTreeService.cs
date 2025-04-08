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
        private async Task<Dictionary<string, string>> AddNewPersonAsync(FamilyNodeDTO node)
        {
            var nameSplit = node.Name.Split(" ");
            var newPerson = new Person
            {
                FirstName = nameSplit.Length > 0 ? nameSplit[0] : string.Empty,
                LastName = nameSplit.Length > 1 ? nameSplit[1] : string.Empty,
                Gender = node.Gender
            };

            newPerson = await _familyTreeRepository.AddPersonAsync(newPerson);
            return new Dictionary<string, string>
            {
                { node.Id, newPerson.PersonId.ToString() }
            };
        }

        private async Task AddNewPersonRelationshipAsync(FamilyNodeDTO addNodesData, IEnumerable<Dictionary<string, string>> oldIdNewIdList)
        {            
            // new relationship/s
            if (oldIdNewIdList.Any())
            {
                var oldIdNewId = oldIdNewIdList.FirstOrDefault(x => x.ContainsKey(addNodesData.Id));
                if (oldIdNewId != null)
                {
                    var addRelationshipsTaskList = new List<Task>();
                    var newPersonId = int.Parse(oldIdNewId[addNodesData.Id]);
                    // spouses
                    if (addNodesData.Pids != null && addNodesData.Pids.Any())
                    {
                        // replace old Pids with new Ids
                        var intIdlist = new List<int>();
                        foreach (var pid in addNodesData.Pids)
                        {
                            if (oldIdNewId.ContainsKey(pid))
                            {
                                intIdlist.Add(int.Parse(oldIdNewId[pid]));
                            }
                            else
                            {
                                intIdlist.Add(int.Parse(pid));
                            }
                        }
                        
                        var partners = await _familyTreeRepository.GetAllPersonsByIdListAsync([.. intIdlist]);
                        if (partners.Any())
                        {
                            foreach (var partner in partners)
                            {
                                var newRelationship = new Relationship
                                {
                                    PersonId1 = newPersonId,
                                    PersonId2 = partner.PersonId,
                                    RelationshipType = "spouse"
                                };
                                addRelationshipsTaskList.Add(_familyTreeRepository.AddRelationShipAsync(newRelationship));
                            }
                        }
                    }

                    // father
                    if (!string.IsNullOrEmpty(addNodesData.Fid))
                    {
                        //replace old Fid with new Id
                        var oldNewFid = oldIdNewIdList.FirstOrDefault(x => x.ContainsKey(addNodesData.Fid));
                        if (oldNewFid != null)
                        {
                            addNodesData.Fid = oldNewFid[addNodesData.Fid];
                        }

                        var father = await _familyTreeRepository.GetPersonByIdAsync(int.Parse(addNodesData.Fid));

                        if (father != null)
                        {
                            var newRelationship = new Relationship
                            {
                                PersonId1 = newPersonId,
                                PersonId2 = father.PersonId,
                                RelationshipType = "father-child"
                            };
                            addRelationshipsTaskList.Add(_familyTreeRepository.AddRelationShipAsync(newRelationship));
                        }
                    }

                    // mother
                    if (!string.IsNullOrEmpty(addNodesData.Mid))
                    {
                        //replace old Mid with new Id
                        var oldNewMid = oldIdNewIdList.FirstOrDefault(x => x.ContainsKey(addNodesData.Mid));
                        if (oldNewMid != null)
                        {
                            addNodesData.Mid = oldNewMid[addNodesData.Mid];
                        }

                        var mother = await _familyTreeRepository.GetPersonByIdAsync(int.Parse(addNodesData.Mid));
                        if (mother != null)
                        {
                            var newRelationship = new Relationship
                            {
                                PersonId1 = newPersonId,
                                PersonId2 = mother.PersonId,
                                RelationshipType = "mother-child"
                            };
                            addRelationshipsTaskList.Add(_familyTreeRepository.AddRelationShipAsync(newRelationship));
                        }
                    }

                    await Task.WhenAll(addRelationshipsTaskList);
                }
            }
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
                // Add new person/s first to get their database ids
                var addPersonTaskList = new List<Task<Dictionary<string, string>>>();
                foreach (var node in updateNodeArgs.AddNodesData)
                {

                    addPersonTaskList.Add(this.AddNewPersonAsync(node));
                }

                Task.WaitAll([.. addPersonTaskList]);
                
                foreach (var task in addPersonTaskList)
                {
                    var result = task.Result;
                    if (result != null)
                    {
                        oldIdNewId.Add(result);
                    }
                }

                //Add new relationships
                var addRelationshipTaskList = new List<Task>();
                foreach (var node in updateNodeArgs.AddNodesData)
                {
                    addRelationshipTaskList.Add(AddNewPersonRelationshipAsync(node, oldIdNewId));
                }

                Task.WaitAll([.. addRelationshipTaskList]);
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
                //TODO: Insert update relationship changes
            }
            
            return oldIdNewId.AsEnumerable();
        }
    }
}
