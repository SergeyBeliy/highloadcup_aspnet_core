using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AccountsApi.Infrastructure;
using AccountsApi.Infrastructure.Database;
using AccountsApi.Infrastructure.Helpers;
using AccountsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using PostgreSQLCopyHelper;

namespace AccountsApi.Database.Infrastructure {
    public class Database : IDatabase {
        private const string UnzipperFolderName = "unzippedData";
        private long LikeIdCounter = 1;
        private long PremiumIdCounter = 1;

        // private ConcurrentDictionary<long, Account> _accounts = new ConcurrentDictionary<long, Account> ();

        private ILogger Logger { get; set; }

        private AccountContext AccountContext { get; set; }
        public Database (ILogger logger, AccountContext accountContext) {
            Logger = logger;
            AccountContext = accountContext;
        }
        #region Initialization

        public void InitData (string initialDataPath) {
            Logger.Debug ($"Initializing...");
            UnzipFile (initialDataPath);
            ParseFiles ();
            Logger.Debug ($"Initialization complete");
        }

        private void UnzipFile (string zipPath) {
            Logger.Debug ($"Try unzip file: {zipPath}");
            Logger.Debug ($"Current directory: { Directory.GetCurrentDirectory()}");
            var items = Directory.EnumerateFileSystemEntries (Directory.GetCurrentDirectory ());
            foreach (var item in items) {
                Logger.Debug ($"{item}");
            }
            try {
                if (Directory.Exists (UnzipperFolderName))
                    Directory.Delete (UnzipperFolderName, true);
                ZipFile.ExtractToDirectory (zipPath, UnzipperFolderName);
                Logger.Debug ($"Unzip success");
            } catch (Exception ex) {
                Logger.Error ("Unzip data error", ex);
            }
        }

        private void ParseFiles () {
            Logger.Debug ($"Try parse files: {UnzipperFolderName} ...");
            var files = Directory.GetFiles (UnzipperFolderName, "*.json");
            var tasks = new List<Task> ();
            foreach (var file in files) {
                try {
                    Logger.Debug ($"Try parse file: {file}");
                    var accountsData = JsonConvert.DeserializeObject<AccountsData> (File.ReadAllText (file));
                    Logger.Debug ($"File {file} parsed success. {accountsData?.Accounts?.LongLength} accounts found.");
                    BatchInsert (accountsData.Accounts);
                    Logger.Debug ($"File {file} pushed to database.");
                } catch (Exception ex) {
                    Logger.Error ($"Parse file {file} error", ex);
                }
            }

            Logger.Debug ($"Parse files: {UnzipperFolderName} success");

        }

        private void BatchInsert (IEnumerable<Account> accounts) {
            var helperAccounts = new PostgreSQLCopyHelper<Account> ("public", "accounts")
                .MapBigInt ("id", x => x.Id)
                .MapText ("sname", x => x.SName)
                .MapText ("fname", x => x.FName)
                .MapText ("country", x => x.Country)
                .MapText ("city", x => x.City)
                .MapText ("phone", x => x.Phone)
                .MapText ("email", x => x.EMail)
                .MapInteger ("sex", x => (int?) x.Sex)
                .MapTimeStamp ("birth", x => x.Birth)
                .MapTimeStamp ("joined", x => x.Joined)
                .MapText ("status", x => x.Status)
                .MapNullable ("premium_start", x => x.PremiumStart, NpgsqlDbType.Bigint)
                .MapNullable ("premium_finish", x => x.PremiumFinish, NpgsqlDbType.Bigint)
                .MapArray ("interests", x => x.Interests)
                .MapArray ("like_ids", x => x.LikeIds)
                .MapArray ("like_tss", x => x.LikeTSs);

            using (var connection = new NpgsqlConnection ("Host=localhost;Port=5432;Username=accountsdb_user;Password=Tester01;Database=accountsDB;")) {
                connection.Open ();
                helperAccounts.SaveAll (connection, accounts);
            }

        }

        #endregion

        public async Task Put (Account account) {
            AccountContext.Accounts.Add (account);
            await AccountContext.SaveChangesAsync ();
        }

        public IEnumerable<Account> FilterQuery (FilterQuery query) {
            IEnumerable<Account> accountsQuery = AccountContext.Accounts;

            accountsQuery = AddQueryItems (accountsQuery, query);

            return accountsQuery.OrderByDescending (s => s.Id).Take (query.Limit);
        }

