using System;
using System.Text.RegularExpressions;

namespace Collector.Common.Validation.NationalIdentifier.Validators
{
    /// <remarks>
    /// Allowed formats: YY-MM-DD-NNNC, YY-MM-DD+NNNC, YYYY-MM-DD-NNNC or YYYY-MM-DD+NNNC
    ///    where DD = day, MM = month, YY = year, NNN = serial number, C = control digits
    /// Hyphen(-) and plus(+) is optional. For YYMMDDNNNC when the last -/+ is missing then hyphen is assumed(-).
    /// 
    /// Century of the birthdate is determined by format if YYYY then the century is already included
    /// If YY then the century is determined by taking current century, (i.e. 2000) and adding the year to it,
    /// if the date is into the future then 100 is subtract. If the + sign is used then the hundred is also subtract from the final value.
    /// For temporary National Identifier 60 is added to the day i.e. day 01 becomes 61 and day 31 ecomes 91.
    /// </remarks>
    public class SwedishNationalIdentifierValidator : NationalIdentifierValidator
    {
        /// <remarks>Regex has been check with SDL Regex Fuzzer tool so it should not be a source for DOS attacks. IF YOU UPDATE THE REGEX recheck it again with the SDL tool.</remarks>>
        private static readonly Regex NationalIdentifierWhitelistValidator =
            new Regex(@"^((\d{2}|\d{4})\-?[0-1]\d\-?[0-3]\d[\-+]?\d{4})$", RegexOptions.ECMAScript);

        /// <remarks>Regex has been check with SDL Regex Fuzzer tool so it should not be a source for DOS attacks. IF YOU UPDATE THE REGEX recheck it again with the SDL tool.</remarks>>
        private static readonly Regex TemporaryNationalIdentifierWhitelistValidator =
            new Regex(@"^((\d{2}|\d{4})\-?[0-1]\d\-?[6-9]\d[\-+]?\d{4})$", RegexOptions.ECMAScript);

        public override NationalIdentifierCountry NationalCountry => NationalIdentifierCountry.Sweden;

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

            var hasPlusSign = nationalIdentifier.Contains("+");
            var valueToCheck = nationalIdentifier.Replace("-", string.Empty).Replace("+", string.Empty);
            var yearhWithCentury = ExtractYearhWithCentury(valueToCheck, hasPlusSign);

            // Since we no longer need centuary information remove it if it existed
            if (valueToCheck.Length == 12)
                valueToCheck = valueToCheck.Substring(2);

            // valueToCheck should contain the following format YYMMDDNNNC
            var month = int.Parse(valueToCheck.Substring(2, 2));
            var day = int.Parse(valueToCheck.Substring(4, 2));
            if (isTemporary)
                day -= 60; // 60 is added to the day of temporary numbers so subtract away it

            return IsValidDate(yearhWithCentury, month, day) && HasValidControlDigit(valueToCheck);
        }

        /// <summary>Transforms valid national identifier into format YYYYMMDDNNNC.</summary>
        /// <exception cref="ArgumentException">If <paramref name="nationalIdentifier"/> is not valid.</exception>
        public override string Normalize(string nationalIdentifier)
        {
            if (!IsValid(nationalIdentifier))
                throw new ArgumentException(
                    ErrorMessages.GetInvalidIdentifierMessage(nationalIdentifier, NationalCountry),
                    nameof(nationalIdentifier));

            var hasPlusSign = nationalIdentifier.Contains("+");
            var valueToCheck = nationalIdentifier.Replace("-", string.Empty).Replace("+", string.Empty);

            // valueToCheck should contain either YYYYMMDDNNNC or YYMMDDNNNC
            if (valueToCheck.Length == 12)
                return valueToCheck;

            var yearhWithCentury = ExtractYearhWithCentury(valueToCheck, hasPlusSign);
            return yearhWithCentury + valueToCheck.Substring(2);
        }

        private static int ExtractYearhWithCentury(string nationalIdentifierWhereHyphenAndPlusHasBeenStripped,
                                                   bool hasPlusSign)
        {
            // Should contain the following format YYMMDDNNNC or YYYYMMDDNNNC if YYYYMMDDNNNC then return YYYY
            // otherwise figure out the centuary based on YY and current year and hasPlusSign.

            if (nationalIdentifierWhereHyphenAndPlusHasBeenStripped.Length > 10)
                return int.Parse(nationalIdentifierWhereHyphenAndPlusHasBeenStripped.Substring(0, 4));

            var currentYear = SystemTime.Current.Year;
            var currentCentury = currentYear - currentYear % 100;
            var nationalIdentifierYear = int.Parse(nationalIdentifierWhereHyphenAndPlusHasBeenStripped.Substring(0, 2));
            var yearhWithCentury = currentCentury + nationalIdentifierYear;

            if (yearhWithCentury > currentYear)
                yearhWithCentury -= 100; // Assumes person day of birth is less than 100

            if (hasPlusSign)
                yearhWithCentury -=
                    100; // Take current year and subtract hundred since the plus sign indicates the person is over a hundred years old.

            return yearhWithCentury;
        }

        private static bool HasValidControlDigit(string valueToCheck)
        {
            // Check that the control digit is correct, valueToCheck should have the following format YYMMDDNNNC
            var nationalIdentifier = long.Parse(valueToCheck);

            // a9a8 a7a6 a5a4 a3a2a1a0;  a9-a8 = year, a7a6 = month, a5a4= day, a3a2a1 = semi-random number,  a0 = Control digit
            var controlDigit = 0L;
            for (var i = 1; i < 10; ++i)
            {
                var multiplier = i % 2 == 1 ? 2 : 1;
                var digit = GetDigitAtPosition(nationalIdentifier, i) * multiplier;
                controlDigit += GetDigitAtPosition(digit, 1) + GetDigitAtPosition(digit, 0);
            }

            controlDigit = (10 - controlDigit % 10) % 10;
            return controlDigit == GetDigitAtPosition(nationalIdentifier, 0);
        }
    }
}