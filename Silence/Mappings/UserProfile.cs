using AutoMapper;
using Silence.Web.Entities;
using Silence.Infrastructure.ViewModels;

namespace Silence.Web.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(x => x.UserName));

            CreateMap<UserViewModel, User>();
        }
    }
}
