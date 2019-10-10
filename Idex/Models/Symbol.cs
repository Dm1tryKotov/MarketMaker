using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Models
{
    public class Symbol
    {
        public string QuoteCurrency { get; private set; }
        public string BaseCurrency { get; private set; }

        public float TickSize { get; private set; }

        public float QuantityIncrement { get; private set; }

        public Symbol(string quoteCurrency, string baseCurrency, float tickSize, float quantityIncrement) {
            QuoteCurrency = quoteCurrency;
            BaseCurrency = baseCurrency;
            TickSize = tickSize;
            QuantityIncrement = quantityIncrement;
        }
    }
}
