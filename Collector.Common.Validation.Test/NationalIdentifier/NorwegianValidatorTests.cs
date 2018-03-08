using System;
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
            _sut = new NorwegianNationalIdentifierValidator();
        }

        [TestCase("311299-56715")]
        [TestCase("31129956715")]
        [TestCase("290232-20197")]  // 1932 has leapyear
        [TestCase("290204-50051")] // 2004 has leapyear
        [TestCase("410184-12339")]  // Valid temporary number with 01 days
        [TestCase("710184-12351")]  // Valid temporary number with 31 days
        [TestCase("690204-50045")]  // Valid temporary number with leapyear
        //    500–749 = If YY >= 54 then 1854–1899 , if YY <= 39 then 2000-2039. YY > 39 and YY< 54 then undefined
        [TestCase("010140-49920")]    // 010140-499 CC   // Valid for serialnumber 499 for 500 it would be invalid
        [TestCase("010153-49901")]    // 010153-499 CC  // Valid for serialnumber 499 for 500 it would be invalid
        //    750-899 = If YY <= 39 then 2000-2039 otherwise undefined
        [TestCase("010154-74943")]    // 010154-749 CC   // Valid for serialnumber 749 for 750 it would be invalid
        [TestCase("010199-74940")]    // 010199-749 CC   // Valid for serialnumber 749 for 750 it would be invalid
        [TestCase("010140-90017")]    // 010140-900 CC   // Valid for serialnumber 900 for 899 it would be invalid
        [TestCase("010199-90016")]    // 010199-900 CC   // Valid for serialnumber 900 for 899 it would be invalid
        public void IsValid_NationalIdentifierIsValid_ReturnTrue(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.True);
        }

        [TestCase(31129956715)]
        [TestCase(29023220197)]
        public void IsValid_NationalIdentifierAsInt_ReturnTrue(long nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.True);
        }


        [TestCase(null)]
        [TestCase("xx")] // Invalid
        [TestCase("311280=88566")]  // Invalid Format
        [TestCase("190033-20076")] // Invalid month
        [TestCase("191333-20094")] // Invalid month
        [TestCase("890133-20072")] // Invalid day
        [TestCase("990133-20043")] // Invalid day
        [TestCase("290233-20027")] // 1933 does not have leapyear
        [TestCase("290205-50072")] // 2005 does not have leapyear
        [TestCase("311280-30187")] // Control number not correct
        //    500–749 = If YY >= 54 then 1854–1899 , if YY <= 39 then 2000-2039. YY > 39 and YY < 54 then undefined
        [TestCase("010140-50066")]    // Invalid serial number   010140-500 CC   - control digits should be valid
        [TestCase("010153-50047")]    // Invalid serial number   010153-500 CC   - control digits should be valid
        [TestCase("010140-60029")]    // Invalid serial number   010140-600 CC   - control digits should be valid
        [TestCase("020153-60049")]    // Invalid serial number   010153-600 CC   - control digits should be valid
        [TestCase("010140-74941")]    // Invalid serial number   010140-749 CC   - control digits should be valid
        [TestCase("010153-74922")]    // Invalid serial number   010153-749 CC   - control digits should be valid
        //    750-899 = If YY <= 39 then 2000-2039 otherwise undefined
        [TestCase("010140-75069")]    // Invalid serial number   010140-750 CC   - control digits should be valid
        [TestCase("010170-75043")]    // Invalid serial number   010170-750 CC   - control digits should be valid
        [TestCase("010199-75068")]    // Invalid serial number   010199-750 CC   - control digits should be valid
        [TestCase("010140-89981")]    // Invalid serial number   010140-899 CC   - control digits should be valid
        [TestCase("010170-89966")]    // Invalid serial number   010170-899 CC   - control digits should be valid
        [TestCase("010199-89980")]    // Invalid serial number   010199-899 CC   - control digits should be valid
        public void IsValid_NationalIdentifierIsInvalid_ReturnFalse(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.False);
        }

        [TestCase(19133320094)]
        [TestCase(89013320072)] 
        public void IsValid_NationalIdentifierAsInt_ReturnFalse(long nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.False);
        }


        [TestCase("010122-00028", "01012200028")]
        [TestCase("29020450051", "29020450051")] // 2004 has leapyear
        [TestCase("410184-12339", "41018412339")]
        [TestCase("71018412351", "71018412351")]
        //    500–749 = If YY >= 54 then 1854–1899 , if YY <= 39 then 2000-2039. YY > 39 and YY< 54 then undefined
        [TestCase("010140-49920", "01014049920")]
        //    750-899 = If YY <= 39 then 2000-2039 otherwise undefined
        [TestCase("010154-74943", "01015474943")]    // 010154-749 CC   // Valid for serialnumber 749 for 750 it would be invalid
        public void Normalize_NationalIdentifierIsValid_ReturnNormalizedValue(string nationalIdentifier, string expectedValue)
        {
            var actual = _sut.Normalize(nationalIdentifier);

            Assert.That(actual, Is.EqualTo(expectedValue));
        }


        [TestCase(null)]
        [TestCase("xx")] // Invalid
        [TestCase("311280=88566")]  // Invalid Format
        [TestCase("190033-20076")] // Invalid month
        [TestCase("890133-20072")] // Invalid day
        [TestCase("290205-50072")] // 2005 does not have leapyear
        [TestCase("311280-30187")] // Control number not correct
        //    500–749 = If YY >= 54 then 1854–1899 , if YY <= 39 then 2000-2039. YY > 39 and YY < 54 then undefined
        [TestCase("010140-50066")]    // Invalid serial number   010140-500 CC   - control digits should be valid
        [TestCase("010153-74922")]    // Invalid serial number   010153-749 CC   - control digits should be valid
        //    750-899 = If YY <= 39 then 2000-2039 otherwise undefined
        [TestCase("010140-75069")]    // Invalid serial number   010140-750 CC   - control digits should be valid
        [TestCase("010199-89980")]    // Invalid serial number   010199-899 CC   - control digits should be valid
        public void Normalize_NationalIdentifierIsInvalid_ThrowArgumentException(string nationalIdentifier)
        {
            var ex = Assert.Throws<ArgumentException>(() => _sut.Normalize(nationalIdentifier));
            Assert.That(ex.ParamName, Is.EqualTo("nationalIdentifier"), nameof(ex.ParamName));
            Assert.That(ex.Message, Does.Contain(nationalIdentifier ?? string.Empty).And.Contain(_sut.NationalCountry.ToString()), nameof(ex.Message));
        }
    }
}