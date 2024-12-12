using AutoMapper;
using Silence.Web.Entities;
using Silence.Infrastructure.ViewModels;

namespace Silence.Web.Mappings
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomViewModel>()
                .ForMember(dst => dst.Admin, opt => opt.MapFrom(x => x.Admin.UserName));

            CreateMap<RoomViewModel, Room>();
        }
    }
}
