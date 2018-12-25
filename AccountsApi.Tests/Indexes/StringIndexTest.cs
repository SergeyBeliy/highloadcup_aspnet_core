using NUnit.Framework;
using AccountsApi.Models;
using Newtonsoft.Json;
using AccountsApi.Infrastructure;
using System.Linq;
using AccountsApi.Index.Infrastructure;

namespace Tests
{
    public class StringIndexTest
    {
        Account _account;
        [SetUp]
        public void Setup()
        {
            var accountJson = @"{
                    ""email"": ""netertitcohyuswe@inbox.com"",
                    ""id"": 10000,
                    ""fname"": ""Ульяна"",
                    ""joined"": 1331596800,                
                    ""likes"": [
                        {
                            ""ts"": 1532388722,
                            ""id"": 3887
                        },
                        {
                            ""ts"": 1497046084,
                            ""id"": 2093
                        },
                        {
                            ""ts"": 1485629830,
                            ""id"": 2699
                        },
                        {
                            ""ts"": 1515509636,
                            ""id"": 499
                        }
                    ],
                    ""sex"": ""f"",
                    ""sname"": ""Пеныкавич"",
                    ""interests"": [
                        ""Ужин с друзьями"",
                        ""Друзья"",
                        ""Пляжный отдых"",
                        ""Друзья и Близкие"",
                        ""Знакомство""
                    ],
                    ""birth"": 739591680
                }";
            _account = JsonConvert.DeserializeObject<Account>(accountJson);
        }

        [Test]
        public void Test1()
        {
            var target = new StringIndex("sname");
            target.Put(_account);
            var actual = target.GetIds("Пеныкавич", Predicate.eq);
            Assert.AreEqual(10000, actual.First());
            
            actual = target.GetIds("not_exists", Predicate.eq);
            Assert.AreEqual(0, actual.Count());

            actual = target.GetIds("not_exists", Predicate.neq);
            Assert.AreEqual(10000, actual.First());

            actual = target.GetIds("Пеныкавич", Predicate.neq);
            Assert.AreEqual(0, actual.Count());

             actual = target.GetIds(string.Empty, Predicate.@null);
            Assert.AreEqual(0, actual.Count());

            actual = target.GetIds("Пеныкавич,not_exists", Predicate.neq);
            Assert.AreEqual(10000, actual.First());

            target = new StringIndex("status");
            target.Put(_account);

            actual = target.GetIds(string.Empty, Predicate.@null);
            Assert.AreEqual(10000, actual.First());
        }
    }
}