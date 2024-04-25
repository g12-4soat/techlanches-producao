using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechLanches.Producao.Adapter.AWS
{
    public class TechLanchesCognitoSecrets
    {
        public string Region { get; set; } = string.Empty;
        public string UserPoolId { get; set; } = string.Empty;
        public string UserPoolClientId { get; set; } = string.Empty;
        public string CognitoUri { get => $"https://cognito-idp.{Region}.amazonaws.com/{UserPoolId}"; }
    }
}
