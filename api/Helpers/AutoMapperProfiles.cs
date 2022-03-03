using System.Linq;
using AutoMapper;
using datingApp.api.DTOs;
using datingApp.api.Entities;
using datingApp.api.Extensions;

namespace datingApp.api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.MainPhotoUrl, option => option.MapFrom(src =>
                       src.UserPhotos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                    src.DateOfBirth.CalculateAge()));
            CreateMap<UserPhoto, UserPhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
        }
    }
}