using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure;
using AccountsApi.Infrastructure.Indexes;
using AccountsApi.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests {
    public class EmailDomainTest {

        [Test]
        public void EmailDomainGetTest () {
            var target = new EmailDomainIndex ("email");
            target.Put (new Account { Id = 1, EMail = "1111@mail.ru" });
            target.Put (new Account { Id = 2, EMail = "2222@list.ru" });
            target.Put (new Account { Id = 3, EMail = "3333@live.ru" });
            target.Put (new Account { Id = 4, EMail = "4444@mail.ru" });
            target.Put (new Account { Id = 5, EMail = "5555@mail.ru" });

            var actual = target.GetIds ("mail.ru", Predicate.domain);
            Assert.AreEqual (3, actual.Count ());
            Assert.AreEqual (1, actual.OrderBy (l => l).ToArray () [0]);
            Assert.AreEqual (4, actual.OrderBy (l => l).ToArray () [1]);
            Assert.AreEqual (5, actual.OrderBy (l => l).ToArray () [2]);

            actual = target.GetIds ("notexists.ru", Predicate.domain);
            Assert.AreEqual (0, actual.Count ());
        }
    }

}