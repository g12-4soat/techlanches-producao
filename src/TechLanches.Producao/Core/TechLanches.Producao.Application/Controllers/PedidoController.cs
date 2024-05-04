using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<List<PedidoResponseDTO>> BuscarPorStatus(StatusPedido statusPedido)
        {
            var filaPedidos = await _pedidoGateway.BuscarPorStatus(statusPedido);
            return filaPedidos;
        }

        public async Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido)
        {
            var pedido = await _pedidoGateway.TrocarStatus(pedidoId, statusPedido);
            return pedido;
        }
    }
}
