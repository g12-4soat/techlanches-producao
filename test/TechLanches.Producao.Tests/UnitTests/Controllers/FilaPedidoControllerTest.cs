using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TechLanches.Producao.Application.Constantes;
using TechLanches.Producao.Application.Controllers;
using TechLanches.Producao.Application.Options;
using TechLanches.Producao.Tests.FakeHttpHandler;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechLanches.Producao.Adapter.RabbitMq;
using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Tests.Fixtures;
using NSubstitute;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using TechLanches.Producao.Domain.Enums;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Amazon.Lambda;
using TechLanches.Producao.Adapter.RabbitMq.Messaging;

namespace TechLanches.Producao.Tests.UnitTests.Controllers
{
    public class FilaPedidoControllerTest : IClassFixture<FilaPedidoFixture>
    {

        private IOptions<WorkerOptions> _workerOptions;
        private ILogger<FilaPedidoController> _logger;
        private readonly FilaPedidoFixture _filaPedidoFixture;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private FilaPedidoController _filaPedidoController;
        private PedidoController _pedidoController;
        private PedidoResponseDTO _pedidoResponse;
        private readonly IAmazonLambda _lambdaClient;
        private readonly IRabbitMqService _rabbitMqService;

        public FilaPedidoControllerTest(FilaPedidoFixture filaPedidoFixture)
        {
            _workerOptions = Options.Create(new WorkerOptions { DelayPreparacaoPedidoEmSegundos = 20 });
            _logger = new Logger<FilaPedidoController>(new NullLoggerFactory());
            _filaPedidoFixture = filaPedidoFixture;
            _cache = new MemoryCache(new MemoryCacheOptions());
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _lambdaClient = Substitute.For<IAmazonLambda>();
            _rabbitMqService = Substitute.For<IRabbitMqService>();
        }

        [Fact]
        public async Task ProcessarMensagem_DevePegarPedidoETrocarStatus()
        {
            // Arrange
            var pedidoId = 1;
            var cpf = "98982135316";
            var message = new PedidoMessage(pedidoId, cpf);

            _pedidoResponse = _filaPedidoFixture.CriarPedidoResponse(1, StatusPedido.PedidoPronto);

            _cache.Set("authtoken", "bearer token");
            CriarFakeHttpClient(HttpStatusCode.OK, JsonSerializer.Serialize(_pedidoResponse));

            // Act
            await _filaPedidoController.ProcessarMensagem(message);

            // Assert
            Assert.NotNull(_pedidoResponse);
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
            _pedidoController = new PedidoController(_httpClientFactory, _cache, _lambdaClient);
            _filaPedidoController = new FilaPedidoController(_pedidoController, _logger, _workerOptions, _rabbitMqService);
            return httpClient;
        }
    }
}
