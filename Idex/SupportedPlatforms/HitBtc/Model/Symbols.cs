using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.HitBtc.Model
{
    public class Symbols
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("exception")]
        public ExceptionInfo Exception { get; set; }

        [JsonProperty("collections")]
        public List<Symbol> SymbolsList { get; set; }
    }

    public class Symbol
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("baseCurrency")]
        public string BaseCurrency { get; set; }
        [JsonProperty("quoteCurrency")]
        public string QuoteCurrency { get; set; }
        [JsonProperty("quantityIncrement")]
        public float QuantityIncrement { get; set; }
        [JsonProperty("tickSize")]
        public float TickSize { get; set; }
    }
}
