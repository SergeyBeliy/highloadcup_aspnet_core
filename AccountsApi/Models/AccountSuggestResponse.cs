using System.Collections.Generic;
using System.Linq;

namespace AccountsApi.Models
{
    public class AccountSuggestResponse
    {
        public AccountSuggestResponse(IEnumerable<Suggestion> suggestions){
            Accounts =suggestions.ToArray();
        }
        public Suggestion[] Accounts {get;set;}
    }
}