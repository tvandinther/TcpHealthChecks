using System.Net;
using ExampleWorker;
using TcpHealthChecks;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        services.AddTcpHealthChecks(x => true, options =>
        {
            options.IpAddress = IPAddress.Any;
            options.Port = 8080;
            options.HeartbeatInterval = TimeSpan.FromSeconds(5);
        });
    })
    .Build();

await host.RunAsync();