using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestSharp;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using TvMaze.ConfigSettings;
using TvMaze.DataAccess;
using TvMaze.Interfaces;
using TvMaze.ScraperService;
using TvMaze.TvMazeClient;
using WebApi.HostedService;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebApi
{
    public class Startup
    {
        private const string DateFormatStringSettingsKey = "DateFormatString";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.DateFormatString =
                        Configuration.GetSection(DateFormatStringSettingsKey).Value;
                });

            services.Configure<DbSettings>(options => Configuration.GetSection(nameof(DbSettings)).Bind(options));
            services.Configure<RepositorySettings>(options => Configuration.GetSection(nameof(RepositorySettings)).Bind(options));
            services.Configure<ApiSettings>(options => Configuration.GetSection(nameof(ApiSettings)).Bind(options));
            services.Configure<ScraperSettings>(options => Configuration.GetSection(nameof(ScraperSettings)).Bind(options));
            services.Configure<BackgroundTaskSettings>(options => Configuration.GetSection(nameof(BackgroundTaskSettings)).Bind(options));

            services.AddSingleton<IShowContext, ShowContext>();
            services.AddTransient<IShowRepository, ShowRepository>();
            services.AddTransient<IRestClient, RestClient>();
            services.AddTransient<ITvMazeApiClient, TvMazeApiClient>();
            services.AddTransient<IScraperService, Scraper>();
            services.AddSingleton<IHostedService, ScraperBackgroundRunner>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "DK Shows API", Version = "v1"
                });
                
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}
