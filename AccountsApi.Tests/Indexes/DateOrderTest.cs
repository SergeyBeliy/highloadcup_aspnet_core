using NUnit.Framework;
using AccountsApi.Models;
using Newtonsoft.Json;
using AccountsApi.Infrastructure;
using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure.Indexes;

namespace Tests
{
    public class DateOrderTest
    {

        [Test]
        public void DateLtTest()
        {
            var target = new DateOrderIndex("Birth");
            target.Put(new Account{ Id = 1, Birth = 1111});
            target.Put(new Account{ Id = 2, Birth = 2222});
            target.Put(new Account{ Id = 3, Birth = 3333});
            target.Put(new Account{ Id = 4, Birth = 4444});
            target.Put(new Account{ Id = 5, Birth = 5555});

            var actual = target.GetIds("3333", Predicate.lt);
            Assert.AreEqual(3, actual.Count());
            Assert.AreEqual(1, actual.OrderBy(l => l).ToArray()[0]);
            Assert.AreEqual(2, actual.OrderBy(l => l).ToArray()[1]);
            Assert.AreEqual(3, actual.OrderBy(l => l).ToArray()[2]);
        }

        [Test]
        public void DateGtTest()
        {
            var target = new DateOrderIndex("Birth");
            target.Put(new Account{ Id = 1, Birth = 1111});
            target.Put(new Account{ Id = 2, Birth = 2222});
            target.Put(new Account{ Id = 3, Birth = 3333});
            target.Put(new Account{ Id = 4, Birth = 4444});
            target.Put(new Account{ Id = 5, Birth = 5555});

            var actual = target.GetIds("3333", Predicate.gt);
            Assert.AreEqual(2, actual.Count());
            Assert.AreEqual(4, actual.OrderBy(l => l).ToArray()[0]);
            Assert.AreEqual(5, actual.OrderBy(l => l).ToArray()[1]);
        }
    }
}