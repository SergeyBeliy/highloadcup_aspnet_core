using System.Collections.Generic;
using System.Linq;
using AccountsApi.Infrastructure.Database;

namespace AccountsApi.Models
{
    public class AccountFilterResponse
    {
        public AccountFilterResponse(IEnumerable<Account> accounts, QueryBase query){
            Accounts = accounts.Select(a => new AccountShort(a, query)).ToArray();
        }
        public AccountShort[] Accounts {get;set;}
    }
}