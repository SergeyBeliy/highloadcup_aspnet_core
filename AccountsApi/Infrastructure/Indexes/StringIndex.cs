using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AccountsApi.Infrastructure;
using AccountsApi.Models;

namespace AccountsApi.Index.Infrastructure {
    public class StringIndex : IndexBase {

        public StringIndex (string fieldName) : base (fieldName) { }

        public ConcurrentDictionary<string, List<long>> Values { get; private set; } = new ConcurrentDictionary<string, List<long>> ();

        public override Predicate[] SupportPredicates => 
            new Predicate[]{Predicate.eq, Predicate.neq, Predicate.any, Predicate.@null};

        public override IEnumerable<long> GetIds (string value, Predicate predicate) {
            switch (predicate) {
                case Predicate.eq:
                    return GetEq (value);
                case Predicate.neq:
                    return GetNeq (value);
                case Predicate.any:
                    return GetAny (value);
                case Predicate.@null:
                    return GetNull (value);
                default:
                    throw new NotSupportedException ($"Predicate {predicate} not supports");
            }

        }

        public override void Put (Account account) {
            var value = GetValue (account) ?? string.Empty;
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
                return account.Sex.ToString ().ToLower();
            }

            if (FieldName.Equals (nameof (account.Status), StringComparison.OrdinalIgnoreCase)) {
                return account.Status;
            }

            throw new Exception ("Property not found");
        }

        private IEnumerable<long> GetEq (string value) {
            if (Values.ContainsKey (value)) {
                return Values[value];
            }
            return new long[] { };
        }

        private IEnumerable<long> GetNeq (string value) {
            var res = new List<long> ();
            foreach (var item in Values) {
                if (item.Key != value) {
                    res.AddRange (item.Value);
                }
            }
            return res.ToArray ();
        }

        private IEnumerable<long> GetAny (string value) {
            var valuesToFind = value.Split (",");
            var res = new List<long> ();
            foreach (var item in Values) {
                if (valuesToFind.Any (v => v == item.Key)) {
                    res.AddRange (item.Value);
                }
            }
            return res.ToArray ();
        }

        private IEnumerable<long> GetNull (string value) {
            switch (value) {
                case "0":
                    return Values.Where(k => k.Key != string.Empty).SelectMany(s => s.Value);
                case "1":
                    if (Values.ContainsKey (string.Empty)) {
                        return Values[string.Empty];
                    }
                    return new long[] { };
                default:
                    throw new NotSupportedException ($"Value {value} not supports");
            }

        }

    }
}