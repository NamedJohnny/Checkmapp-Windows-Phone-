using System;
using System.Linq;
using CheckMapp.Model.Tables;
using FluentValidation.Validators;
using CheckMapp.Resources;

namespace CheckMapp.Utils.Validations.Validators.CustomValidators
{
    public class ValidateTripCoordinates : PropertyValidator
    {
        public ValidateTripCoordinates()
        : base(AppResources.Error_EmptyCoordinates)
        {

        }
 
        protected override bool IsValid(PropertyValidatorContext context)
        {
            Trip trip = context.Instance as Trip;

            if (trip.DepartureLatitude == 0.0 || trip.DepartureLongitude == 0.0 ||
                trip.DestinationLatitude == 0.0 || trip.DestinationLongitude == 0.0)
                return false;

            return true;
        }
    }
}