        public IEnumerable<AccountGroup> GroupQuery (GroupQuery query) {
            IEnumerable<Account> accountsQuery = AccountContext.Accounts;

            accountsQuery = AddQueryItems (accountsQuery, query);

            IEnumerable<AccountGroup> groups;
            switch (query.Keys) {
                case "sex":
                    groups = accountsQuery.GroupBy (s => s.Sex).Select (s => new AccountGroup {
                        Count = s.Count (),
                            Sex = s.Key.ToString ().ToLower ()
                    });
                    break;
                case "status":
                    groups = accountsQuery.Where (s => s.Status != null).GroupBy (s => s.Status).Select (s => new AccountGroup {
                        Count = s.Count (),
                            Status = s.Key?.ToString ().ToLower ()
                    });
                    break;
                case "country":
                    groups = accountsQuery.Where (s => s.Country != null).GroupBy (s => s.Country).Select (s => new AccountGroup {
                        Count = s.Count (),
                            Country = s.Key?.ToString ().ToLower ()
                    });
                    break;
                case "city":
                    groups = accountsQuery.Where (s => s.City != null).GroupBy (s => s.City).Select (s => new AccountGroup {
                        Count = s.Count (),
                            City = s.Key?.ToString ().ToLower ()
                    });
                    break;
                case "interests":
                    groups = accountsQuery.Where (s => s.Interests != null).SelectMany (s => s.Interests).GroupBy (s => s).Select (s => new AccountGroup {
                        Count = s.Count (),
                            Interests = s.Key?.ToString ().ToLower ()
                    });
                    break;
                default:
                    throw new Exception ($"Unsupported keys: {query.Keys}");
            }
            if (query.Order == 1) {
                groups = groups.OrderBy (s => s.Count);
            } else {
                groups = groups.OrderByDescending (s => s.Count);
            }
            return groups.Take (query.Limit);
        }

        public IEnumerable<Recommendation> RecommendQuery (RecommendQuery query) {
            var account = AccountContext.Accounts.FirstOrDefault (s => s.Id == query.AccountId);
            if (account == null) {
                return null;
            }
            IEnumerable<Account> accountsQuery = AccountContext.Accounts;

            accountsQuery = AddQueryItems (accountsQuery, query);
            accountsQuery = accountsQuery.Where (s => s.Sex != account.Sex);
            var now = SecondEpochConverter.ConvertTo (DateTime.UtcNow);
            var recommendation = accountsQuery.Select (a => new {
                    Account = a,
                        Premium = ((a.PremiumStart??long.MaxValue) <= now && ((a.PremiumFinish??0) >= now)) ? 1000000000 : 0,
                        Status = GetStatusCount (a.Status),
                        Interests = InterestsComp (a.Interests, account.Interests),
                        AgeDif = Math.Abs ((account.Birth - a.Birth).TotalSeconds)
                })
                .Where (r => r.Interests > 0)
                .OrderByDescending (r => r.Premium)
                .ThenByDescending (r => r.Status)
                .ThenByDescending (r => r.Interests)
                .ThenBy (r => r.AgeDif)
                .ThenBy (r => r.Account.Id)
                .Select (r => new Recommendation (r.Account))
                .Take (query.Limit);
            return recommendation;
        }

        public IEnumerable<Suggestion> SuggestQuery (SuggestQuery query) {
            var account = AccountContext.Accounts.FirstOrDefault (s => s.Id == query.AccountId);
            if (account == null) {
                return null;
            }
            IEnumerable<Account> accountsQuery = AccountContext.Accounts;

            accountsQuery = AddQueryItems (accountsQuery, query);
            accountsQuery = accountsQuery.Where (s => s.Sex == account.Sex);
            var allLikes = accountsQuery.Select (a => new {
                    Account = a,
                        Similarity = GetSimilarity (account, a),
                })
                .OrderByDescending (s => s.Similarity)
                .SelectMany (s => s.Account.LikeIds.OrderBy (f => f))
                .Where (s => !account.LikeIds.Contains (s))
                .Take (query.Limit).ToArray();
            return AccountContext.Accounts.Where(a => allLikes.Contains(a.Id)).Select(a => new Suggestion(a));
        }

        private float GetSimilarity (Account a1, Account a2) {
            if (a1.LikeIds == null)
                return 0;
            if (a2.LikeIds == null)
                return 0;
            var dict1 = GetLikesDictionary (a1);
            var dict2 = GetLikesDictionary (a2);
            var matchLikes = dict1.Keys.Intersect (dict2.Keys);
            return matchLikes.Select (s => 1 / Math.Abs (dict1[s] - dict2[s])).Sum ();
        }

        private Dictionary<long, long> GetLikesDictionary (Account account) {
            var dic = new Dictionary<long, long> ();
            if (account.LikeIds != null) {
                for (int i = 0; i < account.LikeIds.Length; i++) {
                    if (dic.ContainsKey (account.LikeIds[i])) {
                        dic[account.LikeIds[i]] = (dic[account.LikeIds[i]] + account.LikeTSs[i]) / 2;
                    } else
                        dic[account.LikeIds[i]] = account.LikeTSs[i];
                }
            }
            return dic;
        }

