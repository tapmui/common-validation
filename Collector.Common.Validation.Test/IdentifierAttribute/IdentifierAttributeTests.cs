using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Collector.Common.Validation.NationalIdentifier;
using Collector.Common.Validation.NationalIdentifier.Attribute;
using Collector.Common.Validation.Test.NationalIdentifier;
using NUnit.Framework;

namespace Collector.Common.Validation.Test.IdentifierAttribute
{
    public class IdentifierAttributeTests
    {
        [TestCaseSource(typeof(DanishValidatorTests.DanishIdentifiers),
            nameof(DanishValidatorTests.DanishIdentifiers.Valid))]
        [TestCaseSource(typeof(FinnishValidatorTests.FinnishIdentifiers),
            nameof(FinnishValidatorTests.FinnishIdentifiers.Valid))]
        [TestCaseSource(typeof(NorwegianValidatorTests.NorwegianIdentifiers),
            nameof(NorwegianValidatorTests.NorwegianIdentifiers.Valid))]
        [TestCaseSource(typeof(SwedishValidatorTests.SwedishIdentifiers),
            nameof(SwedishValidatorTests.SwedishIdentifiers.Valid))]
        public void IsValid_ValidIdentifier_ReturnsTrue(string nationalIdentifier)
        {
            var validationResults = Validate(nationalIdentifier);
            Assert.That(validationResults.Count, Is.EqualTo(0));
        }

        [TestCaseSource(typeof(NationalIdentifierValidatorTests.InvalidNumbersForAnyCountry))]
        public void IsValid_InvalidIdentifier_ReturnsFalse(string nationalIdentifier)
        {
            var result = Validate(nationalIdentifier);
            Assert.That(result.Count, Is.GreaterThan(0));
        }

        [TestCaseSource(typeof(AttributeTestCases), nameof(AttributeTestCases.Valid))]
        public void IsValidSingleCountry_ValidIdentifier_ReturnsTrue(string nationalIdentifier, CountryCode countryCode)
        {
            var result = Validate(nationalIdentifier, countryCode);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [TestCaseSource(typeof(AttributeTestCases), nameof(AttributeTestCases.Invalid))]
        public void IsValidSignleCountry_InvalidIdentifier_ReturnsFalse(string nationalIdentifier, CountryCode countryCode)
        {
            var result = Validate(nationalIdentifier, countryCode);
            Assert.That(result.Count, Is.GreaterThan(0));
        }

        private static List<ValidationResult> Validate(string nationalIdentifier, CountryCode countryCode)
        {
            switch (countryCode)
            {
                case CountryCode.SE:
                    return Validate(new IdentifierPropertyCarrierSe{NationalIdentifier = nationalIdentifier});
                case CountryCode.NO:
                    return Validate(new IdentifierPropertyCarrierNo{NationalIdentifier = nationalIdentifier});
                case CountryCode.FI:
                    return Validate(new IdentifierPropertyCarrierFi{NationalIdentifier = nationalIdentifier});
                case CountryCode.DK:
                    return Validate(new IdentifierPropertyCarrierDk{NationalIdentifier = nationalIdentifier});
                default:
                    throw new ArgumentOutOfRangeException(nameof(countryCode), countryCode, null);
            }
        }

        private static List<ValidationResult> Validate(string nationalIdentifier)
        {
            var propertyCarrier = new IdentifierPropertyCarrier { NationalIdentifier = nationalIdentifier };
            return Validate(propertyCarrier);
        }

        private static List<ValidationResult> Validate(object propertyCarrier)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(propertyCarrier, null, null);
            Validator.TryValidateObject(propertyCarrier, ctx, validationResults, true);
            return validationResults;
        }

        public class AttributeTestCases
        {
            public static IEnumerable Valid
            {
                get
                {
                    foreach (var identifier in DanishValidatorTests.DanishIdentifiers.Valid)
                        yield return new TestCaseData(identifier, CountryCode.DK);
                    foreach (var identifier in FinnishValidatorTests.FinnishIdentifiers.Valid)
                        yield return new TestCaseData(identifier, CountryCode.FI);
                    foreach (var identifier in NorwegianValidatorTests.NorwegianIdentifiers.Valid)
                        yield return new TestCaseData(identifier, CountryCode.NO);
                    foreach (var identifier in SwedishValidatorTests.SwedishIdentifiers.Valid)
                        yield return new TestCaseData(identifier, CountryCode.SE);
                }
            }

            public static IEnumerable Invalid
            {
                get
                {
                    foreach (var identifier in DanishValidatorTests.DanishIdentifiers.Invalid)
                        yield return new TestCaseData(identifier, CountryCode.DK);
                    foreach (var identifier in FinnishValidatorTests.FinnishIdentifiers.Invalid)
                        yield return new TestCaseData(identifier, CountryCode.FI);
                    foreach (var identifier in NorwegianValidatorTests.NorwegianIdentifiers.Invalid)
                        yield return new TestCaseData(identifier, CountryCode.NO);
                    foreach (var identifier in SwedishValidatorTests.SwedishIdentifiers.Invalid)
                        yield return new TestCaseData(identifier, CountryCode.SE);
                }
            }
        }

        private class IdentifierPropertyCarrier
        {
            [NationalIdentifier]
            public string NationalIdentifier { get; set; }
        }

        private class IdentifierPropertyCarrierDk
        {
            [NationalIdentifier(CountryCode.DK)]
            public string NationalIdentifier { get; set; }
        }

        private class IdentifierPropertyCarrierFi
        {
            [NationalIdentifier(CountryCode.FI)]
            public string NationalIdentifier { get; set; }
        }

        private class IdentifierPropertyCarrierNo
        {
            [NationalIdentifier(CountryCode.NO)]
            public string NationalIdentifier { get; set; }
        }

        private class IdentifierPropertyCarrierSe
        {
            [NationalIdentifier(CountryCode.SE)]
            public string NationalIdentifier { get; set; }
        }
        
    }
}