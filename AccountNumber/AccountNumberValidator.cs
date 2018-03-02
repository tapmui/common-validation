using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Collector.Common.Validation.AccountNumber.Models;

namespace Collector.Common.Validation.AccountNumber
{
    public class AccountNumberValidator : IAccountNumberValidator
    {
        private const string NO_DIGITS_PATTERN = @"[^\d]";

        private readonly AccountValidationModel[] _banks;

        public AccountNumberValidator(string path)
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

            try
            {
                var config = _banks.Single(c => Regex.IsMatch(clearingNumber2, c.ClearingRegex));

                return Task.FromResult(
                    new BankIdentityModel(clearingNumber2.Substring(0, config.Clearing), config.Name)
                );
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException("Clearing number could not be identified", "clearingNumber");
            }
        }

        public Task<BankAccountModel> Validate(string clearingNumber, string accountNumber)
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

            // Look up validation config
            var length = clearingNumber2.Length + accountNumber2.Length;
            var configs = _banks.Where(c => (length <= (c.Clearing + c.Account)) && Regex.IsMatch(clearingNumber2, c.ClearingRegex));
            foreach (var config in configs)
            {
                // Ensure account number contains specified number of digits
                if (accountNumber2.Length < config.Account)
                {
                    var zeroes = config.Account - accountNumber2.Length;
                    accountNumber2 = String.Join(string.Empty, Enumerable.Repeat("0", zeroes).ToArray()) + accountNumber2;
                }

                var number = clearingNumber2 + accountNumber2;
                if (Regex.IsMatch(number, config.NumberRegex))
                {
                    // Extract account number digits based on validation config
                    var number2 = number.Substring(number.Length - config.Control, config.Control);

                    // Validate number according to configuration
                    if ((config.Modulo == Mod10.ID && Mod10.Validate(number2)) || (config.Modulo == Mod11.ID && Mod11.Validate(number2)))
                    {
                        // Return a new bank information model
                        return Task.FromResult(
                            new BankAccountModel(
                                name: config.Name,
                                clearingNumber: clearingNumber2,
                                accountNumber: accountNumber2
                            )
                        );
                    }
                }
            }

            throw new ArgumentException("Account number could not be validated");
        }

        public Task<BankAccountModel> Validate(string number)
        {
            if (number == null)
            {
                throw new ArgumentNullException("accountNumber", "Account number must have a value");
            }

            // Remove all non-numerical characters in parameters
            var number2 = Regex.Replace(number, NO_DIGITS_PATTERN, string.Empty);

            // Look up validation config based on clearing number Regex pattern
            var configs = _banks.Where(c => (number2.Length >= c.Clearing) && (number2.Length <= (c.Clearing + c.Account)) && Regex.IsMatch(number2.Substring(0, c.Clearing), c.ClearingRegex));
            foreach (var config in configs)
            {
                var clearingNumber = number2.Substring(0, config.Clearing);
                var accountNumber = number2.Substring(config.Clearing);

                // Ensure account number part is padded with zeroes to reach the correct length
                if (accountNumber.Length < config.Account)
                {
                    var zeroes = config.Account - accountNumber.Length;
                    accountNumber = String.Join(string.Empty, Enumerable.Repeat("0", zeroes).ToArray()) + accountNumber;
                }

                // Construct complete account number based on config
                var number3 = clearingNumber + accountNumber;
                if (!Regex.IsMatch(number3, config.NumberRegex))
                {
                    continue;
                }

                // Extract account number control digits based on validation config
                var controlNumber = number3.Substring(number3.Length - config.Control, config.Control);

                // Validate number according to configuration
                if ((config.Modulo == Mod10.ID && Mod10.Validate(controlNumber)) || (config.Modulo == Mod11.ID && Mod11.Validate(controlNumber)))
                {
                    return Task.FromResult(
                        new BankAccountModel(
                            clearingNumber: clearingNumber, 
                            accountNumber: accountNumber, 
                            name: config.Name
                        )
                    );
                }
            }

            throw new ArgumentException("Account number could not be validated", "number");
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
