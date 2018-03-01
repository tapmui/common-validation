using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Collector.Common.Validation.AccountNumber.Models;

namespace Collector.Common.Validation.AccountNumber
{
    public class Validator
    {
        private const string NO_DIGITS_PATTERN = @"[^\d]";

        private readonly AccountValidationModel[] _banks;

        public Validator(string path)
        {
            _banks = JsonConvert.DeserializeObject<AccountValidationModel[]>(File.ReadAllText(path));
        }

        public Task<BankIdentityModel> Identify(string clearingNumber)
        {
            if (clearingNumber == null)
            {
                throw new ArgumentNullException("clearingNumber", "Clearing number must have a value");
            }

            var clearingNumber2 = Regex.Replace(clearingNumber, NO_DIGITS_PATTERN, string.Empty);
            if (string.IsNullOrEmpty(clearingNumber2))
            {
                throw new ArgumentException("Clearing number must contain digits", "clearingNumber");
            }

            var config = _banks
                .Where(c => c.Clearing <= clearingNumber2.Length)
                .FirstOrDefault(c => Regex.IsMatch(clearingNumber2.Substring(0, c.Clearing), c.ClearingRegex));
            if (config == null)
            {
                throw new ArgumentException("Cannot identify clearing number");
            }

            return Task.FromResult(
                new BankIdentityModel(clearingNumber2.Substring(0, config.Clearing) , config.Name)
            );
        }

        public Task<BankAccountModel> Validate(string clearingNumber, string accountNumber, bool padAccountNumberWithZeroes = true)
        {
            if (clearingNumber == null)
            {
                throw new ArgumentNullException("clearingNumber", "Clearing number must have a value");
            }

            if (accountNumber == null)
            {
                throw new ArgumentNullException("accountNumber", "Account number must have a value");
            }

            // Remove all non-numerical characters in parameters
            var clearingNumber2 = Regex.Replace(clearingNumber, NO_DIGITS_PATTERN, string.Empty);
            var accountNumber2 = Regex.Replace(accountNumber, NO_DIGITS_PATTERN, string.Empty);
            
            // Look up validation config based on account number Regex pattern
            var config = _banks
                .Where(c => c.Clearing <= clearingNumber2.Length)
                .FirstOrDefault(c => Regex.IsMatch(clearingNumber2.Substring(0, c.Clearing), c.ClearingRegex));
            if (config == null)
            {
                throw new ArgumentException("Could not match clearing number to a bank");
            }

            // Ensure account number contains specified number of digits
            if (padAccountNumberWithZeroes && accountNumber2.Length < config.Account)
            {
                var zeroes = config.Account - accountNumber2.Length;
                accountNumber2 = String.Join(string.Empty, Enumerable.Repeat("0", zeroes).ToArray()) + accountNumber2;
            }

            bool isValid = false;

            var number = clearingNumber2 + accountNumber2;
            if (Regex.IsMatch(number, config.NumberRegex))
            {
                // Extract account number digits based on validation config
                var number2 = number.Substring(number.Length - config.Control, config.Control);

                // Validate number according to configuration
                isValid = (config.Modulo == Mod10.ID && Mod10.Validate(number2)) || (config.Modulo == Mod11.ID && Mod11.Validate(number2));
            }

            // Return a new bank information model
            return Task.FromResult(
                new BankAccountModel(
                    name: config.Name,
                    clearingNumber: clearingNumber2,
                    accountNumber: accountNumber2,
                    valid: isValid
                )
            );
        }

        public Task<bool> TryIdentify(string clearingNumber, out BankIdentityModel result)
        {
            try
            {
                result = Identify(clearingNumber).Result;
            }
            catch (ArgumentException)
            {
                result = null;

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> TryValidate(string clearingNumber, string accountNumber, out BankAccountModel result)
        {
            try
            {
                result = Validate(clearingNumber, accountNumber).Result;
            }
            catch (ArgumentException)
            {
                result = null;

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
