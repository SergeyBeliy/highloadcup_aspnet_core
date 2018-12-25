using System;
using System.Collections.Generic;
using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure.Indexes {
    public class DateOrderIndex : IndexBase {
        public DateOrderIndex (string fieldName) : base (fieldName) { }

        private SortedDictionary<long, List<long>> SortedDictionary = new SortedDictionary<long, List<long>> ();
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
            if (SortedDictionary.ContainsKey (account.Birth)) {
                SortedDictionary[account.Birth].Add (account.Id);
            } else {
                SortedDictionary.Add (account.Birth, new List<long> { account.Id });
            }

        }

        private IEnumerable<long> GetLt (string value) {
            long val = GetLong(value);            var res = new List<long> ();
            foreach (var key in SortedDictionary.Keys) {
                if (key <= val)
                    res.AddRange (SortedDictionary[key]);
            }
            return res;
        }

        private IEnumerable<long> GetGt (string value) {
            long val = GetLong(value);
            var res = new List<long> ();
            foreach (var key in SortedDictionary.Keys.Reverse ()) {
                if (key > val)
                    res.AddRange (SortedDictionary[key]);

            }
            return res;
        }

        private long GetLong (string value) {
            long val;
            if (!long.TryParse (value, out val)) {
                throw new ArgumentException ("Argument must be of long type");
            }
            return val;
        }
    }
}