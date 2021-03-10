﻿using System;
using System.Collections;
using Collector.Common.Validation.NationalIdentifier;
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
            SystemTime.TestTime = new DateTimeOffset(new DateTime(2010, 01, 01), TimeSpan.Zero);
            _sut = new FinnishNationalIdentifierValidator();
        }

        [TestCaseSource(typeof(FinnishIdentifiers), nameof(FinnishIdentifiers.Valid))]
        public void IsValid_NationalIdentifierIsValid_ReturnTrue(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);
			Assert.That(actual, Is.True);
            var parsedResult = _sut.Parse(nationalIdentifier);
            Assert.That(parsedResult.Valid, Is.True);
        }


        [TestCaseSource(typeof(FinnishIdentifiers), nameof(FinnishIdentifiers.Invalid))]
        public void IsValid_NationalIdentifierIsInvalid_ReturnFalse(string nationalIdentifier)
        {
            var actual = _sut.IsValid(nationalIdentifier);
            Assert.That(actual, Is.False);
            var parsedResult = _sut.Parse(nationalIdentifier);
            Assert.That(parsedResult.Valid, Is.False);
        }

        [TestCaseSource(typeof(FinnishIdentifiers), nameof(FinnishIdentifiers.Normalized))]
        public void Normalize_NationalIdentifierIsValid_ReturnNormalizedValue(
            string nationalIdentifier, string expectedValue)
        {
            var actual = _sut.Normalize(nationalIdentifier);

            Assert.That(actual, Is.EqualTo(expectedValue));
        }


        [TestCaseSource(typeof(FinnishIdentifiers), nameof(FinnishIdentifiers.Invalid))]
        public void Normalize_NationalIdentifierIsInvalid_ThrowArgumentException(string nationalIdentifier)
        {
            var ex = Assert.Throws<ArgumentException>(() => _sut.Normalize(nationalIdentifier));
            Assert.That(ex.ParamName, Is.EqualTo("nationalIdentifier"), nameof(ex.ParamName));
            Assert.That(ex.Message,
                Does.Contain(nationalIdentifier ?? string.Empty).And.Contain(_sut.CountryCode.ToString()),
                nameof(ex.Message));
        }

        public class FinnishIdentifiers
        {
            public static IEnumerable Valid
            {
                get
                {
                    yield return "311280-888Y";
                    yield return "311280A888Y";
                    yield return "311280+888Y";
                    yield return "181280-888K";
                    yield return "181285-810B";
                    yield return "181285-8056";
                    yield return "290232-2001"; // 1932 has leapyear
                    yield return "290204A500F"; // 2004 has leapyear
                }
            }

            public static IEnumerable Invalid
            {
                get
                {
                    yield return null;
                    yield return "xx";
                    yield return "311280#8888"; // Invalid Format
                    yield return "190033-2007"; // Invalid month
                    yield return "191333-200P"; // Invalid month   Control Value: 22
                    yield return "490133-200C"; // Invalid day
                    yield return "590133-200U"; // Invalid day     Control Value: 26
                    yield return "290233-2009"; // 1933 does not have leapyear
                    yield return "290205A500R"; // 2005 does not have leapyear  Control Value: 23
                    yield return "311280-888A"; // Control number not correct
                }
            }

            public static IEnumerable Normalized
            {
                get
                {
                    yield return new TestCaseData("311280-888Y", "311280-888Y");
                    yield return new TestCaseData("311280A888Y", "311280A888Y");
                    yield return new TestCaseData("311280+888Y", "311280+888Y");
                    yield return new TestCaseData("290232-2001", "290232-2001");
                    yield return new TestCaseData("290204A500F", "290204A500F");
                }
            }
        }
    }
}