using System.Collections.Generic;
using System.Linq;

namespace AccountsApi.Models {
    public class AccountGroup {

        public long Count { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Sex { get; set; }

        public string Status { get; set; }

        public string Interests { get; set; }
    }
}