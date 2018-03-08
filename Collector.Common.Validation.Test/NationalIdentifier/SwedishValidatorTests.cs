using System;
using Collector.Common.Validation.NationalIdentifier;
using Collector.Common.Validation.NationalIdentifier.Validators;
using NUnit.Framework;

namespace Collector.Common.Validation.Test.NationalIdentifier
{
    public class SwedishValidatorTests
    {
        private DateTimeOffset _defaultTestTime;
        private SwedishNationalIdentifierValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new SwedishNationalIdentifierValidator();
            _defaultTestTime = new DateTimeOffset(new DateTime(2010, 01, 01), TimeSpan.Zero);
        }

        [TestCase("19640101-8780")]
        [TestCase("1964-01-01-8780")]
        [TestCase("196401-01-8780")]
        [TestCase("196401018780")]
        [TestCase("19800520-2216")]
        [TestCase("6401018780")]
        [TestCase("640101-8780")]
        [TestCase("800520+2216")]
        [TestCase("320229-4173")]     // 1932 has leapyear
        [TestCase("19320229-4173")]  // 1932 has leapyear
        [TestCase("040222-2269")]    // 2004 has leapyear
        [TestCase("20040222-2269")]  // 2004 has leapyear
        [TestCase("840161-2729")]   // Valid temporary number with 01 days
        [TestCase("840191-0313")]   // Valid temporary number with 31 days
        [TestCase("040289-5049")]   // Valid temporary number with leapyear
        public void IsValid_NationalIdentifierIsValid_ReturnTrue(string nationalIdentifier)
        {
            SystemTime.TestTime = _defaultTestTime;

            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.True);
        }

        [TestCase(196401018780)]
        [TestCase(198005202216)]
        public void IsValid_NationalIdentifierAsInt_ReturnTrue(long nationalIdentifier)
        {
            SystemTime.TestTime = _defaultTestTime;

            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.True);
        }


        [TestCase(null)]
        [TestCase("xx")] // Invalid
        [TestCase("311280=8856")]  // Invalid Format
        [TestCase("330019-2071")]  // Invalid month
        [TestCase("331319-2092")]  // Invalid month
        [TestCase("330149-2074")]  // Invalid day
        [TestCase("330159-2048")]  // Invalid day
        [TestCase("330229-7316")] // 1933 does not have leapyear
        [TestCase("050229-5009")] // 2005 does not have leapyear
        [TestCase("581023-9574")] // Control number not correct
        public void IsValid_NationalIdentifierIsInvalid_ReturnFalse(string nationalIdentifier)
        {
            SystemTime.TestTime = _defaultTestTime;

            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.False);
        }

        [TestCase(3313192092)]
        [TestCase(3301492074)]
        public void IsValid_NationalIdentifierAsInt_ReturnFalse(long nationalIdentifier)
        {
            SystemTime.TestTime = _defaultTestTime;

            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.False);
        }


        [TestCase("196401018780", "196401018780")]
        [TestCase("64-01-01-8780", "196401018780")]
        [TestCase("19800520-2216", "198005202216")]
        [TestCase("800520-2216", "198005202216")]
        [TestCase("010520-2212", "200105202212")]
        [TestCase("030520+2210", "190305202210")]
        [TestCase("091230-7808", "200912307808")]
        [TestCase("100102-4841", "201001024841")]
        [TestCase("100102+4841", "191001024841")]
        [TestCase("040222-2269", "200402222269")]    // 2004 has leapyear
        [TestCase("840161-2729", "198401612729")]    // Valid temporary number with 01 days
        public void Normalize_NationalIdentifierIsValid_ReturnNormalizedValue(string nationalIdentifier, string expectedValue)
        {
            SystemTime.TestTime = _defaultTestTime;

            var actual = _sut.Normalize(nationalIdentifier);

            Assert.That(actual, Is.EqualTo(expectedValue));
        }


        [TestCase(null)]
        [TestCase("xx")] // Invalid
        [TestCase("100102=4841")] // Invalid format
        [TestCase("330149-2074")]  // Invalid day
        [TestCase("330019-2070")]  // Invalid month
        [TestCase("050229-5009")] // 2005 does not have leapyear
        [TestCase("581023-9574")] // Control number not correct
        public void Normalize_NationalIdentifierIsInvalid_ThrowArgumentException(string nationalIdentifier)
        {
            SystemTime.TestTime = _defaultTestTime;

            var ex = Assert.Throws<ArgumentException>(() => _sut.Normalize(nationalIdentifier));
            Assert.That(ex.ParamName, Is.EqualTo("nationalIdentifier"), nameof(ex.ParamName));
            Assert.That(ex.Message, Does.Contain(nationalIdentifier ?? string.Empty).And.Contain(_sut.NationalCountry.ToString()), nameof(ex.Message));
        }
    }
}