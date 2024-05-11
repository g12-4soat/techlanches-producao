using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechLanches.Producao.Application.DTOs
{
    public class LambdaResponseDTO
    {
        [JsonPropertyName("body")]
        public string Body { get; set; }

        public string GetAccessToken()
        {
            var bodyJson = JsonSerializer.Deserialize<JsonElement>(Body);
            var accessToken = bodyJson.GetProperty("AccessToken").GetString();
            return accessToken;
        }
    }
}
