using System.Collections.Generic;
using System.Linq;

namespace AccountsApi.Infrastructure.Database {
    public class RecommendQuery : QueryBase {

        public long AccountId { get; set; }

        public string City {
            get {
                return Items.FirstOrDefault (s => s.Key == "city")?.Value;
            }
        }

        public string Country {
            get {
                return Items.FirstOrDefault (s => s.Key == "country")?.Value;
            }
        }
    }
}