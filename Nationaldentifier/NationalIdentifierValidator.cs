using System;
using System.Collections.Generic;

namespace Collector.Common.Validation.NationalIdentifier
{
    public abstract class NationalIdentifierValidator : INationalIdentifierValidator
    {
        private static readonly Dictionary<NationalIdentifierCountry, NationalIdentifierValidator> SignMethodToValidatorMapping = new Dictionary<NationalIdentifierCountry, NationalIdentifierValidator>
            {
                { NationalIdentifierCountry.Sweden, new SwedishNationalIdentifierValidator() },
                { NationalIdentifierCountry.Norway, new NorwegianNationalIdentifierValidator() },
                { NationalIdentifierCountry.Finland, new FinnishNationalIdentifierValidator() },
                { NationalIdentifierCountry.Denmark, new DanishNationalIdentifierValidator() }
            };

        public static INationalIdentifierValidator GetValidator(NationalIdentifierCountry country)
        {
            if(SignMethodToValidatorMapping.ContainsKey(country))
                return SignMethodToValidatorMapping[country];
            throw new ArgumentOutOfRangeException(nameof(country), country, "There is no validation implemented for the given country.");
        }

        public static INationalIdentifierValidator GetValidator(string nationalIdentifier)
        {
            foreach (var validator in SignMethodToValidatorMapping)
            {
                if (validator.Value.IsValid(nationalIdentifier))
                {
                    return SignMethodToValidatorMapping[validator.Value.NationalCountry];
                }
            }
            throw new ArgumentException("National Identifier is not valid");
        }

        /// <summary>Country code for the country that the instance that implements <see cref="NationalIdentifierValidator"/> applies to as ISO 3166-1 alpha-2.</summary>
        public abstract NationalIdentifierCountry NationalCountry { get; }

        public static bool IsValidInAnyCountry(string nationalIdentifier)
        {
            foreach (var validator in SignMethodToValidatorMapping)
            {
                if (validator.Value.IsValid(nationalIdentifier))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsValidInAnyCountry(int nationalIdentifier)
        {
            var nationalIdentifierString = nationalIdentifier.ToString();
            return IsValidInAnyCountry(nationalIdentifierString);
        }

        public static string NormalizeForAnyCountry(string nationalIdentifier)
        {
            foreach (var validator in SignMethodToValidatorMapping)
            {
                if (validator.Value.IsValid(nationalIdentifier))
                {
                    return validator.Value.Normalize(nationalIdentifier);
                }
            }
            throw new ArgumentException("National Identifier is not valid");
        }

        public static string NormalizeForAnyCountry(int nationalIdentifier)
        {
            var nationalIdentifierString = nationalIdentifier.ToString();
            return NormalizeForAnyCountry(nationalIdentifierString);
        }

        public abstract bool IsValid(string nationalIdentifier);
        public abstract bool IsValid(int nationalIdentifier);
        public abstract string Normalize(string nationalIdentifier);
        public abstract string Normalize(int nationalIdentifier);

        internal static bool IsValidDate(int year, int month, int day)
        {
            var maxDaysInMonth = new[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            if (DateTime.IsLeapYear(year))
                maxDaysInMonth[1] = 29;

            return month > 0 && month < 13 && day > 0 && day <= maxDaysInMonth[month - 1];
        }

        internal static long GetDigitAtPosition(long input, int position)
        {
            var divisor = (long)Math.Pow(10, position);
            var lowestDigitContainsDigitWeWant = input / divisor;
            return lowestDigitContainsDigitWeWant % 10;  // Take the remainder of 10 to extract the digit we want
        }
    }
}
