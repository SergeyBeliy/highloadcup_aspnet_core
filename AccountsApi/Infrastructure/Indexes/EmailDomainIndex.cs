using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AccountsApi.Infrastructure;
using AccountsApi.Models;

namespace AccountsApi.Index.Infrastructure {
    public class EmailDomainIndex : IndexBase {
        public ConcurrentDictionary<string, List<long>> Values { get; private set; } = new ConcurrentDictionary<string, List<long>> ();

        public override Predicate[] SupportPredicates => new Predicate[] { Predicate.domain };

        public EmailDomainIndex (string fieldName) : base (fieldName) { }

        public override IEnumerable<long> GetIds (string value, Predicate predicate) {
            if (predicate == Predicate.domain) {
                if(Values.ContainsKey(value)) 
                    return Values[value];
                return new List<long> ();
            }
            throw new NotSupportedException ($"Predicate {predicate} not supports");
        }

        public override void Put (Account account) {
            var value = GetEmailDomain (account);
            if (!Values.ContainsKey (value)) {
                Values[value] = new List<long> ();
            }
            Values[value].Add (account.Id);
        }

        private string GetEmailDomain (Account account) {
            var email = account.EMail??string.Empty;
            var res = email.Split ('@');
            if (res.Length > 1)
                return res[1];
            return string.Empty;

        }
    }
}