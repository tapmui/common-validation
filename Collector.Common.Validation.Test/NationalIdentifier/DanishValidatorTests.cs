using System;
using Collector.Common.Validation.NationalIdentifier.Validators;
using NUnit.Framework;

namespace Collector.Common.Validation.Test.NationalIdentifier
{
    public class DanishValidatorTests
    {
        private DanishNationalIdentifierValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new DanishNationalIdentifierValidator();
        }

        [TestCase("121043-2312")]
        [TestCase("1210432312")]
        [TestCase("310184-1232")]
        [TestCase("290232-2000")]  // 1932 has leapyear
        [TestCase("290204-5000")]  // 2004 has leapyear
        [TestCase("610184-1232")]  // Valid temporary number with 01 days
        [TestCase("910184-1232")]  // Valid temporary number with 31 days
        [TestCase("890204-5000")]  // Valid temporary number with leapyear
        public void IsValid_NationalIdentifierIsValid_ReturnTrue(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.True);
        }


        [TestCase(null)]
        [TestCase("xx")] // Invalid
        [TestCase("310184#1232")] // Invalid Format
        [TestCase("190033-2000")]  // Invalid month
        [TestCase("191333-2000")]  // Invalid month
        [TestCase("490133-2000")]  // Invalid day
        [TestCase("590133-2000")]  // Invalid day
        [TestCase("290233-2000")]  // 1933 does not have leapyear
        [TestCase("290205-5000")]  // 2005 does not have leapyear
        public void IsValid_NationalIdentifierIsInvalid_ReturnFalse(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.False);
        }


        [TestCase("121043-2312", "1210432312")]
        [TestCase("1210432312", "1210432312")]
        [TestCase("610184-1232", "6101841232")]
        [TestCase("9101841232", "9101841232")]
        public void Normalize_NationalIdentifierIsValid_ReturnNormalizedValue(string nationalIdentifier, string expectedValue)
        {
            var actual = _sut.Normalize(nationalIdentifier);

            Assert.That(actual, Is.EqualTo(expectedValue));
        }


        [TestCase(null)]
        [TestCase("xx")] // Invalid
        [TestCase("310184#1232")] // Invalid Format
        [TestCase("190033-2000")]  // Invalid month
        [TestCase("490133-2000")]  // Invalid day
        [TestCase("290205-5000")]  // 2005 does not have leapyear
        public void Normalize_NationalIdentifierIsInvalid_ThrowArgumentException(string nationalIdentifier)
        {

            var ex = Assert.Throws<ArgumentException>(() => _sut.Normalize(nationalIdentifier));
            Assert.That(ex.ParamName, Is.EqualTo("nationalIdentifier"), nameof(ex.ParamName));
            Assert.That(ex.Message, Does.Contain(nationalIdentifier ?? string.Empty).And.Contain(_sut.NationalCountry.ToString()), nameof(ex.Message));
        }
    }
}
