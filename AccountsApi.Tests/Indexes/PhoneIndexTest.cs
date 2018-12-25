using NUnit.Framework;
using AccountsApi.Models;
using Newtonsoft.Json;
using AccountsApi.Infrastructure;
using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure.Indexes;

namespace Tests
{
    public class PhoneIndexTest
    {

        [Test]
        public void CodeTest()
        {
            var target = new PhoneIndex("Phone");
            target.Put(new Account{ Id = 1, Phone = "8(926)1112233"});
            target.Put(new Account{ Id = 2, Phone = "8(926)2112233"});
            target.Put(new Account{ Id = 3, Phone = "8(926)3112233"});
            target.Put(new Account{ Id = 4, Phone = "8(926)4112233"});
            target.Put(new Account{ Id = 5, Phone = "8(926)5112233"});
            target.Put(new Account{ Id = 6, Phone = "8(918)5112233"});
            target.Put(new Account{ Id = 7, Phone = "8(964)5112233"});

            var actual = target.GetIds("926", Predicate.code);
            Assert.AreEqual(5, actual.Count());
            Assert.AreEqual(1, actual.OrderBy(l => l).ToArray()[0]);
            Assert.AreEqual(2, actual.OrderBy(l => l).ToArray()[1]);
            Assert.AreEqual(3, actual.OrderBy(l => l).ToArray()[2]);
            Assert.AreEqual(4, actual.OrderBy(l => l).ToArray()[3]);
            Assert.AreEqual(5, actual.OrderBy(l => l).ToArray()[4]);

            actual = target.GetIds("918", Predicate.code);
            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(6, actual.OrderBy(l => l).ToArray()[0]);

            actual = target.GetIds("964", Predicate.code);
            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(7, actual.OrderBy(l => l).ToArray()[0]);

            actual = target.GetIds("111", Predicate.code);
            Assert.AreEqual(0, actual.Count());
            
        }

        
    }
}