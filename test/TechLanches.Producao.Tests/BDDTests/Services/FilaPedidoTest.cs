using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net;
using System.Text.Json;
using TechLanches.Producao.Adapter.RabbitMq;
using TechLanches.Producao.Application.Constantes;
using TechLanches.Producao.Application.Controllers;
using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Application.Options;
using TechLanches.Producao.Domain.Enums;
using TechLanches.Producao.Tests.FakeHttpHandler;
using TechLanches.Producao.Tests.Fixtures;

namespace TechLanches.Producao.Tests.BDDTests.Services
{
    [Trait("Services", "FilaPedido")]
    public class FilaPedidoTest : IClassFixture<FilaPedidoFixture>
    {
        private PedidoMessage _message;
        private IOptions<WorkerOptions> _workerOptions;
        private ILogger<FilaPedidoController> _logger;
        private readonly FilaPedidoFixture _filaPedidoFixture;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private FilaPedidoController _filaPedidoController;
        private PedidoController _pedidoController;
        private PedidoResponseDTO _pedidoResponse;

        public FilaPedidoTest(FilaPedidoFixture filaPedidoFixture)
        {
            _workerOptions = Options.Create(new WorkerOptions { DelayPreparacaoPedidoEmSegundos = 20 });
            _logger = new Logger<FilaPedidoController>(new NullLoggerFactory());
            _filaPedidoFixture = filaPedidoFixture;
            _cache = new MemoryCache(new MemoryCacheOptions());
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
        }

        [Fact(DisplayName = "Deve processar pedido com sucesso")]
        public async Task ProcessarPedido_DeveRetornarSucesso()
        {
            Given_PedidoComDadosValidos();
            await When_ProcessarMensagem();
            Then_StatusPedidoNaoDeveSerNulo();
            Then_StatusPedidoDeveSerPronto();
        }

        private void Given_PedidoComDadosValidos()
        {
            var pedidoId = 1;
            var cpf = "98982135316";
            _message = new PedidoMessage(pedidoId, cpf);
        }

        private async Task When_ProcessarMensagem()
        {
            _pedidoResponse = _filaPedidoFixture.CriarPedidoResponse(1, StatusPedido.PedidoPronto);

            _cache.Set("authtoken", "bearer token");
            CriarFakeHttpClient(HttpStatusCode.OK, JsonSerializer.Serialize(_pedidoResponse));

            await _filaPedidoController.ProcessarMensagem(_message);
        }

        private void Then_StatusPedidoNaoDeveSerNulo()
        {
            Assert.NotNull(_pedidoResponse);
        }

        private void Then_StatusPedidoDeveSerPronto()
        {
            Assert.Equal(StatusPedido.PedidoPronto, _pedidoResponse.StatusPedido);
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
            _filaPedidoController = new FilaPedidoController(_pedidoController, _logger, _workerOptions);
            return httpClient;
        }

    }
}
