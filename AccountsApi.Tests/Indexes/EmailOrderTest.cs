using NUnit.Framework;
using AccountsApi.Models;
using Newtonsoft.Json;
using AccountsApi.Infrastructure;
using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure.Indexes;

namespace Tests
{
    public class EmailOrderTest
    {

        [Test]
        public void EmailLtTest()
        {
            var target = new EmailOrderIndex("email");
            target.Put(new Account{ Id = 1, EMail = "1111@mail.ru"});
            target.Put(new Account{ Id = 2, EMail = "2222@mail.ru"});
            target.Put(new Account{ Id = 3, EMail = "3333@mail.ru"});
            target.Put(new Account{ Id = 4, EMail = "4444@mail.ru"});
            target.Put(new Account{ Id = 5, EMail = "5555@mail.ru"});

            var actual = target.GetIds("3333@mail.ru", Predicate.lt);
            Assert.AreEqual(3, actual.Count());
            Assert.AreEqual(1, actual.OrderBy(l => l).ToArray()[0]);
            Assert.AreEqual(2, actual.OrderBy(l => l).ToArray()[1]);
            Assert.AreEqual(3, actual.OrderBy(l => l).ToArray()[2]);
        }

        [Test]
        public void EmailGtTest()
        {
            var target = new EmailOrderIndex("email");
            target.Put(new Account{ Id = 1, EMail = "1111@mail.ru"});
            target.Put(new Account{ Id = 2, EMail = "2222@mail.ru"});
            target.Put(new Account{ Id = 3, EMail = "3333@mail.ru"});
            target.Put(new Account{ Id = 4, EMail = "4444@mail.ru"});
            target.Put(new Account{ Id = 5, EMail = "5555@mail.ru"});

            var actual = target.GetIds("3333@mail.ru", Predicate.gt);
            Assert.AreEqual(2, actual.Count());
            Assert.AreEqual(4, actual.OrderBy(l => l).ToArray()[0]);
            Assert.AreEqual(5, actual.OrderBy(l => l).ToArray()[1]);
        }
    }
}