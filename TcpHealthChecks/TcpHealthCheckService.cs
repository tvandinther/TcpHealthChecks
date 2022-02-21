using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace TcpHealthChecks;

public class TcpHealthCheckService : BackgroundService
{
    private readonly TcpListener _listener;
    private readonly HealthCheckService _healthCheckService;
    private readonly Func<HealthCheckRegistration, bool> _healthCheckPredicate;

    public TcpHealthCheckService(IPAddress address, int port, HealthCheckService healthCheckService, Func<HealthCheckRegistration, bool> healthCheckPredicate)
    {
        _listener = new TcpListener(address, port);
        _healthCheckService = healthCheckService;
        _healthCheckPredicate = healthCheckPredicate;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        _listener.Start();
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateHeartbeatAsync(stoppingToken);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            if (_listener.Pending())
            {
                _listener.Stop();
            }
        }
        
        _listener.Stop();
    }
    
    private async Task UpdateHeartbeatAsync(CancellationToken cancellationToken)
    {
        var isHealthy = true;
        var status = await _healthCheckService.CheckHealthAsync(_healthCheckPredicate, cancellationToken);
        if (status.Status != HealthStatus.Healthy)
        {
            isHealthy = false;
        }

        if (!isHealthy)
        {
            _listener.Stop();
            return;
        }
        
        _listener.Start();

        while (_listener.Server.IsBound && _listener.Pending())
        {
            var client = await _listener.AcceptTcpClientAsync(cancellationToken);
            client.Close();
        }
    }
}