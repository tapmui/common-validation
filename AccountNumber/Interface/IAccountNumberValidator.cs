using System.Threading.Tasks;
using Collector.Common.Validation.AccountNumber.Models;

namespace Collector.Common.Validation.AccountNumber.Interface
{
    public interface IAccountNumberValidator
    {
        Task<BankAccountModel> Identify(string number);
        Task<bool> TryIdentify(string number, out BankAccountModel result);
        Task<bool>              TryValidate(string number, out BankAccountModel result);
        Task<BankAccountModel>  Validate(string number);
    }
}