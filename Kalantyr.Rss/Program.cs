using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Kalantyr.Rss
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration((hc, config) =>
                {
                    var fileInfo = new FileInfo(typeof(Program).Assembly.Location);
                    config.SetBasePath(fileInfo.Directory.FullName);
                })
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
                .UseWindowsService()
                .Build()
                .Run();
        }
    }
}
