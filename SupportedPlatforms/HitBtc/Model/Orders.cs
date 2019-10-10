using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.HitBtc.Model
{
    public class Orders
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("exception")]
        public ExceptionInfo Exception { get; set; }

        [JsonProperty("collections")]
        public List<Order> OrderList { get; set; }
    }
}
