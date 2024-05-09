using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Domain.Enums;

namespace TechLanches.Producao.Tests.Fixtures
{
    public class FilaPedidoFixture : IDisposable
    {
        public PedidoResponseDTO CriarPedidoResponse(int id, StatusPedido status)
        {
            return new PedidoResponseDTO { Id = id, StatusPedido = status };
        }

        public void Dispose()
        {
        }
    }
}
