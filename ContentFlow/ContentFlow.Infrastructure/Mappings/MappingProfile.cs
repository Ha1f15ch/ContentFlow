using AutoMapper;
using ContentFlow.Application.DTOs;
using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.Identity;

namespace ContentFlow.Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Post, PostDto>();

        CreateMap<Comment, CommentDto>();

        CreateMap<Tag, TagDto>();
        
        CreateMap<Category, CategoryDto>();
        
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dto => dto.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dto => dto.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dto => dto.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dto => dto.AvatarUrl, opt => opt.MapFrom(src => src.AuthorAvatar ?? "https://example.com/avatar-placeholder.png"))
            .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}