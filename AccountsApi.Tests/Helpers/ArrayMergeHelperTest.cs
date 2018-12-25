using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure;
using AccountsApi.Infrastructure.Helpers;
using AccountsApi.Infrastructure.Indexes;
using AccountsApi.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests {
    public class ArrayMergeHelperTest {

        [Test]
        public void MergeTest () {
            var actual = ArrayMergeHelper.Merge (new long[] { 1, 2, 3 }, new long[] { 2, 3, 4 });
            var expected = new long[] { 2, 3 };

            Assert.IsTrue (ArraysEquals (expected, actual));
        }

        public void MergeDifferentTest () {
            var actual = ArrayMergeHelper.Merge (new long[] { 1, 2, 3 }, new long[] { 4, 5, 6 });
            var expected = new long[] { };

            Assert.IsTrue (ArraysEquals (expected, actual));
        }

        public void MergeEqualsTest () {
            var actual = ArrayMergeHelper.Merge (new long[] { 1, 2, 3 }, new long[] { 3,2,1 });
            var expected = new long[] { 1,2,3};

            Assert.IsTrue (ArraysEquals (expected, actual));
        }

        private bool ArraysEquals (long[] arr1, long[] arr2) {
            if (arr1.Length != arr2.Length)
                return false;
            var res1 = arr1.OrderBy (s => s).ToArray ();
            var res2 = arr2.OrderBy (s => s).ToArray ();
            for (var i = 0; i < res1.Length; i++) {
                if (res1[i] != res2[i]) return false;
            }
            return true;
        }
    }

}