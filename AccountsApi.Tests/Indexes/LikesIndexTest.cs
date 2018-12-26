using NUnit.Framework;
using AccountsApi.Models;
using Newtonsoft.Json;
using AccountsApi.Infrastructure;
using System.Linq;
using AccountsApi.Index.Infrastructure;
using AccountsApi.Infrastructure.Indexes;

namespace Tests
{
    public class LikesIndexTest
    {

        
        [Test]
        public void LikesIndexContainsTest()
        {
            var target = new LikesIndex("likes");
            target.Put(new Account{ Id = 1, Likes = new LikeModel[] {new LikeModel {Id = 1, Ts = 11}, new LikeModel {Id = 2, Ts = 11}, new LikeModel {Id = 3, Ts = 11}}});
            target.Put(new Account{ Id = 2, Likes = new LikeModel[] {new LikeModel {Id = 1, Ts = 11}, new LikeModel {Id = 2, Ts = 11}, new LikeModel {Id = 3, Ts = 11}}});
            target.Put(new Account{ Id = 3, Likes = new LikeModel[] {new LikeModel {Id = 1, Ts = 11}, new LikeModel {Id = 2, Ts = 11}, new LikeModel {Id = 3, Ts = 11}}});
            target.Put(new Account{ Id = 4, Likes = new LikeModel[] {new LikeModel {Id = 4, Ts = 11}, new LikeModel {Id = 5, Ts = 11}, new LikeModel {Id = 6, Ts = 11}}});
            target.Put(new Account{ Id = 5, Likes = new LikeModel[] {new LikeModel {Id = 7, Ts = 11}, new LikeModel {Id = 8, Ts = 11}, new LikeModel {Id = 9, Ts = 11}}});
            

            var actual = target.GetIds("1,2,3", Predicate.contains);
            Assert.AreEqual(3, actual.Count());
            Assert.AreEqual(1, actual.OrderBy(l => l).ToArray()[0]);
            Assert.AreEqual(2, actual.OrderBy(l => l).ToArray()[1]);
            Assert.AreEqual(3, actual.OrderBy(l => l).ToArray()[2]);

            actual = target.GetIds("1,2,7", Predicate.contains);
            Assert.AreEqual(0, actual.Count());

            actual = target.GetIds("4,5", Predicate.contains);
            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(4, actual.OrderBy(l => l).ToArray()[0]);
        }
    }
}