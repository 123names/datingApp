namespace api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.MainPhotoUrl, option => option.MapFrom(src =>
                    src.UserPhotos.FirstOrDefault(photo => photo.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                    src.DateOfBirth.CalculateAge()));
            CreateMap<UserPhoto, UserPhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderMainPhotoUrl, option => option.MapFrom(src =>
                    src.Sender.UserPhotos.FirstOrDefault(photo => photo.IsMain).Url))
                .ForMember(dest => dest.RecipientMainPhotoUrl, option => option.MapFrom(src =>
                    src.Recipient.UserPhotos.FirstOrDefault(photo => photo.IsMain).Url));
        }
    }
}