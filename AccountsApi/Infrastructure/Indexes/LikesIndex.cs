using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure.Helpers;
using AccountsApi.Models;

namespace AccountsApi.Infrastructure.Indexes {
    public class LikesIndex : IndexBase {
        public LikesIndex (string fieldName) : base (fieldName) { }
        public ConcurrentDictionary<string, List<long>> Values { get; private set; } = new ConcurrentDictionary<string, List<long>> ();

        public override Predicate[] SupportPredicates => new Predicate[] { Predicate.contains };

        public override IEnumerable<long> GetIds (string value, Predicate predicate) {
            if(predicate != Predicate.contains)
                throw new NotSupportedException ($"Predicate {predicate} not supports"); 
            return GetContains(value);
        }

        public override void Put (Account account) {
            var likes = account.Likes ?? new LikeModel[0];
            var likeKeys = likes.Length == 0?new string[] {string.Empty}: likes.Select(l => l.Id.ToString()).ToArray();
            foreach(var likeKey in likeKeys){
                PutLike(likeKey, account.Id);
            }
        }

        private void PutLike (string key, long id) {
            if (!Values.ContainsKey (key)) {
                Values[key] = new List<long> ();
            }
            Values[key].Add (id);
        }

        private IEnumerable<long> GetContains(string value){
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