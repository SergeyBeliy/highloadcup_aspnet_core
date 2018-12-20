using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using AccountsApi.Infrastructure;
using AccountsApi.Models;
using Newtonsoft.Json;

namespace AccountsApi.Infrastructure {
    public class Database : IDatabase {
        private const string UnzipperFolderName = "unzippedData";
        private bool _isInit = false;

        private ConcurrentDictionary<long, Account> _accounts = new ConcurrentDictionary<long, Account> ();

        private ILogger Logger { get; set; }
        public Database (ILogger logger) {
            Logger = logger;
        }

        public void InitData (string initialDataPath) {
            Logger.Debug ($"Initializing...");
            Task.Run (() => {
                UnzipFile (initialDataPath);
                ParseFiles ();
            }).ContinueWith ((res) => {
                _isInit = true;
                Logger.Debug ($"Initialization complete");
            });
        }

        private void UnzipFile (string zipPath) {
            Logger.Debug ($"Try unzip file: {zipPath}");
            Logger.Debug ($"Current directory: { Directory.GetCurrentDirectory()}");
            var items = Directory.EnumerateFileSystemEntries(Directory.GetCurrentDirectory());
            foreach(var item in items) {
                Logger.Debug($"{item}");
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
            foreach (var file in files) {
                try {
                    Logger.Debug ($"Try parse file: {file}");
                    //var serializer = new JavaScriptSerializer();
                    var accountsData = JsonConvert.DeserializeObject<AccountsData> (File.ReadAllText (file));
                    Logger.Debug ($"File {file} parsed success. {accountsData?.Accounts?.LongLength} accounts found.");
                    foreach (var account in accountsData.Accounts) {
                        _accounts[account.Id] = account;
                    }
                } catch (Exception ex) {
                    Logger.Error ($"Parse file {file} error", ex);
                }
            }

            Logger.Debug ($"Parse files: {UnzipperFolderName} success");

        }

    }
}