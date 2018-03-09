using System;
using System.Text.RegularExpressions;

namespace Collector.Common.Validation.NationalIdentifier.Validators
{
    /// <remarks>
    /// Allowed formats: DDMMYYZZZQQ or DDMMYY-ZZZQQ
    ///    where DD = day, MM = month, YY = year, ZZZ = serial number, QQ = control digits
    /// Century of the birthdate is determined by the serial number
    ///     000–499 = 1900–1999
    ///     500–749 = If YY &gt;= 54 then 1854–1899 , if YY &lt;= 39 then 2000-2039. YY &gt; 39 and YY &lt; 54 then undefined
    ///     750-899 = If YY &lt;= 39 then 2000-2039 otherwise undefined
    ///     900-999 = If YY &gt;= 40 then 1940-1999, if YY &lt;= 39 then 2000-2039
    /// For temporary National Identifier 4 is added at the beginning i.e. 40 is added to the day so 01 becomes 41 and 30 becomes 70.
    /// </remarks>
    public class NorwegianNationalIdentifierValidator : NationalIdentifierValidator
    {
        /// <remarks>Regex has been check with SDL Regex Fuzzer tool so it should not be a source for DOS attacks. IF YOU UPDATE THE REGEX recheck it again with the SDL tool.</remarks>>
        private static readonly Regex NationalIdentifierWhitelistValidator =
            new Regex(@"^([0-3]\d[0-1]\d{3}\-?\d{5})$", RegexOptions.ECMAScript);

        /// <remarks>Regex has been check with SDL Regex Fuzzer tool so it should not be a source for DOS attacks. IF YOU UPDATE THE REGEX recheck it again with the SDL tool.</remarks>>
        private static readonly Regex TemporaryNationalIdentifierWhitelistValidator =
            new Regex(@"^([4-7]\d[0-1]\d{3}\-?\d{5})$", RegexOptions.ECMAScript);

        /// <remarks>The two first element are set to zero for padding so index don't need to be adjusted when doing calculations.</remarks>>
        private static readonly int[] SumOneFactors = {0, 1, 2, 5, 4, 9, 8, 1, 6, 7, 3};

        /// <remarks>The first element are set to zero for padding so index don't need to be adjusted when doing calculations.</remarks>>
        private static readonly int[] SumTwoFactors = {1, 2, 3, 4, 5, 6, 7, 2, 3, 4, 5};

        public override NationalIdentifierCountry Country => NationalIdentifierCountry.Norway;

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

            // valueToCheck should have format: DDMMYYZZZQQ
            var valueToCheck = nationalIdentifier.Replace("-", string.Empty);
            var year = int.Parse(valueToCheck.Substring(4, 2));
            var serialNumber = int.Parse(valueToCheck.Substring(6, 3));
            var yearhWithCentuary = ExtractYearhWithCentury(year, serialNumber);
            if (yearhWithCentuary < 0)
                return false;

            var month = int.Parse(valueToCheck.Substring(2, 2));
            var day = int.Parse(valueToCheck.Substring(0, 2));
            if (isTemporary)
                day -= 40; // 40 is added to the day of temporary numbers so subtract away it

            return IsValidDate(yearhWithCentuary, month, day) && HasValidControlDigits(valueToCheck);
        }

        public override string Normalize(string nationalIdentifier)
        {
            if (!IsValid(nationalIdentifier))
                throw new ArgumentException(ErrorMessages.GetInvalidIdentifierMessage(nationalIdentifier, Country),
                    nameof(nationalIdentifier));

            return nationalIdentifier.Replace("-", "");
        }

        private static int ExtractYearhWithCentury(int year, int serialNumber)
        {
            /*
            * 000–499 omfatter personer født i perioden 1900–1999.
            * 500–749 omfatter personer født i perioden 1854–1899.
            * 500–999 omfatter personer født i perioden 2000–2039.
            * 900–999 omfatter personer født i perioden 1940–1999.
            * 
            *    000–499 = 1900–1999
            *    500–749 = If YY >= 54 then 1854–1899 , if YY <= 39 then 2000-2039. YY > 39 and YY < 54 then undefined
            *    750-899 = If YY <= 39 then 2000-2039 otherwise undefined
            *    900-999 = If YY >= 40 then 1940-1999, if YY <= 39 then 2000-2039
            */
            int century;
            if (serialNumber <= 499 || (serialNumber >= 900 && serialNumber <= 999 && year >= 40))
            {
                century = 1900;
            }
            else if (serialNumber >= 500 && serialNumber <= 999 && year <= 39)
            {
                century = 2000;
            }
            else if (serialNumber >= 500 && serialNumber <= 749 && year >= 54)
            {
                century = 1800;
            }
            else
            {
                return -1;
            }

            return century + year;
        }


        // Check that the two control digits are correct
        // a10a9 a8a7 a6a5 a4a3a2 a1a0;
        // a10a9 = day, a8a7 = month, a6a5= year, a4a3a2 = semi-random number, a1 = First Control digit, a0 = Second Control digit
        private static bool HasValidControlDigits(string value)
        {
            var civicRegNumber = long.Parse(value);
            var firstControlDigitCalulcated = 0L;
            for (var i = 1; i < 11; ++i)
            {
                var digit = GetDigitAtPosition(civicRegNumber, i);
                firstControlDigitCalulcated += digit * SumOneFactors[i];
            }

            var secondControlDigitCalulcated = 0L;
            for (var i = 0; i < 11; ++i)
            {
                var digit = GetDigitAtPosition(civicRegNumber, i);
                secondControlDigitCalulcated += digit * SumTwoFactors[i];
            }

            return firstControlDigitCalulcated % 11 == 0 && secondControlDigitCalulcated % 11 == 0;
        }
    }
}