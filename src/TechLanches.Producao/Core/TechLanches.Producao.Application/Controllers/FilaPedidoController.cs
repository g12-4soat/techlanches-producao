using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechLanches.Producao.Adapter.RabbitMq;
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

        public FilaPedidoController(IPedidoController pedidoController, ILogger<FilaPedidoController> logger, IOptions<WorkerOptions> workerOptions)
        {
            _pedidoController = pedidoController;
            _logger = logger;
            _workerOptions = workerOptions.Value;
        }

        public async Task ProcessarMensagem(PedidoMessage message)
        {
            _logger.LogInformation("FilaPedidosHostedService iniciado: {time}", DateTimeOffset.Now);

            _logger.LogInformation("Próximo pedido da fila: {proximoPedido.Id}", message.Id);

            await _pedidoController.TrocarStatus(message.Id, StatusPedido.PedidoEmPreparacao);

            _logger.LogInformation("Pedido {proximoPedido.Id} em preparação.", message.Id);

            await Task.Delay(1000 * _workerOptions.DelayPreparacaoPedidoEmSegundos);

            _logger.LogInformation("Pedido {proximoPedido.Id} preparação finalizada.", message.Id);

            await _pedidoController.TrocarStatus(message.Id, StatusPedido.PedidoPronto);

            _logger.LogInformation("Pedido {proximoPedido.Id} pronto.", message.Id);
        }
    }
}
