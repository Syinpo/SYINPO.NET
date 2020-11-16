using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Syinpo.Model.Dto.Devices;

namespace Syinpo.Model.Validators.Devices {
    public class DeviceSmsDtoValidator : AbstractValidator<DeviceSmsDto> {
        public DeviceSmsDtoValidator() {

            RuleFor( x => x.DeviceId )
                .NotEmpty()
                .WithMessage( "设备主键不能为空" );

            RuleFor( x => x.FromPhone )
                .NotEmpty()
                .WithMessage( "从哪个号码不能为空" );

            RuleFor( x => x.ToPhone )
                .NotEmpty()
                .WithMessage( "到哪个号码不能为空" );

            RuleFor( x => x.Sent )
                .NotEmpty()
                .WithMessage( "是否发送，否则接受不能为空" );

            RuleFor( x => x.Content )
                .NotEmpty()
                .WithMessage( "短信内容不能为空" );

        }
    }
}
