//AccountGroupResponse
using System.Collections.Generic;
using System.Linq;

namespace AccountsApi.Models {
    public class AccountGroupResponse {
        public IEnumerable<AccountGroup> Groups { get; set; }

    }
}