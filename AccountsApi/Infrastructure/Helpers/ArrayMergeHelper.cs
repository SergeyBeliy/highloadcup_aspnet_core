using System.Collections.Generic;
using System.Linq;

namespace AccountsApi.Infrastructure.Helpers {
    public class ArrayMergeHelper {
        public static long[] Merge (params long[][] arrs) {
            if(arrs.Length == 0)
                return new long[0];
            if(arrs[0].Length == 0)
                return new long[0];
            IEnumerable<long> res = arrs[0];
            for (int i = 1; i < arrs.Length; i++) {
               res = res.Intersect(arrs[i]);    
               if(!res.Any())
                return res.ToArray(); 
            }
            return res.ToArray ();
        }

    }
}