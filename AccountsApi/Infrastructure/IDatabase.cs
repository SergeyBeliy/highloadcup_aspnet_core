using AccountsApi.Models;

namespace AccountsApi.Infrastructure {
    public interface IDatabase {
        void Put(Account account);
    }
}