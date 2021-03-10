using Collector.Common.Validation.NationalIdentifier.Interface;
using System;
using System.Text.RegularExpressions;

namespace Collector.Common.Validation.NationalIdentifier.Validators
{
    /// <remarks>
    /// Allowed formats: DDMMYYSSSS or DDMMYY-SSSS
    ///    where DD = day, MM = month, YY = year and SSSS = is serial number
    /// Century of the birthdate is determined by the serial number
    ///     0001–3999 = 1900–1999.
    ///     4000-4999 = 1937-1999 or 2000-2036
    ///     5000-8999 = 1858-1899 or 2000-2057
    ///     9000-9999 = 1937-1999 or 2000-2036
    /// For temporary National Identifier 6 is added at the beginning i.e. 60 is added to the day so 01 becomes 61 and 30 becomes 90.
    /// </remarks>
    public class DanishNationalIdentifierValidator : NationalIdentifierValidator
    {
        /// <remarks>Regex has been check with SDL Regex Fuzzer tool so it should not be a source for DOS attacks. IF YOU UPDATE THE REGEX recheck it again with the SDL tool.</remarks>>
        private static readonly Regex NationalIdentifierWhitelistValidator =
            new Regex(@"^([0-3]\d[0-1]\d{3}\-?\d{4})$", RegexOptions.ECMAScript);

        /// <remarks>Regex has been check with SDL Regex Fuzzer tool so it should not be a source for DOS attacks. IF YOU UPDATE THE REGEX recheck it again with the SDL tool.</remarks>>
        private static readonly Regex TemporaryNationalIdentifierWhitelistValidator =
            new Regex(@"^([6-9]\d[0-1]\d{3}\-?\d{4})$", RegexOptions.ECMAScript);

        public override CountryCode CountryCode => CountryCode.DK;

        public override bool IsValid(string nationalIdentifier)
        {
            if (nationalIdentifier == null)
                return false;

            var isTemporary = false;
            if (!NationalIdentifierWhitelistValidator.IsMatch(nationalIdentifier))
            {
                if (!TemporaryNationalIdentifierWhitelistValidator.IsMatch(nationalIdentifier))
                    return false;

                isTemporary = true;
            }

            var valueToCheck = nationalIdentifier.Replace("-", "");
            // valueToCheck should contain the following format DDMMYYSSSS
            var birthYear = int.Parse(valueToCheck.Substring(4, 2));
            var serialnumber = int.Parse(valueToCheck.Substring(6, 4));
            var yearhWithCentury = ExtractYearhWithCentury(birthYear, serialnumber);
            var month = int.Parse(valueToCheck.Substring(2, 2));
            var day = int.Parse(valueToCheck.Substring(0, 2));
            if (isTemporary)
                day -= 60; // 60 is added to the day of temporary numbers so subtract away it

            return IsValidDate(yearhWithCentury, month, day);
        }

		public override ParsedNationalIdentifierData Parse(string nationalIdentifier)
		{
			ParsedNationalIdentifierData parsedObj = new ParsedNationalIdentifierData();
			if (nationalIdentifier == null)
				return parsedObj;

			var isTemporary = false;
			if (!NationalIdentifierWhitelistValidator.IsMatch(nationalIdentifier))
			{
				if (!TemporaryNationalIdentifierWhitelistValidator.IsMatch(nationalIdentifier))
					return parsedObj;

				isTemporary = true;
			}

			var valueToCheck = nationalIdentifier.Replace("-", "");
			// valueToCheck should contain the following format DDMMYYSSSS
			var birthYear = int.Parse(valueToCheck.Substring(4, 2));
			var serialnumber = int.Parse(valueToCheck.Substring(6, 4));
			var yearhWithCentury = ExtractYearhWithCentury(birthYear, serialnumber);
			var month = int.Parse(valueToCheck.Substring(2, 2));
			var day = int.Parse(valueToCheck.Substring(0, 2));
			if (isTemporary)
				day -= 60; // 60 is added to the day of temporary numbers so subtract away it

			parsedObj.Valid = IsValidDate(yearhWithCentury, month, day);
			if (parsedObj.Valid)
			{
				//the last digit of the sequence number is odd for males and even for females.
				parsedObj.Gender = serialnumber % 2 == 1 ? Gender.MALE : Gender.FEMALE;
				var birthday = new DateTime(yearhWithCentury, month, day);
                parsedObj.DateOfBirth = birthday;
                parsedObj.AgeInYears = GetAge(birthday);
			}
			
			return parsedObj;
		}

		/// <summary>Transforms valid national identifier into format DDMMYYSSSS.</summary>
		/// <exception cref="ArgumentException">If <paramref name="nationalIdentifier"/> is not valid.</exception>
		public override string Normalize(string nationalIdentifier)
        {
            if (!IsValid(nationalIdentifier))
                throw new ArgumentException(ErrorMessages.GetInvalidIdentifierMessage(nationalIdentifier, CountryCode),
                    nameof(nationalIdentifier));

            return nationalIdentifier.Replace("-", "");
        }

        private static int ExtractYearhWithCentury(int birthYear, int serialNumber)
        {
            /*
             * Century of the birthdate is determined by the serial number
             *   0001–3999 = 1900–1999.
             *   4000-4999 = 1937-1999 or 2000-2036
             *   5000-8999 = 1858-1899 or 2000-2057
             *   9000-9999 = 1937-1999 or 2000-2036
             */
            var century = 0;
            if (serialNumber >= 0001 && serialNumber <= 3999 ||
                serialNumber >= 4000 && serialNumber <= 4999 && birthYear >= 37 ||
                serialNumber >= 9000 && serialNumber <= 9999 && birthYear >= 37)
            {
                century = 1900;
            }
            else if (serialNumber >= 5000 && serialNumber <= 8999 && birthYear <= 57 ||
                     serialNumber >= 4000 && serialNumber <= 4999 && birthYear <= 36 ||
                     serialNumber >= 9000 && serialNumber <= 9999 && birthYear <= 36)
            {
                century = 2000;
            }
            else if (serialNumber >= 5000 && serialNumber <= 8999 && birthYear >= 58)
            {
                century = 1800;
            }

            return century + birthYear;
        }
    }
}