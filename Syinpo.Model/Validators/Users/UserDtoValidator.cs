using FluentValidation;
using Syinpo.Model.Dto.Users;

namespace Syinpo.Model.Validators.Users {
    public class UserDtoValidator : AbstractValidator<UserDto> {
        public UserDtoValidator() {

            RuleFor( x => x.Username )
                .NotEmpty()
                .WithMessage( "用户名不能为空" );

            RuleFor( x => x.DisplayName )
                .NotEmpty()
                .WithMessage( "显示名称不能为空" );

        }
    }

}
