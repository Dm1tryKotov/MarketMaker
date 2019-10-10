using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.Bistox.Model
{
    public class Payload
    {
        public PayloadDetail payload { get; set; }
    }

    public class PayloadDetail
    {
        public string price { get; set; }
        public string quantity { get; set; }
        public string type { get; set; }
        public string currencyType { get; set; }
        public string baseCurrencyType { get; set; }

    }
}
