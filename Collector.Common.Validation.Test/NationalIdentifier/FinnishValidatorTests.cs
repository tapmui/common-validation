using System;
using Collector.Common.Validation.NationalIdentifier.Validators;
using NUnit.Framework;

namespace Collector.Common.Validation.Test.NationalIdentifier
{
    public class FinnishValidatorTests
    {
        private FinnishNationalIdentifierValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new FinnishNationalIdentifierValidator();
        }

        [TestCase("311280-888Y")]
        [TestCase("311280A888Y")]
        [TestCase("311280+888Y")]
        [TestCase("181280-888K")]
        [TestCase("181285-810B")]
        [TestCase("181285-8056")]
        [TestCase("290232-2001")] // 1932 has leapyear
        [TestCase("290204A500F")] // 2004 has leapyear
        public void IsValid_NationalIdentifierIsValid_ReturnTrue(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.True);
        }


        [TestCase(null)]
        [TestCase("xx")] // Invalid
        [TestCase("311280#8888")] // Invalid Format
        [TestCase("190033-2007")] // Invalid month
        [TestCase("191333-200P")] // Invalid month   Control Value: 22
        [TestCase("490133-200C")] // Invalid day
        [TestCase("590133-200U")] // Invalid day     Control Value: 26
        [TestCase("290233-2009")] // 1933 does not have leapyear
        [TestCase("290205A500R")] // 2005 does not have leapyear  Control Value: 23
        [TestCase("311280-888A")] // Control number not correct
        public void IsValid_NationalIdentifierIsInvalid_ReturnFalse(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.False);
        }


        [TestCase("311280-888Y", "311280-888Y")]
        [TestCase("311280A888Y", "311280A888Y")]
        [TestCase("311280+888Y", "311280+888Y")]
        [TestCase("290232-2001", "290232-2001")]
        [TestCase("290204A500F", "290204A500F")]
        public void Normalize_NationalIdentifierIsValid_ReturnNormalizedValue(string nationalIdentifier, string expectedValue)
        {
            var actual = _sut.Normalize(nationalIdentifier);

            Assert.That(actual, Is.EqualTo(expectedValue));
        }


        [TestCase(null)]
        [TestCase("xx")] // Invalid
        [TestCase("311280#8888")] // Invalid Format
        [TestCase("190033-2007")] // Invalid month
        [TestCase("490133-200C")] // Invalid day
        [TestCase("290233-2009")] // 1933 does not have leapyear
        [TestCase("311280-888A")] // Control number not correct
        public void Normalize_NationalIdentifierIsInvalid_ThrowArgumentException(string nationalIdentifier)
        {
            var ex = Assert.Throws<ArgumentException>(() => _sut.Normalize(nationalIdentifier));
            Assert.That(ex.ParamName, Is.EqualTo("nationalIdentifier"), nameof(ex.ParamName));
            Assert.That(ex.Message, Does.Contain(nationalIdentifier ?? string.Empty).And.Contain(_sut.NationalCountry.ToString()), nameof(ex.Message));
        }
    }
}