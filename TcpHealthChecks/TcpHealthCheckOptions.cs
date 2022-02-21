using System.Net;

namespace TcpHealthChecks;

public class TcpHealthCheckOptions
{
    public IPAddress IpAddress { get; set; } = IPAddress.Any;
    public int Port { get; set; }
    public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(1);
}