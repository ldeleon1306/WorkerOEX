using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Workers;

namespace WorkerOE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
           .Enrich.FromLogContext()
           .WriteTo.File(@"C:\Servicios\wsCheckOExt\LogFile.txt")
           .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
           .ConfigureAppConfiguration((builderContext, config) =>
           {
               //var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
               //IHostEnvironment env = builderContext.HostingEnvironment;
               //config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               //.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
           })
               .ConfigureServices((hostContext, services) =>
               {
                   //services.AddHostedService<Worker>();
                   //services.AddHostedService<WorkerPost>();
                   services.AddHostedService<WorkerFilterCheck>();
                   //services.AddScoped<ILoggerService, LoggerService>();
               })
             .UseSerilog()
       .UseWindowsService();
    }
}
