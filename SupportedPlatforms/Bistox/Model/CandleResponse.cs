using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.Bistox.Model
{
    public class CandleResponse
    {
        public List<Candle> candles;
    }
    public class Candle
    {
        [JsonProperty("open")]
        public float open { get; set; }
        [JsonProperty("close")]
        public float close { get; set; }
        [JsonProperty("low")]
        public float low { get; set; }
        public float high { get; set; }
        public float volume { get; set; }
        public long time { get; set; }
    }
}
