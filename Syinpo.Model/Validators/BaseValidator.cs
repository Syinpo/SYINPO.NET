using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using Syinpo.Model.Dto;

namespace Syinpo.Model.Validators {
    public abstract class BaseValidator<T> : AbstractValidator<T> {

        protected void MergeValidationResult( CustomContext context, ValidationResult validationResult ) {
            if( !validationResult.IsValid ) {
                foreach( var validationFailure in validationResult.Errors ) {
                    context.AddFailure( validationFailure );
                }
            }
        }
    }
}
