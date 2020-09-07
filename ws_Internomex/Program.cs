using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace ws_Internomex
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string Direccion = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug().MinimumLevel
               .Override("Microsoft", LogEventLevel.Warning).Enrich
               .FromLogContext().WriteTo
               .File(Direccion + @"\log.txt")
               .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddHostedService<Worker>();
                    services.AddHostedService<Solicitudes>();
                }).UseWindowsService();
    }
}
