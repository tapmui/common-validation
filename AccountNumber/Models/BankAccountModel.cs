namespace Collector.Common.Validation.AccountNumber.Models
{
    public class BankAccountModel
    {
        public BankAccountModel(
            string name,
            string clearingNumber,
            string accountNumber)
        {
            Name           = name;
            ClearingNumber = clearingNumber;
            AccountNumber  = accountNumber;
        }

        public string Name           { get; }
        public string ClearingNumber { get; }
        public string AccountNumber  { get; }
    }
}