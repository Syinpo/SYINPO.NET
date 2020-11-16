using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Syinpo.Model.Dto.Devices;

namespace Syinpo.Model.Validators.Devices {
    public class DeviceDtoValidator : AbstractValidator<DeviceDto> {
        public DeviceDtoValidator() {

            RuleFor( x => x.DeviceUuid )
                .NotEmpty()
                .WithMessage( "设备IMIE不能为空" );

            RuleFor( x => x.Approved )
                .NotEmpty()
                .WithMessage( "批准不能为空" );

        }
    }
}
