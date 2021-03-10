﻿using Collector.Common.Validation.NationalIdentifier.Interface;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Collector.Common.Validation.NationalIdentifier.Validators
{
    /// <remarks>
    /// Allowed format: DDMMYYCZZZQ
    ///    where DD = day, MM = month, YY = year, C = Century sign can have value +, - or A, 
    ///    ZZZ = serial number
    ///    Q = control digit. Uses character table 0123456789ABCDEFHJKLMNPRSTUVWXY where the index of which character to use
    ///        is calculated by taking the remainder of the value DDMMYYZZZ divide by 31.
    /// Century of the birthdate is determined by the Century sign
    ///     + = 1800
    ///     - = 1900
    ///     A = 2000
    /// </remarks>
    public class FinnishNationalIdentifierValidator : NationalIdentifierValidator
    {
        /// <remarks>Regex has been check with SDL Regex Fuzzer tool so it should not be a source for DOS attacks. IF YOU UPDATE THE REGEX recheck it again with the SDL tool.</remarks>>
        private static readonly Regex NationalIdentifierWhitelistValidator =
            new Regex(@"^[0-3]\d[0-1]\d{3}[\-+A]\d{3}[0123456789ABCDEFHJKLMNPRSTUVWXY]$", RegexOptions.ECMAScript);

        private static readonly char[] ControlChar = "0123456789ABCDEFHJKLMNPRSTUVWXY".ToCharArray();

        private static readonly Dictionary<char, int> CenturarySignToCenturaryLookupTable = new Dictionary<char, int>
        {
            {'+', 1800},
            {'-', 1900},
            {'A', 2000}
        };

        public override CountryCode CountryCode => CountryCode.FI;

        public override bool IsValid(string nationalIdentifier)
        {
            if (nationalIdentifier == null || !NationalIdentifierWhitelistValidator.IsMatch(nationalIdentifier))
			{
				return false;
			}

			// nationalIdentifier should contain the following format DDMMYYCZZZQ since it passed the Regex validation
			var birthYear = int.Parse(nationalIdentifier.Substring(4, 2));
            var centuarySign = nationalIdentifier.Substring(6, 1)[0];
            var yearhWithCentury = ExtractYearhWithCentury(birthYear, centuarySign);
            var month = int.Parse(nationalIdentifier.Substring(2, 2));
            var day = int.Parse(nationalIdentifier.Substring(0, 2));

			return IsValidDate(yearhWithCentury, month, day) && HasValidControlDigit(nationalIdentifier);
        }

		public override ParsedNationalIdentifierData Parse(string nationalIdentifier)
		{
			ParsedNationalIdentifierData parsedObj = new ParsedNationalIdentifierData();
			if (nationalIdentifier == null || !NationalIdentifierWhitelistValidator.IsMatch(nationalIdentifier))
			{
				return parsedObj;
			}

			// nationalIdentifier should contain the following format DDMMYYCZZZQ since it passed the Regex validation
			var birthYear = int.Parse(nationalIdentifier.Substring(4, 2));
			var centuarySign = nationalIdentifier.Substring(6, 1)[0];
			var yearhWithCentury = ExtractYearhWithCentury(birthYear, centuarySign);
			var month = int.Parse(nationalIdentifier.Substring(2, 2));
			var day = int.Parse(nationalIdentifier.Substring(0, 2));
			
			parsedObj.Valid = IsValidDate(yearhWithCentury, month, day) && HasValidControlDigit(nationalIdentifier);
			if (parsedObj.Valid)
			{
				var rollingId = nationalIdentifier.Substring(7, 3);
				parsedObj.Gender = int.Parse(rollingId) % 2 == 1 ? Gender.MALE : Gender.FEMALE;
				var birthday = new DateTime(yearhWithCentury, month, day);
                parsedObj.DateOfBirth = birthday;
				parsedObj.AgeInYears = GetAge(birthday);
			}

			
			return parsedObj;
		}

		/// <summary>Transforms valid national identifier into format DDMMYYCZZZQ.</summary>
		/// <exception cref="ArgumentException">If <paramref name="nationalIdentifier"/> is not valid.</exception>
		public override string Normalize(string nationalIdentifier)
        {
            if (!IsValid(nationalIdentifier))
			{
				throw new ArgumentException(ErrorMessages.GetInvalidIdentifierMessage(nationalIdentifier, CountryCode), nameof(nationalIdentifier));
			}

			return nationalIdentifier;
        }

        private static int ExtractYearhWithCentury(int birthYear, char centuarySign)
        {
            var centuary = CenturarySignToCenturaryLookupTable[centuarySign];
            return centuary + birthYear;
        }


        private static bool HasValidControlDigit(string valueToCheck)
        {
            // valueToCheck should have format DDMMYYCZZZQ

            // Check that the control digit is correct
            var civicValue = valueToCheck.Substring(0, 6) + valueToCheck.Substring(7, 3);
            var controlCharIndex = int.Parse(civicValue) % 31;
            var calculatedControlSign = ControlChar[controlCharIndex];

            var controlSign = valueToCheck[10];
            return calculatedControlSign == controlSign;
        }
		
    }
}