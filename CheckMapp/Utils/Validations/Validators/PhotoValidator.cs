using System;
using System.Linq;
using FluentValidation;
using CheckMapp.Model.Tables;
using CheckMapp.Resources;

namespace CheckMapp.Utils.Validations.Validators
{
    public class PhotoValidator : AbstractValidator<Picture>
    {
        public PhotoValidator()
        {
            RuleFor(x => x.PictureData).NotNull().WithMessage(AppResources.Error_EmptyPicture);
        }
    }
}
