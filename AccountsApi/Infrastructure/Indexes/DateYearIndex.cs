using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure.Indexes {
    public class DateYearIndex : IndexBase {
        public DateYearIndex (string fieldName) : base (fieldName) { }

        private ConcurrentDictionary<int, List<long>> Values = new ConcurrentDictionary<int, List<long>> ();
        public override Predicate[] SupportPredicates => new Predicate[] { Predicate.year };

        public override IEnumerable<long> GetIds (string value, Predicate predicate) {
            if (predicate == Predicate.year) {
                var val = GetInt (value) ;
                if (Values.ContainsKey (val)) {
                    return Values[val];
                }
                return new List<long> ();
            }
            throw new NotSupportedException ($"Predicate {predicate} not supports");
        }

        public override void Put (Account account) {
            var year = GetYear (account);
            if (!Values.ContainsKey (year)) {
                Values[year] = new List<long> ();
            }
            Values[year].Add (account.Id);
        }

        private int GetYear (Account account) {
            System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(account.Birth).ToLocalTime();
            return dtDateTime.Year;
        }

        private int GetInt (string value) {
            int val;
            if (!int.TryParse (value, out val)) {
                throw new ArgumentException ("Argument must be of int type");
            }
            return val;
        }
    }
}