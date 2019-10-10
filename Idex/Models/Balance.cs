using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Models
{
    public class Balance
    {
        public BalanceItem BaseCurrency { get; private set; }
        public BalanceItem QuoteCurrency { get; private set; }

        public Balance(BalanceItem baseCurr, BalanceItem quoteCurr) {
            BaseCurrency = BaseCurrency;
            QuoteCurrency = quoteCurr;
        }
    }

    public class BalanceItem
    {
        public string Name { get; private set; }
        public float Volume { get; private set; }

        public BalanceItem(string name, float volume) {
            Name = name;
            Volume = volume;
        }
    }
}
