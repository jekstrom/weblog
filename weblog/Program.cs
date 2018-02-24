using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net;

namespace weblog
{
	public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel((options) =>
				{
					options.Listen(IPAddress.Any, 80);
					options.Listen(IPAddress.Any, 443);
				})
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
