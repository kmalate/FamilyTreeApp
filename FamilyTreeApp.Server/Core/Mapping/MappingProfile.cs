using AutoMapper;
using FamilyTreeApp.Server.Core.Models;
using FamilyTreeApp.Server.Infrastructure.Models;

namespace FamilyTreeApp.Server.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PersonRelationship, FamilyNodeDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PersonId.ToString()))
                .ForMember(dest => dest.Pids, opt => opt.MapFrom(src => src.SpousePersonIds != null ? src.SpousePersonIds.Split(new[] { ',' }).Select(int.Parse) : new int[0]))
                .ForMember(dest => dest.Mid, opt => opt.MapFrom(src => src.MotherPersonId))
                .ForMember(dest => dest.Fid, opt => opt.MapFrom(src => src.FatherPersonId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));
        }
    }
}
