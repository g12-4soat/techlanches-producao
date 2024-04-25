using TechLanches.Producao.Adapter.API.Middlewares;

namespace TechLanches.Producao.Adapter.API.Configuration
{
    public static class ApplicationBuilderConfig
    {
        public static IApplicationBuilder AddCustomMiddlewares(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<RequestLoggingMiddleware>();
            applicationBuilder.UseMiddleware<GlobalErrorHandlingMiddleware>();

            return applicationBuilder;
        }
    }
}
