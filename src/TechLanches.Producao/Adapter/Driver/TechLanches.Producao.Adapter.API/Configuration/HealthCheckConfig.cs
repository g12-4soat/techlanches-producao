using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TechLanches.Producao.Adapter.FilaPedidos.Health;


namespace TechLanches.Producao.Adapter.API.Configuration
{
    public static class HealthCheckConfig
    {
        public static void AddHealthCheckConfig(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddHealthChecks()
                 .AddCheck<RabbitMQHealthCheck>("rabbit_hc");
        }
        public static void AddHealthCheckEndpoint(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                });
            });
        }
    }
}
