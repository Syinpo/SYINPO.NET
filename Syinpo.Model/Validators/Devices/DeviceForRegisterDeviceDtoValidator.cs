using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Syinpo.Model.Dto.Devices;

namespace Syinpo.Model.Validators.Devices {
    public class DeviceForRegisterDeviceDtoValidator : AbstractValidator<DeviceForRegisterDeviceDto> {
        public DeviceForRegisterDeviceDtoValidator() {

            RuleFor( x => x.DeviceUuid )
                .NotEmpty()
                .WithMessage( "设备IMIE不能为空" );

        }
    }
}
