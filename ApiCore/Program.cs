using ApiCore.Config;
using Autofac.Extensions.DependencyInjection;

namespace ApiCore;

public class Program
{
    protected Program()
    {
    }

    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .AddSerilogCustom()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
