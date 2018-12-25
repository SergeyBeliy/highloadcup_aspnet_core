using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure.Helpers;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure.Indexes {
    public class StringArrayIndex : IndexBase {
        public ConcurrentDictionary<string, List<long>> Values { get; private set; } = new ConcurrentDictionary<string, List<long>> ();

        public StringArrayIndex (string fieldName) : base (fieldName) { }

        public override Predicate[] SupportPredicates => new Predicate[] { Predicate.any, Predicate.contains };

        public override IEnumerable<long> GetIds (string value, Predicate predicate) {
            switch(predicate){
                case Predicate.contains:
                    return GetContains(value);
                case Predicate.any:
                    return GetAny(value);
                default:
                    throw new NotSupportedException ($"Predicate {predicate} not supports");
            }
        }

        public override void Put (Account account) {
            var values = GetValues (account);
            foreach (var value in values) {
                if (!Values.ContainsKey (value)) {
                    Values[value] = new List<long> ();
                }
                Values[value].Add (account.Id);
            }
        }

        private string[] GetValues (Account account) {
            return account.Interests??new string[0];
        }

        private IEnumerable<long> GetAny (string value) {
            var result = new List<long> ();
            var values = value.Split (',');
            foreach (var val in values) {
                if (Values.ContainsKey (val)) {
                    result.AddRange (Values[val]);
                }
            }
            return result.Distinct().ToArray();
        }

        private IEnumerable<long> GetContains (string value) {
            if (string.IsNullOrEmpty (value))
                return new long[0];
            var values = value.Split (",");
            List<long[]> resToMerge = new List<long[]> ();
            foreach (var val in values) {
                if(!Values.ContainsKey(val)){
                    return new long[0];
                }
                resToMerge.Add(Values[val].ToArray());
            }
            return ArrayMergeHelper.Merge(resToMerge.ToArray());
        }
    }
}