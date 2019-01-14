using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountsApi.Database.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AccountsApi
{
    public static class Program
    {
        private const string InitDataPath = "../tmp/data/data.zip";
        public static void Main(string[] args)
        {
            // Process.Start("../etc/init.d/postgresql", "start");
            // Thread.Sleep(30000);
            CreateWebHostBuilder(args).Build()
            .InitData()
            .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();

        public static IWebHost InitData(this IWebHost webhost)
        {
            // string[] allfiles = Directory.GetFiles("./", "*.*", SearchOption.AllDirectories);
            // foreach (var d in allfiles)
            // {
            //     Console.WriteLine(d);
            // }
            using (var scope = webhost.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IDatabase>();
                db.InitData(InitDataPath);
                return webhost;
            }
        }
    }
}