using AREML.EPOD.Core.Entities.Logs;
using AREML.EPOD.Core.Entities.Mappings;
using AREML.EPOD.Core.Entities.Master;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Mapper
{
    public class MasterProfile:Profile
    {
        public MasterProfile()
        {
            CreateMap<UserWithRole, User>().ForMember(dest => dest.UserID, opt => opt.Ignore()).ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.Ignore()).ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now));
           
            CreateMap<UserWithRole, UserCreationErrorLog>().ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.RoleName, opt => opt.Ignore()).ForMember(dest => dest.LogReson, opt => opt.Ignore());
           
            CreateMap<UserWithRole, UserRoleMap>().ForMember(dest => dest.UserID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<RoleWithApp, Role>()
           .ForMember(dest => dest.RoleID, opt => opt.Ignore())
           .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now));

            

        }
    }
}
