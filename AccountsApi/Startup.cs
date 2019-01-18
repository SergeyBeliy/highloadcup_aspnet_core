using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AccountsApi.Database.Infrastructure;
using AccountsApi.Infrastructure;
using AccountsApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Buffering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ILogger = AccountsApi.Infrastructure.ILogger;

namespace AccountsApi {
    public class Startup {

        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2);
            services.AddDbContext<AccountContext> (options =>
                options.UseNpgsql (Configuration.GetConnectionString ("DefaultConnection")));
            services.AddMvc ()
                .AddJsonOptions (options => {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
            services.AddTransient (typeof (ILogger), typeof (Logger));

            services.AddScoped (typeof (IDatabase), typeof (AccountsApi.Database.Infrastructure.Database));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider) {
            app.UseResponseBuffering();
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }

            app.UseHttpsRedirection ();
            app.UseMvc ();
        }

    }
}