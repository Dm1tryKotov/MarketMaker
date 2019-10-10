using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportedPlatforms.Idex.Model
{
    public class Balance
    {
        [JsonProperty("ETH")]
        public float eth { get; set; }
        [JsonProperty("ADHV")]
        public float adh { get; set; }
    }
}
