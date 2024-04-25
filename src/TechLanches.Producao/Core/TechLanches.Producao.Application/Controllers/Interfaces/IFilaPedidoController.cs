using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Domain.Enums;

namespace TechLanches.Producao.Application.Controllers.Interfaces
{
    public interface IFilaPedidoController
    {
        Task<List<PedidoResponseDTO>> BuscarPorStatus(StatusPedido statusPedido);
        Task TrocarStatus(int pedidoId, StatusPedido statusPedido);
    }
}
