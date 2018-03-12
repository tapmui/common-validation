using System.ComponentModel.DataAnnotations;
using Collector.Common.Validation.NationalIdentifier.Validators;

namespace Collector.Common.Validation.NationalIdentifier.Attribute
{
    public class NationalIdentifierAttribute : ValidationAttribute
    {
        private readonly CountryCode? _countryCode;

        public NationalIdentifierAttribute()
        {
        }

        public NationalIdentifierAttribute(CountryCode countryCode)
        {
            _countryCode = countryCode;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var nationalIdentifier = value as string;

            if (_countryCode == null)
            {
                return NationalIdentifierValidator.IsValidInAnyCountry(nationalIdentifier)
                    ? ValidationResult.Success
                    : new ValidationResult(ErrorMessages.GetInvalidIdentifierMessage(nationalIdentifier));
            }

            var countryCode = (CountryCode) _countryCode;
            var validator = NationalIdentifierValidator.GetValidator(countryCode);

            return validator.IsValid(nationalIdentifier)
                ? ValidationResult.Success
                : new ValidationResult(
                    ErrorMessages.GetInvalidIdentifierMessage(nationalIdentifier, countryCode));
        }
    }
}