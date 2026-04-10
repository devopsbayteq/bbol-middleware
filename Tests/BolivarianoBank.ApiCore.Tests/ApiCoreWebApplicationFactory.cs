using ApiCore;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace BolivarianoBank.ApiCore.Tests;

public class ApiCoreWebApplicationFactory
{
    public static async Task<TestServer> CreateServer(Mock<IMediator> mediator)
    {
        var host = new HostBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.UseTestServer()
            .UseEnvironment("Test")
                .ConfigureAppConfiguration((context, config) =>
                {

                    config.Sources.Clear();
                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                    config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureTestServices(services =>
                {
                    services.AddScoped(_ => mediator.Object);
                    //services.AddScoped(_ => SecurityUnitOfWork.Object);
                })
                .UseStartup<Startup>();
        }).Build();

        await host.StartAsync();
        return host.GetTestServer();
    }
}
