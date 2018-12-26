using System.Collections.Generic;
using System.Linq;

namespace AccountsApi.Models
{
    public class AccountFilterResponse
    {
        public AccountFilterResponse(IEnumerable<Account> accounts){
            Accounts = accounts.Select(a => new AccountShort(a)).ToArray();
        }
        public AccountShort[] Accounts {get;set;}
    }
}