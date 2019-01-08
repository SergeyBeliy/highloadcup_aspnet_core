using System;

namespace AccountsApi.Infrastructure.Database {
    public class QueryItem {
        public string Key { get; set; }

        public string Value { get; set; }

        public string FieldName {
            get {
                if (string.IsNullOrEmpty (Key))
                    return string.Empty;
                return Key.Split ("_") [0].ToLower();
            }
        }

        public Predicate? Predicate {
            get {
                if (string.IsNullOrEmpty (Key))
                    throw new ArgumentException ("Wrong predicate");
                var vals = Key.Split ("_");
                if (vals.Length == 1) {
                    return null;
                }
                if (vals.Length != 2)
                    throw new ArgumentException ("Wrong key value");
                Predicate predicate;
                if (Enum.TryParse (vals[1], out predicate)) {
                    return predicate;
                }
                throw new ArgumentException ("Wrong predicate");
            }
        }
    }
}