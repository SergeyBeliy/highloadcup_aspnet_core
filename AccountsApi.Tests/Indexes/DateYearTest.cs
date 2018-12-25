using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure;
using AccountsApi.Infrastructure.Indexes;
using AccountsApi.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests {
    public class DateYearTest {

        [Test]
        public void DateYearGetTest () {
            var target = new DateYearIndex ("Birth");
            target.Put (new Account { Id = 1, Birth = 347155200 });
            target.Put (new Account { Id = 2, Birth = 567993600 });
            target.Put (new Account { Id = 3, Birth = -631152000 });
            target.Put (new Account { Id = 4, Birth = 347155200 });
            target.Put (new Account { Id = 5, Birth = 347155200 });

            var actual = target.GetIds ("1981", Predicate.year);
            Assert.AreEqual (3, actual.Count ());
            Assert.AreEqual (1, actual.OrderBy (l => l).ToArray () [0]);
            Assert.AreEqual (4, actual.OrderBy (l => l).ToArray () [1]);
            Assert.AreEqual (5, actual.OrderBy (l => l).ToArray () [2]);

            actual = target.GetIds ("1950", Predicate.year);
            Assert.AreEqual (1, actual.Count ());
            Assert.AreEqual (3, actual.OrderBy (l => l).ToArray () [0]);

            actual = target.GetIds ("1988", Predicate.year);
            Assert.AreEqual (1, actual.Count ());
            Assert.AreEqual (2, actual.OrderBy (l => l).ToArray () [0]);

            actual = target.GetIds ("1999", Predicate.year);
            Assert.AreEqual (0, actual.Count ());
        }
    }

}