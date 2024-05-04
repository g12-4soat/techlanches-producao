using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechLanches.Producao.Application.DTOs;
using TechLanches.Producao.Domain.Enums;

namespace TechLanches.Producao.Application.Gateways.Interfaces
{
    public interface IPedidoGateway
    {
        Task<List<PedidoResponseDTO>> BuscarPorStatus(StatusPedido statusPedido);
        Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido);
    }
}
