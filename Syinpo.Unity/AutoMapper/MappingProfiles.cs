using System;
using System.Linq;
using System.Security.Cryptography;
using Aliyun.Acs.Core.Auth.Sts;
using AutoMapper;
using Syinpo.Core;
using Syinpo.Core.Domain.MonitorPoco;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Helpers;
using Syinpo.Core.Mapper;
using Syinpo.Core.Monitor.PackModule;
using Syinpo.Model.Dto.Devices;
using Syinpo.Model.Dto.Monitor;
using Syinpo.Model.Dto.Users;
using Syinpo.Model.Request.Devices;

namespace Syinpo.Unity.AutoMapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateDeviceMapper();

            CreateDeviceSms();

            CreateUserMapper();

            CreateMonitorMapper();
        }


        protected void CreateUserMapper()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<User, UserForDeviceDto>();
            CreateMap<User, UserForWkfDto>();


            CreateMap<User, UserForConsoleDto>();
            CreateMap<UserForConsoleDto, User>();
        }

        protected void CreateDeviceMapper()
        {
            CreateMap<Device, DeviceDto>();
            CreateMap<DeviceDto, Device>();



            CreateMap<Device, DeviceEditDto>();
            CreateMap<DeviceEditDto, Device>();


            CreateMap<DeviceForRegisterDeviceDto, Device>();
        }

        private void CreateDeviceSms()
        {
            CreateMap<DeviceSms, DeviceSmsDto>();
            CreateMap<DeviceSmsDto, DeviceSms>();
        }

        protected void CreateMonitorMapper()
        {
            CreateMap<HttpLog, RequestLog>()
                .ForMember(dest => dest.CreateTime, opts => opts.MapFrom(src => DateTime.Now));
            CreateMap<RequestLog, HttpLog>();

            CreateMap<RequestLog, RequestLogDto>();
            CreateMap<RequestLogDto, RequestLog>();

            CreateMap<ResponseSnap, ResponseSnapDto>();
            CreateMap<ResponseSnapDto, ResponseSnap>();

            CreateMap<TrafficStatist, TrafficStatistDto>();
            CreateMap<TrafficStatistDto, TrafficStatist>();

            CreateMap<SqlSnap, SqlSnapDto>();
            CreateMap<SqlSnapDto, SqlSnap>();

            CreateMap<ExceptionSnap, ExceptionSnapDto>();
            CreateMap<ExceptionSnapDto, ExceptionSnap>();
        }


    }
}
