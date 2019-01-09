using System.Collections.Generic;
using System.Threading.Tasks;
using AccountsApi.Infrastructure.Database;
using AccountsApi.Models;

namespace AccountsApi.Database.Infrastructure {
    public interface IDatabase {

        void InitData(string initialDataPath);
        Task Put(Account account);

        IEnumerable<Account> FilterQuery(FilterQuery query);

        IEnumerable<AccountGroup> GroupQuery (GroupQuery query);

        IEnumerable<Recommendation> RecommendQuery (RecommendQuery query);
    }
}