using System.Collections.Generic;

namespace AccountsApi.Infrastructure.Database {
    public class GroupQuery : QueryBase {

        public int Order { get; set; }

        public string Keys { get; set; }

    }
}