        private long GetStatusCount (string status) {
            if (status == "свободны")
                return 900000000;
            if (status == "всё сложно")
                return 800000000;
            return 700000000;
        }

        private long InterestsComp (string[] interests1, string[] interests2) {
            if (interests1 == null) return 0;
            if (interests2 == null) return 0;
            return interests1.Intersect (interests2).Count () * 100000;
        }
        private IEnumerable<Account> AddQueryItem (IEnumerable<Account> accounts, QueryItem queryItem) {
            switch (queryItem.FieldName) {

                case "sex":
                    return AddSexQuery (accounts, queryItem);
                case "email":
                    return AddEmailQuery (accounts, queryItem);
                case "status":
                    return AddStatusQuery (accounts, queryItem);
                case "fname":
                    return AddFNameQuery (accounts, queryItem);
                case "sname":
                    return AddSNameQuery (accounts, queryItem);
                case "phone":
                    return AddPhoneQuery (accounts, queryItem);
                case "country":
                    return AddCountryQuery (accounts, queryItem);
                case "city":
                    return AddCityQuery (accounts, queryItem);
                case "birth":
                    return AddBirthQuery (accounts, queryItem);
                case "joined":
                    return AddJoinedQuery (accounts, queryItem);
                case "interests":
                    return AddInterestsQuery (accounts, queryItem);
                case "likes":
                    return AddLikesQuery (accounts, queryItem);
                case "premium":
                    return AddPremiumQuery (accounts, queryItem);
                default:
                    throw new Exception ("Unsupported field");
            }
            // return accounts;
        }

