using System;
using System.Collections;
using Collector.Common.Validation.NationalIdentifier;
using Collector.Common.Validation.NationalIdentifier.Validators;
using NUnit.Framework;

namespace Collector.Common.Validation.Test.NationalIdentifier
{
    public class NationalIdentifierValidatorTests
    {
        [SetUp]
        public void SetUp()
        {
            SystemTime.TestTime = new DateTimeOffset(new DateTime(2010, 01, 01), TimeSpan.Zero);
        }

        [TestCase(CountryCode.DK, typeof(DanishNationalIdentifierValidator))]
        [TestCase(CountryCode.FI, typeof(FinnishNationalIdentifierValidator))]
        [TestCase(CountryCode.NO, typeof(NorwegianNationalIdentifierValidator))]
        [TestCase(CountryCode.SE, typeof(SwedishNationalIdentifierValidator))]
        public void GetValidator_Country_ReturnsCorrectValidator(CountryCode countryCode, Type type)
        {
            var result = NationalIdentifierValidator.GetValidator(countryCode);
            Assert.That(result, Is.AssignableFrom(type));
        }

        [TestCaseSource(typeof(DanishValidatorTests.DanishIdentifiers), nameof(DanishValidatorTests.DanishIdentifiers.Valid))]
        [TestCaseSource(typeof(FinnishValidatorTests.FinnishIdentifiers), nameof(FinnishValidatorTests.FinnishIdentifiers.Valid))]
        [TestCaseSource(typeof(NorwegianValidatorTests.NorwegianIdentifiers), nameof(NorwegianValidatorTests.NorwegianIdentifiers.Valid))]
        [TestCaseSource(typeof(SwedishValidatorTests.SwedishIdentifiers), nameof(SwedishValidatorTests.SwedishIdentifiers.Valid))]
        public void IsValidInAnyCountry_ValidNationalIdentifier_ReturnsTrue(string nationalIdentifier)
        {
            var result = NationalIdentifierValidator.IsValidInAnyCountry(nationalIdentifier);
            Assert.That(result, Is.True);
        }

        [TestCaseSource(typeof(InvalidNumbersForAnyCountry))]
        public void IsValidInAnyCountry_InvalidNationalIdentifier_ReturnsFalse(string nationalIdentifier)
        {
            var result = NationalIdentifierValidator.IsValidInAnyCountry(nationalIdentifier);
            Assert.That(result, Is.False);
        }

        [TestCaseSource(typeof(DanishValidatorTests.DanishIdentifiers),
            nameof(DanishValidatorTests.DanishIdentifiers.Normalized))]
        [TestCaseSource(typeof(FinnishValidatorTests.FinnishIdentifiers),
            nameof(FinnishValidatorTests.FinnishIdentifiers.Normalized))]
        [TestCaseSource(typeof(NorwegianValidatorTests.NorwegianIdentifiers),
            nameof(NorwegianValidatorTests.NorwegianIdentifiers.Normalized))]
        [TestCaseSource(typeof(SwedishValidatorTests.SwedishIdentifiers),
            nameof(SwedishValidatorTests.SwedishIdentifiers.Normalized))]
        public void NormalizeForAnyCountry_ValidNationalIdentifier_ReturnsNormalizedValue(
            string nationalIdentifier, string expected)
        {
            var result = NationalIdentifierValidator.NormalizeForAnyCountry(nationalIdentifier);
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(typeof(InvalidNumbersForAnyCountry))]
        public void NormalizeForAnyCountry_InvalidNationalIdentifier_ThrowsArgumentException(string nationalIdentifier)
        {
            var ex = Assert.Throws<ArgumentException>(() => NationalIdentifierValidator.NormalizeForAnyCountry(nationalIdentifier));
            Assert.That(ex.ParamName, Is.EqualTo("nationalIdentifier"), nameof(ex.ParamName));
            Assert.That(ex.Message,
                Does.Contain(nationalIdentifier ?? string.Empty),
                nameof(ex.Message));
        }

        public class InvalidNumbersForAnyCountry : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return null;
                yield return "";
                yield return "xx";
                yield return "310184#1232";
                yield return "190033-2000";
                yield return "191333-2000";
                yield return "290233-2009";
                yield return "290205A500R";
                yield return "311280-888A";
                yield return "010140-89981";
                yield return "010170-89966";
                yield return "010199-89980";
                yield return "330229-7316";
                yield return "330159-2048";
                yield return "581023-9574";
            }
        }
    }
}