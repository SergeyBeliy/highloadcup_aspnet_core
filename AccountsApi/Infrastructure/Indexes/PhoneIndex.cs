using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure.Indexes {
    public class PhoneIndex : IndexBase {
        public PhoneIndex (string fieldName) : base (fieldName) { }

        private ConcurrentDictionary<string, List<long>> Values = new ConcurrentDictionary<string, List<long>> ();
        public override Predicate[] SupportPredicates => new Predicate[] { Predicate.code };

        public override IEnumerable<long> GetIds (string value, Predicate predicate) {
            if (predicate == Predicate.code) {
                if (Values.ContainsKey (value)) {
                    return Values[value];
                }
                return new List<long> ();
            }
            throw new NotSupportedException ($"Predicate {predicate} not supports");
        }

        public override void Put (Account account) {
            var code = GetCode (account);
            if (!Values.ContainsKey (code)) {
                Values[code] = new List<long> ();
            }
            Values[code].Add (account.Id);
        }

        private string GetCode (Account account) {
            if (string.IsNullOrEmpty (account.Phone)) {
                return string.Empty;
            }
            var regex = new Regex ("\\((\\d\\d\\d)\\)");
            var matches = regex.Matches (account.Phone);
            if (matches.Count > 0 &&  matches[0].Groups.Count > 1) {
                return matches[0].Groups[1].Value;
            }
            return string.Empty;
        }
    }
}