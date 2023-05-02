using DripChipWebApplication.Server.Models;
using DripChipWebApplication.Server.Data;
using DripChipWebApplication.Server.Models.ResponseModels.Account;

namespace DripChipWebApplication.Server.Services.ServiceInterfaces
{
    public interface IAccountService
    {
        Account? Authenticate(string? email, string? password);
        AccountDTO AddAccount(AccountRegistrationDTO account);
        AccountDTO GetAccount(int id);
        Account GetAccountByEmail(string email);
        AccountDTO[] SearchAccounts(string? firstName, string? lastName, string? email, int from, int size);
        AccountDTO EditAccount(int id, AccountRegistrationDTO account);
        void DeleteAccount(int id);
    }
}
