using AutoMapper;
using ContentFlow.Application.DTOs;
using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.Identity;

namespace ContentFlow.Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>();

        CreateMap<Post, PostDto>();

        CreateMap<Comment, CommentDto>();

        CreateMap<Tag, TagDto>();
        
        CreateMap<Category, CategoryDto>();
    }
}