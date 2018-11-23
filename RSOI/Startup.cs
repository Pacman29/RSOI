using AuthOptions;
using JobExecutor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
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
			services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            ConfigureDependencyInjection(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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
			app.UseCors(builder => builder
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials());  
            
            app.UseAuthentication();
            
            app.UseMvc();
            loggerFactory.AddProvider(new ConsoleLoggerProvider(
                (text, logLevel) => logLevel >= LogLevel.Information , true));
            loggerFactory.AddDebug();
            loggerFactory.AddConsole();
        }
        
        public virtual void ConfigureDependencyInjection(IServiceCollection services)
        {            
            SetupAuthServices.Setup(services,Configuration);
            
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