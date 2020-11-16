using Syinpo.Core.Domain.Poco;
using Syinpo.Model.Dto.Devices;
using Syinpo.Model.Dto.Users;

namespace Syinpo.Model {
    public interface ICurrent {
        UserDto User {
            get; set;
        }

        DeviceDto Device {
            get; set;
        }

        DeviceDto HttpDevice {
            get;
        }

        User DbUser {
            get; set;
        }


    }
}
