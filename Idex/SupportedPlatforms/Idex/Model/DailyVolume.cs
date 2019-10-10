using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportedPlatforms.Idex.Model
{
    public class DailyVolume
    {
        [JsonProperty("ETH_ADH")]
        public Volume volume { get; set; }
    }

    public class Volume
    {
        [JsonProperty("ETH")]
        public float eth { get; set; }
        [JsonProperty("ADH")]
        public float adh { get; set; }
    }
}
