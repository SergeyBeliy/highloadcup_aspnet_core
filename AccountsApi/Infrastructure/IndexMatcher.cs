using System.Collections.Generic;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure
{
    public class IndexMatcher
    {
        public void Index(Account account, IndexMeta meta, Dictionary<string, IndexBase> indexes){
            
        }

        private string GetValue(Account account, IndexMeta meta){
            return string.Empty;
        }
    }
}