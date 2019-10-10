using Newtonsoft.Json;
using System.Collections.Generic;

namespace MMS.SupportedPlatforms.HitBtc.Model
{
    public class Balance
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("available")]
        public float Available { get; set; }

        [JsonProperty("reserved")]
        public float Reserved { get; set; }
    }

    public class Balances
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("exception")]
        public ExceptionInfo Exception { get; set; }

        [JsonProperty("collections")]
        public List<Balance> BalanceList { get; set; }
    }
}
