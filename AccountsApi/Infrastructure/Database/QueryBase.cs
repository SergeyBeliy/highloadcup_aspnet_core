using System.Collections.Generic;

namespace AccountsApi.Infrastructure.Database {
    public abstract class QueryBase {
        public IEnumerable<QueryItem> Items { get; set; }

        public int Limit { get; set; }

    }
}