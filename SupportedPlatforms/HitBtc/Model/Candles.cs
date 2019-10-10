using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.HitBtc.Model
{
    public class Candles
    {
        [JsonProperty("collections")]
        public List<Candle> CandlesList { get; set; }

        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("exception")]
        public ExceptionInfo Exception { get; set; }
    }

    public class Candle
    {
        [JsonProperty("timestamp")]
        public string time { get; set; }
        [JsonProperty("open")]
        public float open { get; set; }
        [JsonProperty("max")]
        public float high { get; set; }
        [JsonProperty("min")]
        public float low { get; set; }
        [JsonProperty("close")]
        public float close { get; set; }
        [JsonProperty("volume")]
        public float vol { get; set; }
    }
}
