using System;
using System.Collections;
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

        [TestCaseSource(typeof(DanishIdentifiers), nameof(DanishIdentifiers.Valid))]
        public void IsValid_NationalIdentifierIsValid_ReturnTrue(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.True);
        }

        [TestCaseSource(typeof(DanishIdentifiers), nameof(DanishIdentifiers.Invalid))]
        public void IsValid_NationalIdentifierIsInvalid_ReturnFalse(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.False);
        }
        
        [TestCaseSource(typeof(DanishIdentifiers), nameof(DanishIdentifiers.Normalized))]
        public void Normalize_NationalIdentifierIsValid_ReturnNormalizedValue(
            string nationalIdentifier, string expectedValue)
        {
            var actual = _sut.Normalize(nationalIdentifier);

            Assert.That(actual, Is.EqualTo(expectedValue));
        }


        [TestCaseSource(typeof(DanishIdentifiers), nameof(DanishIdentifiers.Invalid))]
        public void Normalize_NationalIdentifierIsInvalid_ThrowArgumentException(string nationalIdentifier)
        {
            var ex = Assert.Throws<ArgumentException>(() => _sut.Normalize(nationalIdentifier));
            Assert.That(ex.ParamName, Is.EqualTo("nationalIdentifier"), nameof(ex.ParamName));
            Assert.That(ex.Message,
                Does.Contain(nationalIdentifier ?? string.Empty).And.Contain(_sut.Country.ToString()),
                nameof(ex.Message));
        }

        public class DanishIdentifiers
        {
            public static IEnumerable Valid
            {
                get
                {
                    yield return "121043-2312";
                    yield return "1210432312";
                    yield return "310184-1232";
                    yield return "290232-2000"; // 1932 has leapyear
                    yield return "290204-5000"; // 2004 has leapyear
                    yield return "610184-1232"; // Valid temporary number with 01 days
                    yield return "910184-1232"; // Valid temporary number with 31 days
                    yield return "890204-5000"; // Valid temporary number with leapyear
                }
            }

            public static IEnumerable Invalid
            {
                get
                {
                    yield return null;
                    yield return "xx";          // Invalid
                    yield return "310184#1232"; // Invalid Format
                    yield return "190033-2000"; // Invalid month
                    yield return "191333-2000"; // Invalid month
                    yield return "490133-2000"; // Invalid day
                    yield return "590133-2000"; // Invalid day
                    yield return "290233-2000"; // 1933 does not have leapyear
                    yield return "290205-5000"; // 2005 does not have leapyear
                }
            }

            public static IEnumerable Normalized
            {
                get
                {
                    yield return new TestCaseData("121043-2312", "1210432312");
                    yield return new TestCaseData("1210432312", "1210432312");
                    yield return new TestCaseData("610184-1232", "6101841232");
                    yield return new TestCaseData("9101841232", "9101841232");
                }
            }
        }
    }
}