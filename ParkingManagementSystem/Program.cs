using Microsoft.AspNetCore;

namespace ParkingManagementSystem.API
{
    public static class Program
    {

        public static int Main(string[] args)
        {
            Log.Logger = LoggerHelper.CreateLoggerConfiguration().CreateLogger();
            try
            {
                CreateWebHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

}

