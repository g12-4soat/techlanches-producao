using TechLanches.Producao.Adapter.API.Endpoints;

namespace TechLanches.Producao.Adapter.API.Configuration
{
    public static class MapEndpointsConfig
    {
        public static void UseMapEndpointsConfiguration(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapFilaPedidoEndpoints();
        }
    }
}
