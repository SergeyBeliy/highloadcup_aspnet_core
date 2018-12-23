using System.Collections.Generic;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure
{
    public abstract class IndexBase {

        public IndexBase(string fieldName){
            FieldName = fieldName;
        }
        public string FieldName { get; set; }

        public abstract void Put(Account account);
        
        public abstract IEnumerable<long> GetIds(string value, Predicate predicate, int max = 20);

    }
}