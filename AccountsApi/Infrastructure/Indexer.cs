using System.Collections.Generic;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure {
    public class Indexer: IIndexer {

        private IndexMatcher indexMatcher = new IndexMatcher();
        public Indexer(IndexMeta[] metas)
        {
            Metas = metas;
        }
        public IndexMeta[] Metas { get; set; }
        
        public Dictionary<string, IndexBase> Indexes { get; private set; } = new Dictionary<string, IndexBase>();

        public long[] Get(string name, string value)
        {
            throw new System.NotImplementedException();
        }

        public void Put(Account account)
        {
            throw new System.NotImplementedException();
        }
    }
}