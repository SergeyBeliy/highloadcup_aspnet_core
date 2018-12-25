using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AccountsApi.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ILogger = AccountsApi.Infrastructure.ILogger;

namespace AccountsApi {
    public class Startup {
        private string InitDataPath = "../tmp/data/data.zip";
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddMvc().SetCompatibilityVersion (CompatibilityVersion.Version_2_2);
            services.AddTransient(typeof(ILogger), typeof(Logger));
            services.AddSingleton(typeof(IDatabase), GetDatabaseService(new Logger(), new Indexer()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }

            app.UseHttpsRedirection ();
            app.UseMvc ();
        }

        private IDatabase GetDatabaseService(ILogger logger, IIndexer indexer){
            var database = new Database(logger, indexer);
            database.InitData(InitDataPath);
            return database;
        }
    }
}