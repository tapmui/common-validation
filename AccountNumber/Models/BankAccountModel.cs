namespace Collector.Common.Validation.AccountNumber.Models
{
    public class BankAccountModel
    {
        public BankAccountModel(
            string name, 
            string clearingNumber, 
            string accountNumber, 
            bool valid)
        {
            Name = name;
            ClearingNumber = clearingNumber;
            AccountNumber = accountNumber;
            Valid = valid;
        }

        public string Name { get; }
        public string ClearingNumber { get; }
        public string AccountNumber { get; }
        public bool Valid { get; }
    }
}