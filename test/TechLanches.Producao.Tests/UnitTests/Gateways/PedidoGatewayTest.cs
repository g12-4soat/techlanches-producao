﻿using Amazon.Lambda;
using Amazon.Lambda.Model;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using System.Net;
using System.Text;
using System.Text.Json;
using TechLanches.Producao.Application.Constantes;
using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Application.Gateways;
using TechLanches.Producao.Domain.Enums;
using TechLanches.Producao.Tests.FakeHttpHandler;
using TechLanches.Producao.Tests.Fixtures;


namespace TechLanches.Producao.Tests.UnitTests.Gateways
{
    public class PedidoGatewayTest : IClassFixture<FilaPedidoFixture>
    {
        private readonly FilaPedidoFixture _filaPedidoFixture;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private PedidoGateway _pedidoGateway;
        private readonly IAmazonLambda _lambdaClient;

        public PedidoGatewayTest(FilaPedidoFixture filaPedidoFixture)
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _filaPedidoFixture = filaPedidoFixture;
            _lambdaClient = Substitute.For<IAmazonLambda>();
        }

        [Fact]
        public async Task BuscarTodos_DeveRetornarFilaDePedidos()
        {
            // Arrange
            var listPedidoResponseDto = new List<PedidoResponseDTO>()
                {
                    _filaPedidoFixture.CriarPedidoResponse(1, StatusPedido.PedidoRecebido),
                    _filaPedidoFixture.CriarPedidoResponse(2, StatusPedido.PedidoEmPreparacao),
                    _filaPedidoFixture.CriarPedidoResponse(3, StatusPedido.PedidoPronto),
                    _filaPedidoFixture.CriarPedidoResponse(4, StatusPedido.PedidoFinalizado)
                };

            _cache.Set("authtoken", "bearer token");
            CriarFakeHttpClient(HttpStatusCode.OK, JsonSerializer.Serialize(listPedidoResponseDto));

            // Act
            var listPedidoResponse = await _pedidoGateway.BuscarTodos();

            // Assert
            Assert.NotNull(listPedidoResponse);
            Assert.True(listPedidoResponse.Any());
        }

        //[Fact]
        //public async Task BuscarTodos_StatusDiferenteDeOk_DeveLancarException()
        //{
        //    // Arrange
        //    var listPedidoResponseDto = new List<PedidoResponseDTO>();

        //    _cache.Set("authtoken", "bearer token");
        //    CriarFakeHttpClient(HttpStatusCode.InternalServerError, JsonSerializer.Serialize(listPedidoResponseDto));

        //    // Act & Assert
        //    await Assert.ThrowsAsync<Exception>(async () => await _pedidoGateway.BuscarTodos());
        //}

        [Fact]
        public async Task TrocarStatus_DeveChamarMetodosCorretosETratarStatusCodeOk()
        {
            // Arrange
            var pedidoId = 1;
            var statusPedido = StatusPedido.PedidoRecebido;
            var pedidoResponseDto = _filaPedidoFixture.CriarPedidoResponse(pedidoId, statusPedido);

            _cache.Set("authtoken", "bearer token");
            CriarFakeHttpClient(HttpStatusCode.OK, JsonSerializer.Serialize(pedidoResponseDto));

            // Act
            var pedidoResponse = await _pedidoGateway.TrocarStatus(pedidoId, statusPedido);

            // Assert
            Assert.Equal(pedidoId, pedidoResponse.Id);
            Assert.Equal(statusPedido, pedidoResponse.StatusPedido);
        }

        [Fact]
        public async Task TrocarStatus_DeveTratarStatusCodeDiferenteDeOk()
        {

            // Arrange
            var pedidoId = 1;
            var statusPedido = StatusPedido.PedidoRecebido;
            var pedidoResponseDto = _filaPedidoFixture.CriarPedidoResponse(pedidoId, statusPedido);

            _cache.Set("authtoken", "bearer token");
            CriarFakeHttpClient(HttpStatusCode.InternalServerError, JsonSerializer.Serialize(pedidoResponseDto));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _pedidoGateway.TrocarStatus(pedidoId, statusPedido));
        }

        [Fact]
        public async Task BuscarTokenLambda_ComCPFValido_DeveRetornarToken()
        {
            // Arrange
            var cpfTechLanches = "61872530001";
            var fakeResponsePayload = "{\"body\":\"{\\\"TokenId\\\":\\\"eyJk9KWTg\\\",\\\"AccessToken\\\":\\\"eyJraWQiyn3r\\\"}\"}";
            var response = new InvokeResponse
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                Payload = new MemoryStream(Encoding.UTF8.GetBytes(fakeResponsePayload))
            };
            _lambdaClient.InvokeAsync(Arg.Any<InvokeRequest>()).Returns(Task.FromResult(response));

            _pedidoGateway = new PedidoGateway(_httpClientFactory, _cache, _lambdaClient);

            // Act
            await _pedidoGateway.BuscarTokenLambda(cpfTechLanches);

            // Assert
            await _lambdaClient.Received(1).InvokeAsync(Arg.Any<InvokeRequest>());
        }

        private HttpClient CriarFakeHttpClient(HttpStatusCode statusCode, string content = null)
        {
            var handler = new FakeHttpMessageHandler(content, statusCode);
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://example.com/")
            };
            _httpClientFactory.CreateClient(Constantes.API_PEDIDO).Returns(httpClient);
            _pedidoGateway = new PedidoGateway(_httpClientFactory, _cache, _lambdaClient);
            return httpClient;
        }
    }
}
