using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechLanches.Producao.Adapter.FilaPedidos.Options;
using TechLanches.Producao.Adapter.RabbitMq.Messaging;
using TechLanches.Producao.Application.Controllers.Interfaces;
using TechLanches.Producao.Domain.Enums;

namespace TechLanches.Producao.Adapter.FilaPedidos
{
    public class FilaPedidosHostedService : BackgroundService
    {
        private readonly IPedidoController _pedidoController;
        private readonly ILogger<FilaPedidosHostedService> _logger;
        private readonly WorkerOptions _workerOptions;
        private readonly IRabbitMqService _rabbitMqService;

        public FilaPedidosHostedService(IPedidoController pedidoController,
                                        ILogger<FilaPedidosHostedService> logger,
                                        IOptions<WorkerOptions> workerOptions,
                                        IRabbitMqService rabbitMqService)
        {
            _pedidoController = pedidoController;
            _logger = logger;
            _workerOptions = workerOptions.Value;
            _rabbitMqService = rabbitMqService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitMqService.Consumir(ProcessMessageAsync);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public async Task ProcessMessageAsync(string message)
        {
            var pedidoId = Convert.ToInt32(message);//Deserialize a nova classe e pegar cpf no cognito

            _logger.LogInformation("FilaPedidosHostedService iniciado: {time}", DateTimeOffset.Now);

            _logger.LogInformation("Próximo pedido da fila: {proximoPedido.Id}", pedidoId);

            await _pedidoController.TrocarStatus(pedidoId, StatusPedido.PedidoEmPreparacao);

            _logger.LogInformation("Pedido {proximoPedido.Id} em preparação.", pedidoId);

            await Task.Delay(1000 * _workerOptions.DelayPreparacaoPedidoEmSegundos);

            _logger.LogInformation("Pedido {proximoPedido.Id} preparação finalizada.", pedidoId);

            await _pedidoController.TrocarStatus(pedidoId, StatusPedido.PedidoPronto);

            _logger.LogInformation("Pedido {proximoPedido.Id} pronto.", pedidoId);
        }
    }
}
