using Collector.Common.Validation.AccountNumber;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Collector.Common.Validation.Tests.AccountNumber
{
    public class ValidationTest
    {
        private IAccountNumberValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new AccountNumberValidator(AppDomain.CurrentDomain.BaseDirectory + "banks.json");
        }

        [Theory]
        [TestCase("9561", "Avanza Bank")]
        [TestCase("9046", "Citibank")]
        [TestCase("9188", "Danske Bank")]
        [TestCase("6123", "Handelsbanken")]
        [TestCase("1119", "Nordea")]
        [TestCase("2030", "Nordea")]
        [TestCase("3311", "Nordea")]
        [TestCase("4123", "Nordea")]
        [TestCase("5482", "SEB")]
        [TestCase("9137", "SEB")]
        [TestCase("8123", "Swedbank")]
        [TestCase("8123-5", "Swedbank")]
        public void Identify_Bank_By_ClearingNumber(string clearingNumber, string expectedBankName)
        {
            var result = _sut.Identify(clearingNumber).Result;

            Assert.That(result.Name, Is.EqualTo(expectedBankName));
        }

        [Theory]
        [TestCase("8123", "1234567", "Swedbank")]
        [TestCase("8123-5", "7654321", "Swedbank")]
        public void Validate_AccountNumber_And_Expect_Exception(string clearingNumber, string accountNumber, string expectedBankName)
        {
            Assert.ThrowsAsync<ArgumentException>(() => _sut.Validate(clearingNumber, accountNumber));
        }

        [Theory]
        [TestCase("1110", "0000100", "Nordea")]
        [TestCase("3300", "0009100", "Nordea")]
        public void Validate_AccountNumber_and_Expect_BankName(string clearingNumber, string accountNumber, string expectedBankName)
        {
            var result = _sut.Validate(clearingNumber, accountNumber).Result;

            Assert.That(result.Name, Is.EqualTo(expectedBankName));
        }

        [Theory]
        [Explicit]
        [TestCaseSource("GetNumbers")]
        public void Validate_AccountNumbers_From_Source_Where_Bank_Is_Recognised(string number)
        {
            Assert.DoesNotThrowAsync(() => _sut.Validate(number));
        }

        private static IEnumerable<object> GetNumbers()
        {
            var allRows = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "numbers.csv"));
            foreach (var line in allRows)
            {
                yield return new object[] { line };
            }
        }
    }
}
