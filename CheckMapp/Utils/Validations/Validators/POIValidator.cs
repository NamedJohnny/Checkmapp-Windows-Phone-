using System;
using System.Linq;
using FluentValidation;
using CheckMapp.Model.Tables;
using CheckMapp.Resources;

namespace CheckMapp.Utils.Validations.Validators
{
    public class POIValidator : AbstractValidator<PointOfInterest>
    {
        public POIValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(AppResources.Error_EmptyName);
        }
    }
}
