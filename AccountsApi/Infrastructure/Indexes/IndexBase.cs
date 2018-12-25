using System.Collections.Generic;
using AccountsApi.Infrastructure;
using AccountsApi.Models;

namespace AccountsApi.Index.Infrastructure
{
    public abstract class IndexBase {

        public IndexBase(string fieldName){
            FieldName = fieldName;
        }

        public abstract Predicate[] SupportPredicates { get;}
        public string FieldName { get; set; }

        public abstract void Put(Account account);
        
        public abstract IEnumerable<long> GetIds(string value, Predicate predicate);

    }
}