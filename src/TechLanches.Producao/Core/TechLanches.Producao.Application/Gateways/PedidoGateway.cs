using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TechLanches.Producao.Application.Constantes;
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

        public async Task<List<PedidoResponseDTO>> BuscarTodos()
        {
            SetToken();

            var response = await _httpClient.GetAsync($"api/pedidos");

            if (response.IsSuccessStatusCode == false)
                throw new Exception("Erro durante chamada api de pedidos.");

            string resultStr = await response.Content.ReadAsStringAsync();

            var pedidos = JsonSerializer.Deserialize<List<PedidoResponseDTO>>(resultStr);

            return pedidos.Where(x => x.StatusPedido == StatusPedido.PedidoRecebido ||
                            x.StatusPedido == StatusPedido.PedidoEmPreparacao ||
                            x.StatusPedido == StatusPedido.PedidoPronto ||
                            x.StatusPedido == StatusPedido.PedidoFinalizado).ToList();
        }

        public async Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido)
        {
            SetToken();

            var content = new StringContent(((int)statusPedido).ToString(), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/pedidos/{pedidoId}/trocarstatus", content);

            if (response.IsSuccessStatusCode == false)
                throw new Exception("Erro durante chamada api de pedidos.");

            string resultStr = await response.Content.ReadAsStringAsync();

            var pedido = JsonSerializer.Deserialize<PedidoResponseDTO>(resultStr);

            return pedido;
        }

        public async Task BuscarTokenLambda(string cpf)
        {
            var lambdaAuth = new AmazonLambdaClient();

            if (Constantes.Constantes.CPF_USER_DEFAULT == cpf)
                cpf = Constantes.Constantes.USER_DEFAULT;

            var request = new InvokeRequest
            {
                FunctionName = Constantes.Constantes.NOME_LAMBDA,
                Payload = "{\"body\": \"" + cpf + "\"}"
            };

            var response = await lambdaAuth.InvokeAsync(request);


            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseBody = Encoding.Default.GetString(response.Payload.ToArray());
                var lambdaResponse = JsonSerializer.Deserialize<LambdaResponseDTO>(responseBody);

                if (lambdaResponse.Body is not null)
                    _cache.Set("authtoken", lambdaResponse.GetAccessToken());
            }
            else
                throw new Exception("Nenhum Token encontrado.");
        }

        private void SetToken()
        {
            var token = _cache.Get("authtoken").ToString().Split(" ")[1];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }


    }
}
