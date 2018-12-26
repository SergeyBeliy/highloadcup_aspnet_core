using AccountsApi.Infrastructure.Database;
using AccountsApi.Models;

namespace AccountsApi.Database.Infrastructure
{
    public interface IIndexer
    {
         void Put(Account account);

         long[] QueryIndexes(Query query);
    }
}