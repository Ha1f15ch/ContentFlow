using AutoMapper;
using ContentFlow.Application.DTOs;
using ContentFlow.Infrastructure.Identity;

namespace ContentFlow.Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>();
    }
}