using System;
using System.Linq;
using FluentValidation;
using CheckMapp.Model.Tables;
using CheckMapp.Utils.Validations.Validators.CustomValidators;
using CheckMapp.Resources;

namespace CheckMapp.Utils.Validations.Validators
{
    public class TripValidator : AbstractValidator<Trip>
    {
        public TripValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(AppResources.Error_EmptyName);
            RuleFor(x => x.DepartureLatitude).SetValidator(new ValidateTripCoordinates());
            RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.BeginDate.Date).WithMessage(AppResources.ValidatorEndDate)
                .LessThanOrEqualTo(DateTime.Now.Date).WithMessage(AppResources.ValidatorEndDateNow)
                .Unless(x => x.EndDate == null).WithMessage(AppResources.ValidatorEndDateNow);
        }
    }
}
