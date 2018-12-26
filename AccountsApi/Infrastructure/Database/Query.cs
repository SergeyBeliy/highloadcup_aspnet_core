using System.Collections.Generic;

namespace AccountsApi.Infrastructure.Database {
    public class Query {
        public IEnumerable<QueryItem> Items { get; set; }

        public int Limit { get; set; }
    }
}