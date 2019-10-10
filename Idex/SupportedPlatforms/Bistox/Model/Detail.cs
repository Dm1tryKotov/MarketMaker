using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.Bistox.Model
{
    public class Detail
    {
        public JObject obj { get; set; } 
    }

    public class Ticker
    {
        public float price { get; set; }

        public string currencyType { get; set; }

        public string baseCurrencyType { get; set; }

        public string currencyName { get; set; }

        public string baseCurrencyName { get; set; }

        public float change24 { get; set; }

        public float vol24 { get; set; }

        public float min24 { get; set; }

        public float max24 { get; set; }

    }
}
