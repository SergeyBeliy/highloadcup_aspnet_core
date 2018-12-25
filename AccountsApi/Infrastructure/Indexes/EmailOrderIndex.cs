using System;
using System.Collections.Generic;
using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure.Indexes {
    public class EmailOrderIndex : IndexBase {
        public EmailOrderIndex (string fieldName) : base (fieldName) { }

        private SortedDictionary<string, List<long>> SortedDictionary = new SortedDictionary<string, List<long>> ();
        public override Predicate[] SupportPredicates => new Predicate[] { Predicate.lt, Predicate.gt };

        public override IEnumerable<long> GetIds (string value, Predicate predicate) {
            switch (predicate) {
                case Predicate.lt:
                    return GetLt (value);
                case Predicate.gt:
                    return GetGt (value);
                default:
                    throw new NotSupportedException ($"Predicate {predicate} not supports");
            }
        }

        public override void Put (Account account) {
            if (SortedDictionary.ContainsKey (account.EMail)) {
                SortedDictionary[account.EMail].Add (account.Id);
            } else {
                SortedDictionary.Add (account.EMail, new List<long> { account.Id });
            }

        }

        private IEnumerable<long> GetLt (string value) {
            var res = new List<long> ();
            foreach (var key in SortedDictionary.Keys) {
                if (string.Compare (key, value, StringComparison.InvariantCulture) <= 0)
                    res.AddRange (SortedDictionary[key]);
            }
            return res;
        }

        private IEnumerable<long> GetGt (string value) {
            var res = new List<long> ();
            foreach (var key in SortedDictionary.Keys.Reverse ()) {
                if (string.Compare (key, value, StringComparison.InvariantCulture) > 0)
                    res.AddRange (SortedDictionary[key]);

            }
            return res;
        }

    }
}