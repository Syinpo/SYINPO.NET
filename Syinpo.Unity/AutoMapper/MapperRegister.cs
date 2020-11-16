using AutoMapper.Configuration;
using Syinpo.Core.Mapper;

namespace Syinpo.Unity.AutoMapper {
    public static class MapperRegister {
        public static void Register() {
            BaseMapperConfiguration.MapperConfigurationExpression.AddProfile<MappingProfiles>();
        }
    }
}
