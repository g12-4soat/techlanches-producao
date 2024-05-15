using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
        private readonly IAmazonLambda _lambdaClient;

        public PedidoGateway(IHttpClientFactory httpClientFactory, IMemoryCache cache, IAmazonLambda lambdaClient)
        {
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient(Constantes.Constantes.API_PEDIDO);
            _lambdaClient = lambdaClient;
        }

        public async Task<List<PedidoResponseDTO>> BuscarTodos()
        {
            SetToken();

            var pedidos = await _httpClient.GetFromJsonAsync<List<PedidoResponseDTO>>($"api/pedidos");
            Console.WriteLine(JsonSerializer.Serialize(pedidos));
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
            if (Constantes.Constantes.CPF_USER_DEFAULT == cpf)
                cpf = Constantes.Constantes.USER_DEFAULT;

            var request = new InvokeRequest
            {
                FunctionName = Constantes.Constantes.NOME_LAMBDA,
                Payload = "{\"body\": \"" + cpf + "\"}"
            };

            var response = await _lambdaClient.InvokeAsync(request);


            if (response?.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseBody = Encoding.Default.GetString(response.Payload.ToArray());
                var lambdaResponse = JsonSerializer.Deserialize<LambdaResponseDTO>(responseBody);

                if (lambdaResponse.Body is not null)
                    _cache.Set("authtoken", lambdaResponse.GetAccessToken());
            }            
        }

        private void SetToken()
        {
            var token = _cache.Get("authtoken").ToString().Split(" ")[1];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }


    }
}
