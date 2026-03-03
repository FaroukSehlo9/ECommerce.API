
using ECommerce.Api;
using Microsoft.AspNetCore.Hosting;

namespace ECommerce.API
{
    public class Program
    {

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(webBuilder =>
          {
              webBuilder.UseStartup<Startup>();
              webBuilder.ConfigureLogging((ctx, logging) =>
              {
              });
          });

    }
}
