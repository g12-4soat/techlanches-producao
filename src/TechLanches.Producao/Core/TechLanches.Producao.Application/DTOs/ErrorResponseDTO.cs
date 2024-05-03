using System.Net;

namespace TechLanches.Producao.Application.DTOs
{
    public class ErrorResponseDTO
    {
        public HttpStatusCode StatusCode { get; set; }
        public string MensagemErro { get; set; }
    }
}
