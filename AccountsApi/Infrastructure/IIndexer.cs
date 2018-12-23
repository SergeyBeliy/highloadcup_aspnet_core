using AccountsApi.Models;

namespace AccountsApi.Infrastructure
{
    public interface IIndexer
    {
         void Put(Account account);

         long[] Get(string name, string value);
    }
}