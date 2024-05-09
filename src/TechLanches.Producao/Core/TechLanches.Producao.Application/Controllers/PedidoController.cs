using Microsoft.Extensions.Caching.Memory;
using TechLanches.Producao.Application.Controllers.Interfaces;
using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Application.Gateways;
using TechLanches.Producao.Application.Gateways.Interfaces;
using TechLanches.Producao.Domain.Enums;

namespace TechLanches.Producao.Application.Controllers
{
    public class PedidoController : IPedidoController
    {
        private readonly IPedidoGateway _pedidoGateway;

        public PedidoController(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _pedidoGateway = new PedidoGateway(httpClientFactory, cache);
        }

        public async Task<List<PedidoResponseDTO>> BuscarTodos()
        {
            var pedidos = await _pedidoGateway.BuscarTodos();
            return pedidos;
        }

        public async Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido)
        {
            var pedido = await _pedidoGateway.TrocarStatus(pedidoId, statusPedido);
            return pedido;
        }
    }
}
