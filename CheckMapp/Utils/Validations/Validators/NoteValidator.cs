using System;
using System.Linq;
using FluentValidation;
using CheckMapp.Model.Tables;
using CheckMapp.Resources;

namespace CheckMapp.Utils.Validations.Validators
{
    public class NoteValidator : AbstractValidator<Note>
    {
        public NoteValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(AppResources.Error_EmptyName);
            RuleFor(x => x.Message).NotEmpty().WithMessage(AppResources.Error_EmptyMessage);
        }
    }
}
