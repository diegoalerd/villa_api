using AutoMapper;
using MagicVilla_API.Dto;
using MagicVilla_API.Models;

namespace MagicVilla_API
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDto>();
            CreateMap<VillaDto, Villa>();

            CreateMap<Villa, VillaCreateDto>().ReverseMap();
            CreateMap<Villa, VillaUpdateDto>().ReverseMap();
        }
    }
}