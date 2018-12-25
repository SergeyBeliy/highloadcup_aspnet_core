using System.Collections.Generic;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure.Indexes;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure {
    public class Indexer : IIndexer {

        public Indexer () {
            Indexes["sex"] = new IndexBase[] { new StringIndex ("sex") };
            Indexes["email"] = new IndexBase[] { new EmailDomainIndex ("email"), new EmailOrderIndex ("email") };
            Indexes["status"] = new IndexBase[] { new StringIndex ("status") };
            Indexes["fname"] = new IndexBase[] { new StringIndex ("fname") };
            Indexes["sname"] = new IndexBase[] { new StringIndex ("sname") };
            Indexes["phone"] = new IndexBase[] { new StringIndex ("phone"), new PhoneIndex ("phone") };
            Indexes["country"] = new IndexBase[] { new StringIndex ("country") };
            Indexes["city"] = new IndexBase[] { new StringIndex ("city") };
            Indexes["birth"] = new IndexBase[] { new DateOrderIndex ("birth"), new DateYearIndex ("birth") };
            Indexes["interests"] = new IndexBase[] { new StringArrayIndex ("interests") };
        }
      
        public Dictionary<string, IndexBase[]> Indexes { get; private set; } = new Dictionary<string, IndexBase[]> ();

        public long[] Get (string name, string value) {
            throw new System.NotImplementedException ();
        }

        public void Put (Account account) {
            foreach(var indexes in Indexes){
                foreach(var index in indexes.Value)
                    index.Put(account);
            }
        }
    }
}