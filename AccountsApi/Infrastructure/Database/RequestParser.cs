using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;

namespace AccountsApi.Infrastructure.Database {
    public static class RequestParser {
        private static Dictionary<string, Predicate[]> SupportedPredicates = new Dictionary<string, Predicate[]> { { "sex", new Predicate[] { Predicate.eq } },
            { "email", new Predicate[] { Predicate.domain, Predicate.lt, Predicate.gt } },
            { "status", new Predicate[] { Predicate.eq, Predicate.neq } },
            { "fname", new Predicate[] { Predicate.eq, Predicate.any, Predicate.@null } },
            { "sname", new Predicate[] { Predicate.eq, Predicate.starts, Predicate.@null } },
            { "phone", new Predicate[] { Predicate.code, Predicate.@null } },
            { "country", new Predicate[] { Predicate.eq, Predicate.@null } },
            { "city", new Predicate[] { Predicate.eq, Predicate.any, Predicate.@null } },
            { "birth", new Predicate[] { Predicate.lt, Predicate.gt, Predicate.year } },
            { "interests", new Predicate[] { Predicate.any, Predicate.contains } },
            { "likes", new Predicate[] { Predicate.contains } },
            { "premium", new Predicate[] { Predicate.@null, Predicate.now } },
        };
        public static FilterQuery ParseFilter (string queryString) {
            var result = new FilterQuery ();
            var queryItems = new List<QueryItem> ();
            var dict = QueryHelpers.ParseQuery (queryString);
            foreach (var item in dict) {
                if (item.Key.Equals ("query_id", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (item.Key.Equals ("limit", StringComparison.OrdinalIgnoreCase)) {
                    int limit;
                    if (int.TryParse (item.Value.ToString (), out limit)) {
                        result.Limit = limit;
                    }
                    continue;
                }
                queryItems.Add (new QueryItem () {
                    Key = item.Key,
                        Value = item.Value.ToString ()
                });
            }
            result.Items = queryItems;
            return result;
        }

        public static GroupQuery ParseGroup (string queryString) {
            var result = new GroupQuery ();
            var queryItems = new List<QueryItem> ();
            var dict = QueryHelpers.ParseQuery (queryString);
            foreach (var item in dict) {
                if (item.Key.Equals ("query_id", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (item.Key.Equals ("limit", StringComparison.OrdinalIgnoreCase)) {
                    int limit;
                    if (int.TryParse (item.Value.ToString (), out limit)) {
                        result.Limit = limit;
                    }
                    continue;
                }
                if (item.Key.Equals ("keys", StringComparison.OrdinalIgnoreCase)) {
                    result.Keys = item.Value;
                    continue;
                }
                if (item.Key.Equals ("order", StringComparison.OrdinalIgnoreCase)) {
                    int order;
                    if (int.TryParse (item.Value.ToString (), out order)) {
                        result.Order = order;
                    }
                    continue;
                }

                queryItems.Add (new QueryItem () {
                    Key = item.Key,
                        Value = item.Value.ToString ()
                });
            }
            result.Items = queryItems;
            return result;
        }
        public static bool ValidateFilterQuery (FilterQuery query) {
            if (query.Limit <= 0) {
                return false;
            }
            foreach (var item in query.Items) {
                if (!SupportedPredicates.ContainsKey (item.FieldName)) {
                    return false;
                }
                if (!item.Predicate.HasValue) {
                    return false;
                }
                if (!SupportedPredicates[item.FieldName].Contains (item.Predicate.Value)) {
                    return false;
                }
            }
            return true;
        }

        public static bool ValidateGroupQuery (GroupQuery query) {
            if (query.Limit <= 0) {
                return false;
            }
            if (query.Order != 1 && query.Order != -1) {
                return false;
            }
            if (string.IsNullOrEmpty (query.Keys)) {
                return false;
            }
            foreach (var item in query.Items) {
                if (item.Predicate.HasValue) {
                    return false;
                }
            }
            return true;
        }
    }
}