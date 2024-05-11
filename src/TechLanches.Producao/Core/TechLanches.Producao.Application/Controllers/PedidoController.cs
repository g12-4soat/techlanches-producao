using Amazon.Lambda.Model;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using TechLanches.Producao.Application.Controllers.Interfaces;
using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Application.Gateways;
using TechLanches.Producao.Application.Gateways.Interfaces;
using TechLanches.Producao.Domain.Enums;
using Amazon.Lambda;

namespace TechLanches.Producao.Application.Controllers
{
    public class PedidoController : IPedidoController
    {
        private readonly IPedidoGateway _pedidoGateway;

        public PedidoController(IHttpClientFactory httpClientFactory, IMemoryCache cache, AmazonLambdaClient lambdaAuth)
        {
            _pedidoGateway = new PedidoGateway(httpClientFactory, cache, lambdaAuth);
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

        public async Task BuscarTokenLambda(string cpf) => await _pedidoGateway.BuscarTokenLambda(cpf);
      
    }
}
