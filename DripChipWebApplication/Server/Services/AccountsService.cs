using DripChipWebApplication.Server.Models;
using DripChipWebApplication.Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DripChipWebApplication.Server.Services.ServiceInterfaces;
using DripChipWebApplication.Server.Models.ResponseModels.Account;
using System.Text.RegularExpressions;

namespace DripChipWebApplication.Server.Services
{
    public class AccountsService : IAccountService
    {
        IDbContextFactory<APIDbContext> contextFactory;
        public AccountsService(IDbContextFactory<APIDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public AccountDTO AddAccount(AccountRegistrationDTO account)
        {
            using var dbContext = contextFactory.CreateDbContext();
            Account dbAccont = new Account();
            dbAccont.FirstName = account.firstName;
            dbAccont.LastName = account.lastName;
            dbAccont.Email = account.email;
            dbAccont.Password = account.password;
            dbContext.Accounts.Add(dbAccont);
            dbContext.SaveChanges();
            AccountDTO acc = new AccountDTO(dbAccont);
            return acc;
        }

        public Account? Authenticate(string? email, string? password)
        {
            if (email == null && password == null)
            {
                Account guestAccount = new Account();
                guestAccount.Email = "guest";
                return guestAccount;
            }
            using var dbContext = contextFactory.CreateDbContext();
            var account = dbContext.Accounts.FirstOrDefault(x => x.Email == email && x.Password == password);
            return account;
        }

        public void DeleteAccount(int id)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var accountToDeleting = dbContext.Accounts.First(x =>x.Id == id);
            dbContext.Accounts.Remove(accountToDeleting);
            dbContext.SaveChanges();
        }

        public AccountDTO EditAccount(int id, AccountRegistrationDTO account)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var accountToEdit = dbContext.Accounts.First(x => x.Id == id);
            accountToEdit.FirstName = account.firstName;
            accountToEdit.LastName = account.lastName;
            accountToEdit.Email = account.email;
            accountToEdit.Password = account.password;
            dbContext.SaveChanges();
            AccountDTO acc = new AccountDTO(dbContext.Accounts.First(x => x.Id == id));
            return acc;
        }

        public AccountDTO? GetAccount(int id)
        {
            using var dbContext = contextFactory.CreateDbContext();
            if (!dbContext.Accounts.Any(x => x.Id == id))
            {
                return null;
            }
            AccountDTO account = new AccountDTO(dbContext.Accounts.First(x => x.Id == id));
            return account;
        }

        public Account? GetAccountByEmail(string email)
        {
            using var dbContext = contextFactory.CreateDbContext();
            return dbContext.Accounts.FirstOrDefault(x => x.Email == email);
        }

        public AccountDTO[]? SearchAccounts(string? firstName, string? lastName, string? email, int from, int size)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var filteredResult = dbContext.Accounts.AsQueryable();
            if (firstName != null)
            {
                filteredResult = filteredResult.Where(x => x.FirstName.ToLower().Contains(firstName.ToLower()));
            }
            if (lastName != null)
            {
                filteredResult = filteredResult.Where(x => x.LastName.ToLower().Contains(lastName.ToLower()));
            }
            if (email != null)
            {
                filteredResult = filteredResult.Where(x => x.Email.ToLower().Contains(email.ToLower()));
            }
            if (filteredResult.Count() == 0)
            {
                return null;
            }
            filteredResult = filteredResult.OrderBy(x=> x.Id).Skip(from).Take(size);
            List<AccountDTO> accList = new List<AccountDTO>();
            foreach(var account in filteredResult) 
            {
                AccountDTO ac = new AccountDTO(account);
                accList.Add(ac);
            }
            return accList.ToArray();
        }
    }
}
