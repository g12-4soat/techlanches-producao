using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Application.Gateways.Interfaces;
using TechLanches.Producao.Domain.Enums;

namespace TechLanches.Producao.Application.Gateways
{
    public class PedidoGateway : IPedidoGateway
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        public PedidoGateway(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient(Constantes.Constantes.API_PEDIDO);
        }

        public async Task<List<PedidoResponseDTO>> BuscarPorStatus(StatusPedido statusPedido)
        {
            var token = _cache.Get("authtoken").ToString().Split(" ")[1];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/pedidos/buscarpedidosporstatus/{statusPedido}");

            if (response.IsSuccessStatusCode == false)
                throw new Exception("Erro durante chamada api de pedidos.");

            string resultStr = await response.Content.ReadAsStringAsync();

            var pedido = JsonSerializer.Deserialize<List<PedidoResponseDTO>>(resultStr);

            return pedido;
        }

        public async Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido)
        {
            var token = _cache.Get("authtoken").ToString().Split(" ")[1];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(((int)statusPedido).ToString(), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/pedidos/{pedidoId}/trocarstatus", content);

            if (response.IsSuccessStatusCode == false)
                throw new Exception("Erro durante chamada api de pedidos.");

            string resultStr = await response.Content.ReadAsStringAsync();

            var pedido = JsonSerializer.Deserialize<PedidoResponseDTO>(resultStr);

            return pedido;
        }
    }
}
