using NUnit.Framework;
using AccountsApi.Models;
using Newtonsoft.Json;
using AccountsApi.Infrastructure;
using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure.Indexes;

namespace Tests
{
    public class StringArrayIndexTest
    {

        [Test]
        public void StringArrayAnyTest()
        {
            var target = new StringArrayIndex("Interests");
            target.Put(new Account{ Id = 1, Interests = new string[] {"1", "2", "3"}});
            target.Put(new Account{ Id = 2, Interests = new string[] {"2", "3", "4"}});
            target.Put(new Account{ Id = 3, Interests = new string[] {"3", "4", "5"}});
            target.Put(new Account{ Id = 4, Interests = new string[] {"4", "5", "6"}});
            target.Put(new Account{ Id = 5, Interests = new string[] {"5", "6", "7"}});

            var actual = target.GetIds("1,2,3", Predicate.any);
            Assert.AreEqual(3, actual.Count());
            Assert.AreEqual(1, actual.OrderBy(l => l).ToArray()[0]);
            Assert.AreEqual(2, actual.OrderBy(l => l).ToArray()[1]);
            Assert.AreEqual(3, actual.OrderBy(l => l).ToArray()[2]);
        }

        [Test]
        public void StringArrayContainsTest()
        {
            var target = new StringArrayIndex("Interests");
            target.Put(new Account{ Id = 1, Interests = new string[] {"1", "2", "3"}});
            target.Put(new Account{ Id = 2, Interests = new string[] {"2", "3", "4"}});
            target.Put(new Account{ Id = 3, Interests = new string[] {"3", "4", "5"}});
            target.Put(new Account{ Id = 4, Interests = new string[] {"4", "5", "6"}});
            target.Put(new Account{ Id = 5, Interests = new string[] {"5", "6", "7"}});

            var actual = target.GetIds("2,3", Predicate.contains);
            Assert.AreEqual(2, actual.Count());
            Assert.AreEqual(1, actual.OrderBy(l => l).ToArray()[0]);
            Assert.AreEqual(2, actual.OrderBy(l => l).ToArray()[1]);
        }
    }
}