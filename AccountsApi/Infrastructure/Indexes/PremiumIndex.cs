using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure.Helpers;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure.Indexes {
    public class PremiumIndex : IndexBase {
        public PremiumIndex (string fieldName) : base (fieldName) { }
        public ConcurrentDictionary<long, long> NullValues = new ConcurrentDictionary<long, long> ();
        public ConcurrentDictionary<long, List<long>> StartValues { get; private set; } = new ConcurrentDictionary<long, List<long>> ();
        public ConcurrentDictionary<long, List<long>> FinishValues { get; private set; } = new ConcurrentDictionary<long, List<long>> ();
        public override Predicate[] SupportPredicates =>
            throw new System.NotImplementedException ();

        public override IEnumerable<long> GetIds (string value, Predicate predicate) {
            switch (predicate) {
                case Predicate.now:
                    return GetNow();
                case Predicate.@null:
                    return GetNull (value);
                default:
                    throw new NotSupportedException ($"Predicate {predicate} not supports");
            }
        }

        public override void Put (Account account) {
            if (account.Premium == null)
                NullValues[account.Id] = account.Id;
            else {
                PutValue (account.Premium.Start, account.Id, StartValues);
                PutValue (account.Premium.Finish, account.Id, FinishValues);
            }
        }

        private void PutValue (long key, long id, ConcurrentDictionary<long, List<long>> values) {
            if (!values.ContainsKey (key)) {
                values[key] = new List<long> ();
            }
            values[key].Add (id);
        }

        private IEnumerable<long> GetNull (string value) {
            switch (value) {
                case "0":
                    return StartValues.Values.SelectMany (s => s).Distinct ();
                case "1":
                    return NullValues.Values;
                default:
                    throw new NotSupportedException ($"Value {value} not supports");
            }
        }

        private IEnumerable<long> GetNow () {
            Int32 unixTimestamp = (Int32) (DateTime.UtcNow.Subtract (new DateTime (1970, 1, 1))).TotalSeconds;
            var startComplains = StartValues.Where (s => s.Key <= unixTimestamp).SelectMany (s => s.Value).Distinct ().ToArray();
            var finishComplains = FinishValues.Where (s => s.Key >= unixTimestamp).SelectMany (s => s.Value).Distinct ().ToArray();

            return ArrayMergeHelper.Merge(startComplains, finishComplains); 
        }
    }
}