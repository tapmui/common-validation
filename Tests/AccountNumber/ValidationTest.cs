using Collector.Common.Validation.AccountNumber;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace Collector.Common.Validation.Tests.AccountNumber
{
    public class ValidationTest
    {
        private Validator _sut;

        private string[] _csvColumns;
        private string[] _csvRows;

        [SetUp]
        public void SetUp()
        {
            _sut = new Validator(AppDomain.CurrentDomain.BaseDirectory + "banks.json");

            var csvLines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "accounts-numbers.csv"));
            if (csvLines.Length != 0)
            {
                _csvColumns = csvLines[0].Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (csvLines.Length > 1)
                {
                    _csvRows = csvLines.Skip(1).ToArray();
                }
            }
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
            try
            {
                var result = _sut.Identify(clearingNumber).Result;

                Assert.That(result.Name, Is.EqualTo(expectedBankName));
            }
            catch (ArgumentException ex)
            {
                Assert.Fail("Identification failed for clearing number {0}: {1}", clearingNumber, ex.Message);
            }
        }

        [Theory]
        [TestCase("8123", "1234567", "Swedbank")]
        [TestCase("8123-5", "7654321", "Swedbank")]
        public void Validate_AccountNumber_And_Expect_To_Identify_BankName(string clearingNumber, string accountNumber, string expectedBankName)
        {
            try
            {
                var result = _sut.Validate(clearingNumber, accountNumber).Result;

                Assert.That(result.Valid, Is.False);
                Assert.That(result.Name, Is.EqualTo(expectedBankName));
            }
            catch (ArgumentException ex)
            {
                Assert.Fail("Validation failed for number {0} {1}: {2}", clearingNumber, accountNumber, ex.Message);
            }
        }
        
        [Theory]
        public void Validate_AccountNumbers_From_CSV_File()
        {
            if (_csvRows == null || _csvRows.Length == 0)
            {
                return;
            }

            foreach (string row in _csvRows)
            {
                var values = row.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length >= 2)
                {
                    var clearingNumber = values[0];
                    var accountNumber = values[1];

                    try
                    {
                        var result = _sut.Validate(clearingNumber, accountNumber).Result;

                        Assert.That(result.Valid, Is.True);
                    }
                    catch (ArgumentException ex)
                    {
                        Assert.Fail("Validation failed for {0} {1}: {2}", clearingNumber, accountNumber, ex.Message);
                    }
                }
            }
        }
    }
}
