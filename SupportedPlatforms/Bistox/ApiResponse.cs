using MMS.SupportedPlatforms.Bistox.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.Bistox
{
    public class ApiResponse
    {
        public string Content;

        public static implicit operator OrderBookResponse(ApiResponse response)
        {
            return Utilities.ConverFromJason<OrderBookResponse>(response);
        }

        public static implicit operator CandleResponse(ApiResponse response)
        {
            var res = Utilities.ConverFromJason(response);
            var type = res as JObject;
            var type2 = type.Value<JArray>("result");
            var candles = new List<Candle>();
            foreach(var item in type2)
            {
                candles.Add(new Candle()
                {
                    open = item.Value<float>("open"),
                    close = item.Value<float>("close"),
                    low = item.Value<float>("low"),
                    high = item.Value<float>("high"),
                    volume = item.Value<float>("volume"),
                    time = item.Value<long>("time"),
                });
            }
            return new CandleResponse() { candles = candles};
        }

        public static implicit operator Detail(ApiResponse response)
        {
            var res = Utilities.ConverFromJason(response);
            
            var type = res as JObject;
            var type2 = type.Value<JObject>("result");
            var type3 = type2.Value<JObject>("ticker");

            return new Detail() { obj = type3 };
        }

        private class R {
            public E result { get; set; }
        }

        private class E
        {
            public object announcement { get; set; }
            public object spotlight { get; set; }
            public object trending { get; set; }
            public object system { get; set; }
            public object deposit { get; set; }
            public object withdraw { get; set; }
            public object trade { get; set; }
            public W ticker { get; set; }
        }

        private class W
        {

        }
    }
}