        private IEnumerable<Account> AddSexQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            SexEnum sex;
            if (!SexEnum.TryParse (queryItem.Value.ToLower (), out sex)) {
                throw new Exception ($"Wrong value {queryItem.Value}");
            }
            var predicate = queryItem.Predicate??Predicate.eq;
            switch (predicate) {
                case Predicate.eq:
                    return accounts.Where (a => a.Sex == sex);
                case Predicate.neq:
                    return accounts.Where (a => a.Sex != sex);
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddEmailQuery (IEnumerable<Account> accounts, QueryItem queryItem) {

            var predicate = queryItem.Predicate??Predicate.eq;
            switch (predicate) {
                case Predicate.eq:
                    return accounts.Where (a => string.Equals (a.EMail, queryItem.Value, StringComparison.OrdinalIgnoreCase));
                case Predicate.domain:
                    return accounts.Where (a => EF.Functions.Like (a.EMail, $"%@{queryItem.Value}"));
                case Predicate.gt:
                    return accounts.Where (a => string.Compare (a.EMail, queryItem.Value) > 0);
                case Predicate.lt:
                    return accounts.Where (a => string.Compare (a.EMail, queryItem.Value) < 0);
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddStatusQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            var predicate = queryItem.Predicate??Predicate.eq;
            switch (predicate) {
                case Predicate.eq:
                    return accounts.Where (a => a.Status == queryItem.Value);
                case Predicate.neq:
                    return accounts.Where (a => a.Status != queryItem.Value);
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddFNameQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            var predicate = queryItem.Predicate??Predicate.eq;
            switch (predicate) {
                case Predicate.eq:
                    return accounts.Where (a => a.FName == queryItem.Value);
                case Predicate.@null:
                    if (queryItem.Value == "1")
                        return accounts.Where (a => string.IsNullOrEmpty (a.FName));
                    if (queryItem.Value == "0")
                        return accounts.Where (a => !string.IsNullOrEmpty (a.FName));
                    throw new Exception ($"Wrong value {queryItem.Value}");
                case Predicate.any:
                    var values = queryItem.Value.Split (",");
                    return accounts.Where (a => values.Contains (a.FName));
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddSNameQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            var predicate = queryItem.Predicate??Predicate.eq;
            switch (predicate) {
                case Predicate.eq:
                    return accounts.Where (a => a.SName == queryItem.Value);
                case Predicate.@null:
                    if (queryItem.Value == "1")
                        return accounts.Where (a => string.IsNullOrEmpty (a.SName));
                    if (queryItem.Value == "0")
                        return accounts.Where (a => !string.IsNullOrEmpty (a.SName));
                    throw new Exception ($"Wrong value {queryItem.Value}");
                case Predicate.starts:
                    return accounts.Where (a => EF.Functions.Like (a.SName, $"{queryItem.Value}%"));
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddPhoneQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            var predicate = queryItem.Predicate??Predicate.eq;
            switch (predicate) {
                case Predicate.@null:
                    if (queryItem.Value == "1")
                        return accounts.Where (a => string.IsNullOrEmpty (a.Phone));
                    if (queryItem.Value == "0")
                        return accounts.Where (a => !string.IsNullOrEmpty (a.Phone));
                    throw new Exception ($"Wrong value {queryItem.Value}");
                case Predicate.code:
                    return accounts.Where (a => EF.Functions.Like (a.Phone, $"%({queryItem.Value})%"));
                case Predicate.eq:
                    return accounts.Where (a => string.Equals (a.Phone, queryItem.Value, StringComparison.OrdinalIgnoreCase));
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddCountryQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            var predicate = queryItem.Predicate??Predicate.eq;
            switch (predicate) {
                case Predicate.eq:
                    return accounts.Where (a => a.Country == queryItem.Value);
                case Predicate.@null:
                    if (queryItem.Value == "1")
                        return accounts.Where (a => string.IsNullOrEmpty (a.Country));
                    if (queryItem.Value == "0")
                        return accounts.Where (a => !string.IsNullOrEmpty (a.Country));
                    throw new Exception ($"Wrong value {queryItem.Value}");
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddCityQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            var predicate = queryItem.Predicate??Predicate.eq;
            switch (predicate) {
                case Predicate.eq:
                    return accounts.Where (a => a.City == queryItem.Value);
                case Predicate.@null:
                    if (queryItem.Value == "1")
                        return accounts.Where (a => string.IsNullOrEmpty (a.City));
                    if (queryItem.Value == "0")
                        return accounts.Where (a => !string.IsNullOrEmpty (a.City));
                    throw new Exception ($"Wrong value {queryItem.Value}");
                case Predicate.any:
                    var values = queryItem.Value.Split (",");
                    return accounts.Where (a => values.Contains (a.City));
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddBirthQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            long value;
            if (!long.TryParse (queryItem.Value, out value)) {
                throw new Exception ($"Wrong value {queryItem.Value}");
            }
            var predicate = queryItem.Predicate??Predicate.year;
            switch (predicate) {
                case Predicate.gt:
                    var valueDate = SecondEpochConverter.ConvertFrom (value);
                    return accounts.Where (a => a.Birth > valueDate);
                case Predicate.lt:
                    var valueDate1 = SecondEpochConverter.ConvertFrom (value);
                    return accounts.Where (a => a.Birth < valueDate1);
                case Predicate.year:
                    return accounts.Where (a => a.Birth.Year == (int) value);
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddJoinedQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            long value;
            if (!long.TryParse (queryItem.Value, out value)) {
                throw new Exception ($"Wrong value {queryItem.Value}");
            }
            var predicate = queryItem.Predicate??Predicate.year;
            switch (predicate) {
                case Predicate.gt:
                    var valueDate = SecondEpochConverter.ConvertFrom (value);
                    return accounts.Where (a => a.Joined > valueDate);
                case Predicate.lt:
                    var valueDate1 = SecondEpochConverter.ConvertFrom (value);
                    return accounts.Where (a => a.Joined < valueDate1);
                case Predicate.year:
                    return accounts.Where (a => a.Joined.Year == (int) value);
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddInterestsQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            var values = queryItem.Value.Split (",");
            var predicate = queryItem.Predicate??Predicate.any;
            switch (predicate) {
                case Predicate.any:
                    return accounts.Where (a => a.Interests != null && values.Any (v => a.Interests.Any (i => i == v)));
                case Predicate.contains:
                    return accounts.Where (a => a.Interests != null && values.All (v => a.Interests.Contains (v)));
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddLikesQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            var values = queryItem.Value.Split (",").Select (v => {
                long id;
                if (!long.TryParse (v, out id)) {
                    throw new Exception ($"Wrong value {queryItem.Value}");
                }
                return id;
            });
            var predicate = queryItem.Predicate??Predicate.contains;
            switch (predicate) {
                case Predicate.contains:
                    return accounts.Where (a => a.LikeIds != null && values.All (v => a.LikeIds.Contains (v)));
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddPremiumQuery (IEnumerable<Account> accounts, QueryItem queryItem) {

            switch (queryItem.Predicate) {
                case Predicate.@null:
                    if (queryItem.Value == "1")
                        return accounts.Where (a => a.PremiumStart == null);
                    if (queryItem.Value == "0")
                        return accounts.Where (a => a.PremiumStart != null);
                    throw new Exception ($"Wrong value {queryItem.Value}");

                case Predicate.now:
                    var now = SecondEpochConverter.ConvertTo (DateTime.UtcNow);
                    return accounts.Where (a => a.PremiumStart != null && a.PremiumFinish != null && a.PremiumStart <= now && a.PremiumFinish >= now);
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddQueryItems (IEnumerable<Account> accountsQuery, QueryBase query) {
            foreach (var queryItem in query.Items) {
                accountsQuery = AddQueryItem (accountsQuery, queryItem);
            }
            return accountsQuery;

        }
    }
}