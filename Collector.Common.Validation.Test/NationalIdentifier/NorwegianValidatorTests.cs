using System;
using System.Collections;
using Collector.Common.Validation.NationalIdentifier;
using Collector.Common.Validation.NationalIdentifier.Validators;
using NUnit.Framework;

namespace Collector.Common.Validation.Test.NationalIdentifier
{
    public class NorwegianValidatorTests
    {
        private NorwegianNationalIdentifierValidator _sut;

        [SetUp]
        public void SetUp()
        {
            SystemTime.TestTime = new DateTimeOffset(new DateTime(2010, 01, 01), TimeSpan.Zero);
            _sut = new NorwegianNationalIdentifierValidator();
        }

        [TestCaseSource(typeof(NorwegianIdentifiers), nameof(NorwegianIdentifiers.Valid))]
        public void IsValid_NationalIdentifierIsValid_ReturnTrue(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);
            Assert.That(actual, Is.True);
            var parsedResult = _sut.Parse(nationalIdentifier);
            Assert.That(parsedResult.Valid, Is.True);
        }

        [TestCaseSource(typeof(NorwegianIdentifiers), nameof(NorwegianIdentifiers.Invalid))]
        public void IsValid_NationalIdentifierIsInvalid_ReturnFalse(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);
            Assert.That(actual, Is.False);
            var parsedResult = _sut.Parse(nationalIdentifier);
            Assert.That(parsedResult.Valid, Is.False);
        }

        [TestCaseSource(typeof(NorwegianIdentifiers), nameof(NorwegianIdentifiers.Normalized))]
        public void Normalize_NationalIdentifierIsValid_ReturnNormalizedValue(
            string nationalIdentifier, string expectedValue)
        {
            var actual = _sut.Normalize(nationalIdentifier);

            Assert.That(actual, Is.EqualTo(expectedValue));
        }

        [TestCaseSource(typeof(NorwegianIdentifiers), nameof(NorwegianIdentifiers.Invalid))]
        public void Normalize_NationalIdentifierIsInvalid_ThrowArgumentException(string nationalIdentifier)
        {
            var ex = Assert.Throws<ArgumentException>(() => _sut.Normalize(nationalIdentifier));
            Assert.That(ex.ParamName, Is.EqualTo("nationalIdentifier"), nameof(ex.ParamName));
            Assert.That(ex.Message,
                Does.Contain(nationalIdentifier ?? string.Empty).And.Contain(_sut.CountryCode.ToString()),
                nameof(ex.Message));
        }

        public class NorwegianIdentifiers
        {
            public static IEnumerable Valid
            {
                get
                {
                    yield return "311299-56715";
                    yield return "31129956715";
                    yield return "290232-20197"; // 1932 has leapyear
                    yield return "290204-50051"; // 2004 has leapyear
                    yield return "410184-12339"; // Valid temporary number with 01 days
                    yield return "710184-12351"; // Valid temporary number with 31 days
                    yield return "690204-50045"; // Valid temporary number with leapyear
                    //    500–749 = If YY >= 54 then 1854–1899 , if YY <= 39 then 2000-2039. YY > 39 and YY< 54 then undefined
                    yield return "010140-49920"; // 010140-499 CC   // Valid for serialnumber 499 for 500 it would be invalid
                    yield return "010153-49901"; // 010153-499 CC  // Valid for serialnumber 499 for 500 it would be invalid
                    //    750-899 = If YY <= 39 then 2000-2039 otherwise undefined
                    yield return "010154-74943"; // 010154-749 CC   // Valid for serialnumber 749 for 750 it would be invalid
                    yield return "010199-74940"; // 010199-749 CC   // Valid for serialnumber 749 for 750 it would be invalid
                    yield return "010140-90017"; // 010140-900 CC   // Valid for serialnumber 900 for 899 it would be invalid
                    yield return "010199-90016"; // 010199-900 CC   // Valid for serialnumber 900 for 899 it would be invalid
                }
            }

            public static IEnumerable Invalid
            {
                get
                {
                    yield return null;
                    yield return "xx";
                    yield return ""; // Invalid
                    yield return "311280=88566"; // Invalid Format
                    yield return "190033-20076"; // Invalid month
                    yield return "191333-20094"; // Invalid month
                    yield return "890133-20072"; // Invalid day
                    yield return "990133-20043"; // Invalid day
                    yield return "290233-20027"; // 1933 does not have leapyear
                    yield return "290205-50072"; // 2005 does not have leapyear
                    yield return "311280-30187"; // Control number not correct
                    //    500–749 = If YY >= 54 then 1854–1899 , if YY <= 39 then 2000-2039. YY > 39 and YY < 54 then undefined
                    yield return "010140-50066"; // Invalid serial number   010140-500 CC   - control digits should be valid
                    yield return "010153-50047"; // Invalid serial number   010153-500 CC   - control digits should be valid
                    yield return "010140-60029"; // Invalid serial number   010140-600 CC   - control digits should be valid
                    yield return "020153-60049"; // Invalid serial number   010153-600 CC   - control digits should be valid
                    yield return "010140-74941"; // Invalid serial number   010140-749 CC   - control digits should be valid
                    yield return "010153-74922"; // Invalid serial number   010153-749 CC   - control digits should be valid
                    //    750-899 = If YY <= 39 then 2000-2039 otherwise undefined
                    yield return "010140-75069"; // Invalid serial number   010140-750 CC   - control digits should be valid
                    yield return "010170-75043"; // Invalid serial number   010170-750 CC   - control digits should be valid
                    yield return "010199-75068"; // Invalid serial number   010199-750 CC   - control digits should be valid
                    yield return "010140-89981"; // Invalid serial number   010140-899 CC   - control digits should be valid
                    yield return "010170-89966"; // Invalid serial number   010170-899 CC   - control digits should be valid
                    yield return "010199-89980"; // Invalid serial number   010199-899 CC   - control digits should be valid
                }
            }

            public static IEnumerable Normalized
            {
                get
                {
                    yield return new TestCaseData("010122-00028", "01012200028");
                    yield return new TestCaseData("29020450051", "29020450051"); // 2004 has leapyear
                    yield return new TestCaseData("410184-12339", "41018412339");
                    yield return new TestCaseData("71018412351", "71018412351");
                        //    500–749 = If YY >= 54 then 1854–1899 , if YY <= 39 then 2000-2039. YY > 39 and YY< 54 then undefined
                    yield return new TestCaseData("010140-49920", "01014049920");
                        //    750-899 = If YY <= 39 then 2000-2039 otherwise undefined
                    yield return new TestCaseData("010154-74943", "01015474943");
                    // 010154-749 CC   // Valid for serialnumber 749 for 750 it would be invalid
                }
            }
        }
    }
}