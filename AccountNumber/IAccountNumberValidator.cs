using System.Threading.Tasks;
using Collector.Common.Validation.AccountNumber.Models;

namespace Collector.Common.Validation.AccountNumber
{
    public interface IAccountNumberValidator
    {
        Task<BankIdentityModel> Identify(string clearingNumber);
        Task<bool>              TryIdentify(string clearingNumber, out BankIdentityModel result);
        Task<bool>              TryValidate(string clearingNumber, string accountNumber, out BankAccountModel result);
        Task<BankAccountModel>  Validate(string number);
        Task<BankAccountModel>  Validate(string clearingNumber, string accountNumber);
    }
}