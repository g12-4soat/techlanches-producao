using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechLanches.Producao.Adapter.RabbitMq;
using TechLanches.Producao.Adapter.RabbitMq.Messaging;
using TechLanches.Producao.Application.Controllers.Interfaces;
using TechLanches.Producao.Application.Options;
using TechLanches.Producao.Domain.Enums;

namespace TechLanches.Producao.Application.Controllers
{
    public class FilaPedidoController : IFilaPedidoController
    {
        private readonly IPedidoController _pedidoController;
        private readonly ILogger<FilaPedidoController> _logger;
        private readonly WorkerOptions _workerOptions;
        private readonly IRabbitMqService _rabbitMqService;
        public FilaPedidoController(IPedidoController pedidoController, ILogger<FilaPedidoController> logger, IOptions<WorkerOptions> workerOptions,
            IRabbitMqService rabbitMqService)
        {
            _pedidoController = pedidoController;
            _logger = logger;
            _workerOptions = workerOptions.Value;
            _rabbitMqService = rabbitMqService;
        }

        public async Task ProcessarMensagem(PedidoMessage message)
        {
            _logger.LogInformation("FilaPedidosHostedService iniciado: {time}", DateTimeOffset.Now);

            _logger.LogInformation("Próximo pedido da fila: {proximoPedidoId}", message.Id);

            var pedidoStatusMessage = new PedidoStatusMessage(message.Id, StatusPedido.PedidoEmPreparacao);
            _rabbitMqService.Publicar(pedidoStatusMessage);

            _logger.LogInformation("Pedido {proximoPedidoId} em preparação.", message.Id);

            await Task.Delay(1000 * _workerOptions.DelayPreparacaoPedidoEmSegundos);

            _logger.LogInformation("Pedido {proximoPedidoId} preparação finalizada.", message.Id);

            pedidoStatusMessage = new PedidoStatusMessage(message.Id, StatusPedido.PedidoPronto);
            _rabbitMqService.Publicar(pedidoStatusMessage);

            _logger.LogInformation("Pedido {proximoPedidoId} pronto.", message.Id);
        }
    }
}