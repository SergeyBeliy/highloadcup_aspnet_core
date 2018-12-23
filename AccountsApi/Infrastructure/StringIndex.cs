using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure {
    public class StringIndex : IndexBase {

        public StringIndex (string fieldName) : base (fieldName) { }

        public ConcurrentDictionary<string, List<long>> Values { get; private set; } = new ConcurrentDictionary<string, List<long>> ();

        public override IEnumerable<long> GetIds (string value, Predicate predicate, int max = 20) {
            switch (predicate) {
                case Predicate.eq:
                    return GetEq (value, max);
                case Predicate.neq:
                    return GetNeq (value, max);
                case Predicate.any:
                    return GetAny (value, max);
                default:
                    throw new NotSupportedException ($"Predicate {predicate} not supports");
            }

        }

        public override void Put (Account account) {
            var value = GetValue (account);
            if (!Values.ContainsKey (value)) {
                Values[value] = new List<long> ();
            }
            Values[value].Add (account.Id);
        }

        private string GetValue (Account account) {
            if (FieldName.Equals (nameof (account.SName), StringComparison.OrdinalIgnoreCase)) {
                return account.SName;
            }

            if (FieldName.Equals (nameof (account.FName), StringComparison.OrdinalIgnoreCase)) {
                return account.FName;
            }

            if (FieldName.Equals (nameof (account.Country), StringComparison.OrdinalIgnoreCase)) {
                return account.Country;
            }

            if (FieldName.Equals (nameof (account.City), StringComparison.OrdinalIgnoreCase)) {
                return account.City;
            }

            if (FieldName.Equals (nameof (account.Phone), StringComparison.OrdinalIgnoreCase)) {
                return account.Phone;
            }

            if (FieldName.Equals (nameof (account.EMail), StringComparison.OrdinalIgnoreCase)) {
                return account.EMail;
            }

            if (FieldName.Equals (nameof (account.Sex), StringComparison.OrdinalIgnoreCase)) {
                return account.Sex.ToString ();
            }

            if (FieldName.Equals (nameof (account.Status), StringComparison.OrdinalIgnoreCase)) {
                return account.Status.ToString ();
            }

            throw new Exception ("Property not found");
        }

        private IEnumerable<long> GetEq (string value, int max) {
            if (Values.ContainsKey (value)) {
                return Values[value].Take(max).ToArray ();
            }
            return new long[] { };
        }
        private IEnumerable<long> GetNeq (string value, int max) {
            int counter = 0;
            foreach (var item in Values) {
                if (item.Key != value) {
                    foreach(var id in item.Value){
                        if(counter++ > max) yield break;
                        yield return id;
                    }
                }
            }
        }

        private IEnumerable<long> GetAny (string value, int max) {
            int counter = 0;
            var valuesToFind = value.Split(",");
            foreach (var item in Values) {
                if (valuesToFind.Any(v => v == item.Key)) {
                    foreach(var id in item.Value){
                        if(counter++ > max) yield break;
                        yield return id;
                    }
                }
            }
        }
    
    }
}