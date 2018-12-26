using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure;
using AccountsApi.Infrastructure.Indexes;
using AccountsApi.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests {
    public class PremiumIndexTest {

        [Test]
        public void PremiumIndexNowTest () {
            var target = new PremiumIndex ("likes");
            target.Put (new Account { Id = 1, Premium = new PremiumModel { Start = 1514764800, Finish = 1577394064 } });
            target.Put (new Account { Id = 2, Premium = new PremiumModel { Start = 1514764800, Finish = 1577394064 } });
            target.Put (new Account { Id = 3, Premium = new PremiumModel { Start = 1514764800, Finish = 1577394064 } });
            target.Put (new Account { Id = 4, Premium = new PremiumModel { Start = 1514764800, Finish = 1545858223 } });
            target.Put (new Account { Id = 5, Premium = new PremiumModel { Start = 1514764800, Finish = 1545858223 } });

            //1545858230
            var actual = target.GetIds (string.Empty, Predicate.now);
            Assert.AreEqual (3, actual.Count ());
            Assert.AreEqual (1, actual.OrderBy (l => l).ToArray () [0]);
            Assert.AreEqual (2, actual.OrderBy (l => l).ToArray () [1]);
            Assert.AreEqual (3, actual.OrderBy (l => l).ToArray () [2]);

        }

        public void PremiumIndexNullTest () {
            var target = new PremiumIndex ("likes");
            target.Put (new Account { Id = 1 });
            target.Put (new Account { Id = 2 });
            target.Put (new Account { Id = 3, Premium = new PremiumModel { Start = 1514764800, Finish = 1577394064 } });
            target.Put (new Account { Id = 4, Premium = new PremiumModel { Start = 1514764800, Finish = 1545858223 } });
            target.Put (new Account { Id = 5, Premium = new PremiumModel { Start = 1514764800, Finish = 1545858223 } });

            //1545858230
            var actual = target.GetIds ("1", Predicate.now);
            Assert.AreEqual (2, actual.Count ());
            Assert.AreEqual (1, actual.OrderBy (l => l).ToArray () [0]);
            Assert.AreEqual (2, actual.OrderBy (l => l).ToArray () [1]);

            actual = target.GetIds ("0", Predicate.now);
            Assert.AreEqual (3, actual.Count ());
            Assert.AreEqual (3, actual.OrderBy (l => l).ToArray () [0]);
            Assert.AreEqual (4, actual.OrderBy (l => l).ToArray () [1]);
            Assert.AreEqual (5, actual.OrderBy (l => l).ToArray () [2]);
            

        }
    }
}