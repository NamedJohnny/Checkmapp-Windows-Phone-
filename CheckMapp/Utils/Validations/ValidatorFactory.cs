using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using GalaSoft.MvvmLight.Ioc;
using CheckMapp.Model.Tables;
using CheckMapp.Utils.Validations.Validators;
using CheckMapp.Utils.Validations.Validators.CustomValidators;

namespace CheckMapp.Utils.Validations
{
    public class ValidatorFactory : ValidatorFactoryBase
    {
        public ValidatorFactory()
        {
            RegisterValidators();       
        }

        private void RegisterValidators()
        {
            SimpleIoc.Default.Register<IValidator<Note>, NoteValidator>();
            SimpleIoc.Default.Register<IValidator<Picture>, PhotoValidator>();
            SimpleIoc.Default.Register<IValidator<PointOfInterest>, POIValidator>();
            SimpleIoc.Default.Register<IValidator<Trip>, TripValidator>();
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            return SimpleIoc.Default.GetInstance(validatorType) as IValidator;
        }
    }
}
