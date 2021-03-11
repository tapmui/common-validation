using System;
using System.Collections;
using Collector.Common.Validation.NationalIdentifier;
using Collector.Common.Validation.NationalIdentifier.Validators;
using NUnit.Framework;

namespace Collector.Common.Validation.Test.NationalIdentifier
{
    public class GermanValidatorTests
    {
        private DateTimeOffset                     _defaultTestTime;
        private GermanyNationalBirthdayIdentifierValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut             = new GermanyNationalBirthdayIdentifierValidator();
            _defaultTestTime = new DateTimeOffset(new DateTime(2010, 01, 01), TimeSpan.Zero);
        }

        [TestCaseSource(typeof(GermanIdentifiers), nameof(GermanIdentifiers.Valid))]
        public void IsValid_NationalIdentifierIsValid_ReturnTrue(string nationalIdentifier)
        {
            SystemTime.TestTime = _defaultTestTime;

            var actual = _sut.IsValid(nationalIdentifier);
            Assert.That(actual, Is.True);
            var parsedResult = _sut.Parse(nationalIdentifier);
            Assert.That(parsedResult.Valid, Is.True);
        }

        [TestCaseSource(typeof(GermanIdentifiers), nameof(GermanIdentifiers.Invalid))]
        public void IsValid_NationalIdentifierIsInvalid_ReturnFalse(string nationalIdentifier)
        {
            SystemTime.TestTime = _defaultTestTime;

            var actual = _sut.IsValid(nationalIdentifier);
            Assert.That(actual, Is.False);
            var parsedResult = _sut.Parse(nationalIdentifier);
            Assert.That(parsedResult.Valid, Is.False);
        }
        
        [TestCaseSource(typeof(GermanIdentifiers), nameof(GermanIdentifiers.Normalized))]
        public void Normalize_NationalIdentifierIsValid_ReturnNormalizedValue(
            string nationalIdentifier, string expectedValue)
        {
            SystemTime.TestTime = _defaultTestTime;

            var actual = _sut.Normalize(nationalIdentifier);

            Assert.That(actual, Is.EqualTo(expectedValue));
        }


        [TestCaseSource(typeof(GermanIdentifiers), nameof(GermanIdentifiers.Invalid))]
        public void Normalize_NationalIdentifierIsInvalid_ThrowArgumentException(string nationalIdentifier)
        {
            SystemTime.TestTime = _defaultTestTime;

            var ex = Assert.Throws<ArgumentException>(() => _sut.Normalize(nationalIdentifier));
            Assert.That(ex.ParamName, Is.EqualTo("nationalIdentifier"), nameof(ex.ParamName));
            Assert.That(ex.Message,
                Does.Contain(nationalIdentifier ?? string.Empty).And.Contain(_sut.CountryCode.ToString()),
                nameof(ex.Message));
        }

        public class GermanIdentifiers
        {
            public static IEnumerable Valid
            {
                get
                {
                    yield return "01.01.2010";
                    yield return "31.01.2010";
                    yield return "01.12.2010";
                    yield return "01.01.2010";
                    yield return "31.12.2010";
                    yield return "29.02.1932"; // 1932 has leapyear
                    yield return "29.02.2004";   // 2004 has leapyear
                }
            }

            public static IEnumerable Invalid
            {
                get
                {
                    yield return null;
                    yield return "xx";          // Invalid
                    yield return "311280"; // Invalid Format
                    yield return "33.00.1971"; // Invalid month
                    yield return "33.13.1919"; // Invalid month
                    yield return "33.13.19191"; // Invalid year
                    yield return "33.01.1949"; // Invalid day
                    yield return "00.01.1959"; // Invalid day
                    yield return "29.02.1933"; // 1933 does not have leapyear
                    yield return "29.02.2005"; // 2005 does not have leapyear
                    yield return "01.01,2010"; // Invalid Format
                }
            }

            public static IEnumerable Normalized
            {
                get
                {
                    yield return new TestCaseData("18.10.1964", "19641018");
                    yield return new TestCaseData("01.01.1964", "19640101");
                    yield return new TestCaseData("20.05.1980", "19800520");
                    yield return new TestCaseData("29.02.2004", "20040229"); // 2004 has leapyear
                }
            }
        }
    }
}