﻿using AutoMapper;
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
        private async Task<KeyValuePair<string, int>> AddNewPersonAsync(FamilyNodeDTO node)
        {
            var nameSplit = node.Name.Split(" ");
            var newPerson = new Person
            {
                FirstName = nameSplit.Length > 0 ? nameSplit[0] : string.Empty,
                LastName = nameSplit.Length > 1 ? nameSplit[1] : string.Empty,
                Gender = node.Gender
            };

            newPerson = await _familyTreeRepository.AddPersonAsync(newPerson);

            return new KeyValuePair<string, int> (node.Id, newPerson.PersonId);
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
        public async Task<Dictionary<string, string>> UpdateFamilyTreeNodesAsync(UpdateNodeArgsDTO updateNodeArgs)
        {
            var oldIdNewId = new Dictionary<string, string>();
            // Add
            if (updateNodeArgs.AddNodesData.Length > 0)
            {
                // Add new person/s first to get their database ids                
                foreach (var node in updateNodeArgs.AddNodesData)
                {

                    var result = await this.AddNewPersonAsync(node);
                    oldIdNewId.Add(result.Key, result.Value.ToString());
                }

                //Add new relationships                
                foreach (var node in updateNodeArgs.AddNodesData)
                {
                    await UpdatePersonRelationships(node, oldIdNewId);
                }
            }

            // Update
            if (updateNodeArgs.UpdateNodesData.Length > 0)
            {
                foreach (var node in updateNodeArgs.UpdateNodesData)
                {
                    var person = await _familyTreeRepository.GetPersonByIdAsync(int.Parse(node.Id));
                    if (person != null)
                    {
                        var nameSplit = node.Name.Split(" ");
                        person.FirstName = nameSplit[0];
                        person.LastName = nameSplit[1];
                        person.Gender = node.Gender;
                        await _familyTreeRepository.UpdatePersonAsync(person);
                        await UpdatePersonRelationships(node, oldIdNewId);
                    }
                }
            }            

            return oldIdNewId;
        }

        /// <inheritdoc />
        public async Task UpdatePersonRelationships(FamilyNodeDTO familyNodeDTO, Dictionary<string, string> oldIdNewId)
        {
            var personId = oldIdNewId.ContainsKey(familyNodeDTO.Id) ? oldIdNewId[familyNodeDTO.Id] : familyNodeDTO.Id;
            var relationShipList = await _familyTreeRepository.GetSinglePersonRelationshipsAsync(int.Parse(personId));

            // Spouse/s
            if (familyNodeDTO.Pids != null && familyNodeDTO.Pids.Any())
            {
                var spouses = relationShipList.Where(r => r.RelationshipType.Equals("spouse", StringComparison.OrdinalIgnoreCase));
                
                foreach (var item in familyNodeDTO.Pids.Where(p => !spouses.Any(s => s.PersonId2.ToString() == p)))
                {
                    //use newId if exists
                    var spouseId = (oldIdNewId.ContainsKey(item)) ? oldIdNewId[item] : item;

                    var spouse = await _familyTreeRepository.GetPersonByIdAsync(int.Parse(spouseId));
                    if (spouse != null)
                    {
                        var newRelationship = new Relationship
                        {
                            PersonId1 = int.Parse(personId),
                            PersonId2 = spouse.PersonId,
                            RelationshipType = "spouse"
                        };
                        await _familyTreeRepository.AddRelationShipAsync(newRelationship);
                    }
                }               
            }

            //Father
            if (!string.IsNullOrEmpty(familyNodeDTO.Fid))
            {
                var father = relationShipList.FirstOrDefault(r => r.RelationshipType.Equals("father-child", StringComparison.OrdinalIgnoreCase));
                if (father == null)
                {
                    //use newId if exists
                    var fatherId = (oldIdNewId.ContainsKey(familyNodeDTO.Fid)) ? oldIdNewId[familyNodeDTO.Fid] : familyNodeDTO.Fid;
                    var fatherPerson = await _familyTreeRepository.GetPersonByIdAsync(int.Parse(fatherId));
                    if (fatherPerson != null)
                    {
                        var newRelationship = new Relationship
                        {
                            PersonId1 = int.Parse(personId),
                            PersonId2 = fatherPerson.PersonId,
                            RelationshipType = "father-child"
                        };
                        await _familyTreeRepository.AddRelationShipAsync(newRelationship);
                    }
                }
            }

            //Mother
            if (!string.IsNullOrEmpty(familyNodeDTO.Mid))
            {
                var mother = relationShipList.FirstOrDefault(r => r.RelationshipType.Equals("mother-child", StringComparison.OrdinalIgnoreCase));
                if (mother == null)
                {
                    //use newId if exists
                    var motherId = (oldIdNewId.ContainsKey(familyNodeDTO.Mid)) ? oldIdNewId[familyNodeDTO.Mid] : familyNodeDTO.Mid;
                    var motherPerson = await _familyTreeRepository.GetPersonByIdAsync(int.Parse(motherId));
                    if (motherPerson != null)
                    {
                        var newRelationship = new Relationship
                        {
                            PersonId1 = int.Parse(personId),
                            PersonId2 = motherPerson.PersonId,
                            RelationshipType = "mother-child"
                        };
                        await _familyTreeRepository.AddRelationShipAsync(newRelationship);
                    }
                }
            }
        }
    }
}
