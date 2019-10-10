using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Models
{
    public class Candles
    {
        public List<Candle> CandlesCollection { get; private set; }

        public Candles(List<Candle> candles)
        {
            CandlesCollection = candles;
        }
    }

    public class Candle
    {
        public string Time { get; private set; }
        public float Open { get; private set; }
        public float High { get; private set; }
        public float Low { get; private set; }
        public float Close { get; private set; }
        public float Volume { get; private set; }

        public void SetOpen(float price) {
            Open = price;
        }
        public Candle(
            string time,
            float open,
            float close,
            float high,
            float low,
            float volume
            )
        {
            Time = time;
            Open = open;
            Close = close;
            High = high;
            Low = low;
            Volume = volume;
        }
    }
}
