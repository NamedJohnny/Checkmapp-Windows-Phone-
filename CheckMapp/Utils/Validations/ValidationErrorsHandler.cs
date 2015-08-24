using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CheckMapp.Model.Tables;
using FluentValidation.Results;
using System.Windows;
using CheckMapp.Resources;

namespace CheckMapp.Utils.Validations
{
    public static class ValidationErrorsHandler
    {
        public static bool IsValid(dynamic dynamicValidator, Object itemToVerify)
        {
            string type = dynamicValidator.GetType().FullName;

            ValidationResult validationResult = getResult(type, dynamicValidator, itemToVerify);

            if (validationResult != null)
            {
                if (!validationResult.IsValid)
                {
                    ShowErrors(validationResult.Errors);

                    return false;
                }
            }

            return true;
        }

        private static ValidationResult getResult(string type, dynamic dynamicValidator, Object itemToVerify)
        {
            if (type.Contains("NoteValidator") && itemToVerify.GetType() == typeof(Note))
            {
                IValidator<Note> validator = dynamicValidator;
                return validator.Validate(itemToVerify);
            }
            else if (type.Contains("TripValidator") && itemToVerify.GetType() == typeof(Trip))
            {
                IValidator<Trip> validator = dynamicValidator;
                return validator.Validate(itemToVerify);
            }
            else if (type.Contains("POIValidator") && itemToVerify.GetType() == typeof(PointOfInterest))
            {
                IValidator<PointOfInterest> validator = dynamicValidator;
                return validator.Validate(itemToVerify);
            }
            else if (type.Contains("PhotoValidator") && itemToVerify.GetType() == typeof(Picture))
            {
                IValidator<Picture> validator = dynamicValidator;
                return validator.Validate(itemToVerify);
            }
            else
                return null;
        }

        private static void ShowErrors(IEnumerable<ValidationFailure> errors)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(AppResources.ErrorsDetected);
            foreach (ValidationFailure error in errors)
            {
                builder.AppendLine("- " + error.ErrorMessage);
            }

            MessageBox.Show(builder.ToString(), AppResources.Errors, MessageBoxButton.OK);
        }
    }
}
