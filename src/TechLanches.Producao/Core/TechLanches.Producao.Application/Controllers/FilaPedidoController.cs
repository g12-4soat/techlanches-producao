using TechLanches.Producao.Application.Controllers.Interfaces;
using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Domain.Enums;

namespace TechLanches.Producao.Application.Controllers
{
    public class FilaPedidoController : IFilaPedidoController
    {
        public async Task TrocarStatus(int pedidoId, StatusPedido statusPedido)
        {
            //Call MicroService
        }
        public async Task<List<PedidoResponseDTO>> BuscarPorStatus(StatusPedido statusPedido)
        {
            //Call MicroService
            return null;
        }
    }
}
