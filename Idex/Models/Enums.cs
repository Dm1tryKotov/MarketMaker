using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Models
{
    public enum OrderSide
    {
        Sell,
        Buy
    }

    public enum OrderType
    {
        Limit,
        Market
    }

    public enum CandleType
    {
        Up,
        Down
    }

    public enum PairName
    {
        ETHXRP,
        ETHXLM,
        ETHADH,
        ETHBCH,
        ETHLTC,
        ETHLRC,
        ETHXEM,
        ETHUSDT,
        BTCADH,
        BTCLRC,
        BTCBCH,
        BTCETH,
        BTCXRP,
        BTCXLM,
        BTCLTC,
        BTCXEM
    }

    public enum Platform
    {
        HitBTC,
        Liquid,
        Bistox
    }

    public enum TimeFrame
    {
        M1,
        M3,
        M5,
        M15,
        M30,
        H1,
        H4,
        D1,
        D7
    }
}
