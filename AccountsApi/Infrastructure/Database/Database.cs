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
                .MapJson ("premium", x => x.PremiumJson)
                .MapArray ("interests", x => x.Interests)
                .MapArray ("likes", x => x.LikesJson, NpgsqlDbType.Json);

            using (var connection = new NpgsqlConnection ("Host=localhost;Port=5432;Username=accountsdb_user;Password=Tester01;Database=accountsDB;")) {
                connection.Open ();
                helperAccounts.SaveAll (connection, accounts);
            }

        }

        public async Task Put (Account account) {
            AccountContext.Accounts.Add (account);
            await AccountContext.SaveChangesAsync ();
        }

        public IEnumerable<Account> Query (Query query) {
            IEnumerable<Account> accountsQuery = AccountContext.Accounts;

            foreach (var queryItem in query.Items) {
                accountsQuery = AddQueryItem (accountsQuery, queryItem);
            }

            return accountsQuery.OrderByDescending (s => s.Id).Take (query.Limit);
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
            switch (queryItem.Predicate) {
                case Predicate.eq:
                    return accounts.Where (a => a.Sex == sex);
                case Predicate.neq:
                    return accounts.Where (a => a.Sex != sex);
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddEmailQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            switch (queryItem.Predicate) {
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
            switch (queryItem.Predicate) {
                case Predicate.eq:
                    return accounts.Where (a => a.Status == queryItem.Value);
                case Predicate.neq:
                    return accounts.Where (a => a.Status != queryItem.Value);
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddFNameQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            switch (queryItem.Predicate) {
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
            switch (queryItem.Predicate) {
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
            switch (queryItem.Predicate) {
                case Predicate.@null:
                    if (queryItem.Value == "1")
                        return accounts.Where (a => string.IsNullOrEmpty (a.Phone));
                    if (queryItem.Value == "0")
                        return accounts.Where (a => !string.IsNullOrEmpty (a.Phone));
                    throw new Exception ($"Wrong value {queryItem.Value}");
                case Predicate.code:
                    return accounts.Where (a => EF.Functions.Like (a.Phone, $"%({queryItem.Value})%"));
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddCountryQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            switch (queryItem.Predicate) {
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
            switch (queryItem.Predicate) {
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
            double value;
            if (!Double.TryParse (queryItem.Value, out value)) {
                throw new Exception ($"Wrong value {queryItem.Value}");
            }
            switch (queryItem.Predicate) {
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

        private IEnumerable<Account> AddInterestsQuery (IEnumerable<Account> accounts, QueryItem queryItem) {
            var values = queryItem.Value.Split (",");
            switch (queryItem.Predicate) {
                case Predicate.any:
                    return accounts.Where (a => values.Any (v => a.Interests.Any (i => i == v)));
                case Predicate.contains:
                    return accounts.Where (a => values.All (v => a.Interests.Contains (v)));
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
            switch (queryItem.Predicate) {
                case Predicate.contains:
                    return accounts.Where (a => a.Likes != null && values.All (v => a.Likes.Any (l => l.Id == v)));
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }

        private IEnumerable<Account> AddPremiumQuery (IEnumerable<Account> accounts, QueryItem queryItem) {

            switch (queryItem.Predicate) {
                case Predicate.@null:
                    if (queryItem.Value == "1")
                        return accounts.Where (a => string.IsNullOrEmpty (a.PremiumJson));
                    if (queryItem.Value == "0")
                        return accounts.Where (a => !string.IsNullOrEmpty (a.PremiumJson));
                    throw new Exception ($"Wrong value {queryItem.Value}");

                case Predicate.now:
                    var now = SecondEpochConverter.ConvertTo (DateTime.UtcNow);
                    return accounts.Where (a => a.PremiumJson != null && JsonConvert.DeserializeObject<PremiumModel> (a.PremiumJson).Start <= DateTime.UtcNow && JsonConvert.DeserializeObject<PremiumModel> (a.PremiumJson).Finish >= DateTime.UtcNow);
                default:
                    throw new Exception ($"Unsupportend predicate {queryItem.Predicate}");
            }
        }
    }
}