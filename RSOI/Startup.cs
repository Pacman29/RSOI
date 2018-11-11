﻿using JobExecutor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RSOI.Jobs;
using RSOI.Services;
using RSOI.Services.Impl;

namespace RSOI
{
    public class Startup
    {
        public Startup()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = configurationBuilder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            ConfigureDependencyInjection(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
        
        public virtual void ConfigureDependencyInjection(IServiceCollection services)
        {            
            
            var dbService = new DataBaseService("localhost:8080");
            services.AddSingleton<IDataBaseService>(dbService);
            var recognizeService = new RecognizeService("localhost:8081");
            services.AddSingleton<IRecognizeService>(recognizeService);
            var fileService = new FileService("localhost:8082");
            services.AddSingleton<IFileService>(fileService);

            var executor = JobExecutor.JobExecutor.Instance;
            services.AddSingleton<IJobExecutor>(executor);

            services.AddSingleton<IManagerService, ManagerService>();
            services.AddSingleton<IGateWayJobsFabric, GateWayJobsFabric>();
        }

    }
}