using Collector.Common.Validation.NationalIdentifier.Interface;
using System;
using System.Text.RegularExpressions;

namespace Collector.Common.Validation.NationalIdentifier.Validators
{
    /// <remarks>
    /// Allowed formats: DD.MM.YYYY
    ///    where DD = day, MM = month, YY = year
    /// </remarks>
    public class GermanyNationalBirthdayIdentifierValidator : NationalIdentifierValidator
    {
        /// <remarks>Regex has been check with SDL Regex Fuzzer tool so it should not be a source for DOS attacks. IF YOU UPDATE THE REGEX recheck it again with the SDL tool.</remarks>>
        private static readonly Regex NationalIdentifierWhitelistValidator =
            new Regex(@"^(\d{2}\.\d{2}\.\d{4})$", RegexOptions.ECMAScript);


        public override CountryCode CountryCode => CountryCode.DE;

        public override bool IsValid(string nationalIdentifier)
        {
            if (nationalIdentifier == null)
                return false;
            
            if (!NationalIdentifierWhitelistValidator.IsMatch(nationalIdentifier))
            {
                return false;
            }

                var valueToCheck = nationalIdentifier;
            // valueToCheck should contain the following format DD.MM.YYYY
            var day = int.Parse(valueToCheck.Substring(0, 2));
            var month = int.Parse(valueToCheck.Substring(3, 2));
            var yearhWithCentury = int.Parse(valueToCheck.Substring(6, 4));

            return IsValidDate(yearhWithCentury, month, day);
        }

		public override ParsedNationalIdentifierData Parse(string nationalIdentifier)
		{
			ParsedNationalIdentifierData parsedObj = new ParsedNationalIdentifierData();
            parsedObj.Valid = IsValid(nationalIdentifier);

            if (parsedObj.Valid)
			{
				
                parsedObj.Gender = Gender.UNKNOWN;
                var valueToCheck = nationalIdentifier;
                // valueToCheck should contain the following format DD.MM.YYYY
                var day = int.Parse(valueToCheck.Substring(0, 2));
                var month = int.Parse(valueToCheck.Substring(3, 2));
                var yearhWithCentury = int.Parse(valueToCheck.Substring(6, 4));
                var birthday = new DateTime(yearhWithCentury, month, day);
				parsedObj.DateOfBirth = birthday;
				parsedObj.AgeInYears = GetAge(birthday);
			}
			
			return parsedObj;
		}

		/// <summary>Transforms valid national identifier into format YYYYMMDD.</summary>
		/// <exception cref="ArgumentException">If <paramref name="nationalIdentifier"/> is not valid.</exception>
		public override string Normalize(string nationalIdentifier)
        {
            if (!IsValid(nationalIdentifier))
                throw new ArgumentException(
                    ErrorMessages.GetInvalidIdentifierMessage(nationalIdentifier, CountryCode),
                    nameof(nationalIdentifier));

            var valueToCheck = nationalIdentifier;
            // valueToCheck should contain the following format DD.MM.YYYY
            var day = int.Parse(valueToCheck.Substring(0, 2));
            var month = int.Parse(valueToCheck.Substring(3, 2));
            var yearhWithCentury = int.Parse(valueToCheck.Substring(6, 4));
            return yearhWithCentury.ToString() + month.ToString("00") + day.ToString("00");
        }

    }
}