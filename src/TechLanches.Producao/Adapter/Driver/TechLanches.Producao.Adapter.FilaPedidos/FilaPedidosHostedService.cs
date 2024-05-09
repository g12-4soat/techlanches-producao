using Microsoft.Extensions.Hosting;
using TechLanches.Producao.Adapter.RabbitMq.Messaging;
using TechLanches.Producao.Application.Controllers.Interfaces;

namespace TechLanches.Producao.Adapter.FilaPedidos
{
    public class FilaPedidosHostedService : BackgroundService
    {
        private readonly IFilaPedidoController _filaPedidoController;
        private readonly IRabbitMqService _rabbitMqService;

        public FilaPedidosHostedService(IFilaPedidoController filaPedidoController,
                                        IRabbitMqService rabbitMqService)
        {
            _filaPedidoController = filaPedidoController;
            _rabbitMqService = rabbitMqService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitMqService.Consumir(_filaPedidoController.ProcessarMensagem);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
  
    }
}
