using System.Reflection;
using Core.OptionBinders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Repository;
using FluentMigrator.Runner;
using Microsoft.Extensions.Logging;

namespace ManBrewingApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
            services.AddLogging(l =>
            {
                l.AddApplicationInsights(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
            });

            services.Configure<ConnectionStrings>(Configuration.GetSection(nameof(ConnectionStrings)));
            services.AddSingleton<IDataLogService, DataLogService>();
            services.AddSingleton<IOpenWeatherMapService, OpenWeatherMapService>();

            services.AddControllers();

            // generate swagger API documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Man Brewing API", Version = "1.0.0"});
            });

            services.AddFluentMigratorCore()
                .ConfigureRunner(runner =>
                {
                    runner.AddSqlServer()
                        .WithGlobalConnectionString(Configuration["SqlConnectionString"])
                        .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations();
                })
                .AddLogging(l => l.AddFluentMigratorConsole());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();

            // base url should show swagger docs
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Man Brewing API");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // perform any outstanding database migrations
            using var scope = app.ApplicationServices.CreateScope();
            var runner = scope.ServiceProvider.GetService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}
