using System.Collections.Generic;
using System.Linq;

namespace AccountsApi.Models
{
    public class AccountRecommendResponse
    {
        public AccountRecommendResponse(IEnumerable<Recommendation> recommendations){
            Accounts =recommendations.ToArray();
        }
        public Recommendation[] Accounts {get;set;}
    }
}