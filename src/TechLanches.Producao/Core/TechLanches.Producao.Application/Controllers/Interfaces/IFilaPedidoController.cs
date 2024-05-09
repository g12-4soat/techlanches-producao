using TechLanches.Producao.Adapter.RabbitMq;

namespace TechLanches.Producao.Application.Controllers.Interfaces
{
    public interface IFilaPedidoController
    {
        Task ProcessarMensagem(PedidoMessage message);
    }
}
