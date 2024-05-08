using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using TechLanches.Producao.Application.Constantes;
using TechLanches.Producao.Application.Controllers;
using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Domain.Enums;
using TechLanches.Producao.Tests.FakeHttpHandler;
using System.Text.Json;
using System.Net;
using TechLanches.Producao.Tests.Fixtures;

namespace TechLanches.Producao.Tests.UnitTests.Controllers
{
    public class PedidoControllerTest : IClassFixture<FilaPedidoFixture>
    {
        private readonly FilaPedidoFixture _filaPedidoFixture;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private PedidoController _pedidoController;

        public PedidoControllerTest(FilaPedidoFixture filaPedidoFixture)
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _filaPedidoFixture = filaPedidoFixture;
        }

        [Fact]
        public async Task BuscarTodos_DeveRetornarListaPedidoResponseDto()
        {
            // Arrange
            var listPedidoResponseDto = new List<PedidoResponseDTO>()
                {
                    _filaPedidoFixture.CriarPedidoResponse(1, StatusPedido.PedidoPronto),
                    _filaPedidoFixture.CriarPedidoResponse(2, StatusPedido.PedidoPronto)
                };

            _cache.Set("authtoken", "bearer token");
            CriarFakeHttpClient(HttpStatusCode.OK, JsonSerializer.Serialize(listPedidoResponseDto));

            // Act
            var result = await _pedidoController.BuscarTodos();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task TrocarStatus_Pedido_DeveRetornarPedidoResponseDto()
        {
            // Arrange
            var pedidoId = 1;
            var pedidoResponseDto = new PedidoResponseDTO { Id = pedidoId };

            _cache.Set("authtoken", "bearer token");
            CriarFakeHttpClient(HttpStatusCode.OK, JsonSerializer.Serialize(pedidoResponseDto));

            // Act
            var result = await _pedidoController.TrocarStatus(pedidoId, StatusPedido.PedidoRecebido);

            // Assert
            Assert.Equal(pedidoResponseDto.Id, result.Id);
        }

        private HttpClient CriarFakeHttpClient(HttpStatusCode statusCode, string content = null)
        {
            var handler = new FakeHttpMessageHandler(content, statusCode);
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://example.com/")
            };
            _httpClientFactory.CreateClient(Constantes.API_PEDIDO).Returns(httpClient);
            _pedidoController = new PedidoController(_httpClientFactory, _cache);

            return httpClient;
        }
    }
}
