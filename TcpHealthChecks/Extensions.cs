using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace TcpHealthChecks;

public static class Extensions
{
    public static IServiceCollection AddTcpHealthChecks(this IServiceCollection services, Func<HealthCheckRegistration, bool> predicate, Action<TcpHealthCheckOptions> setupAction)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (setupAction == null)
        {
            throw new ArgumentNullException(nameof(setupAction));
        }

        services.Configure(setupAction);

        services.AddHostedService(sp =>
        {
            var options = sp.GetRequiredService<IOptions<TcpHealthCheckOptions>>().Value;
            return new TcpHealthCheckService(options, sp.GetRequiredService<HealthCheckService>(), predicate);
        });

        return services;
    }
}