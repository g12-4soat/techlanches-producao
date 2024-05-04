using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Domain.Enums;

namespace TechLanches.Producao.Application.Controllers.Interfaces
{
    public interface IPedidoController
    {
        Task<List<PedidoResponseDTO>> BuscarPorStatus(StatusPedido statusPedido);
        Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido);
    }
}
