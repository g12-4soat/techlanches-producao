using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechLanches.Producao.Application.DTOs
{
    public class PagamentoResponseDTO
    {
        /// <summary>
        /// Id Pedido
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Valor Pagamento
        /// </summary>
        /// <example>15.50</example>
        public decimal Valor { get; set; }

        /// <summary>
        /// Valor Pagamento
        /// </summary>
        /// <example>Aprovado</example>
        public string StatusPagamento { get; set; }
    }
}
