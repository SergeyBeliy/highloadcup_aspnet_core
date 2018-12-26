using System.Collections.Generic;
using AccountsApi.Infrastructure.Database;
using AccountsApi.Models;

namespace AccountsApi.Database.Infrastructure {
    public interface IDatabase {
        void Put(Account account);

        IEnumerable<Account> Query(Query query);
    }
}