using SupportedPlatforms.Idex.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportedPlatforms.Idex
{
    public class ApiResponse
    {
        public string Content { get; set; }

        public static implicit operator Balance(ApiResponse response)
        {
            return response == null
                ? null
                : Utilities.ConverFromJason<Balance>(response);
        }
        public static implicit operator DailyVolume(ApiResponse response)
        {
            return response == null
                ? null
                : Utilities.ConverFromJason<DailyVolume>(response);
        }
        public static implicit operator OrderBook(ApiResponse response)
        {
            return response == null
                ? null
                : Utilities.ConverFromJason<OrderBook>(response);
        }
    }
}
