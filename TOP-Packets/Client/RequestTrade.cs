using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;
using TOP_Network.Enum;

namespace TOP_Packets.Client
{
    public abstract class TradeType;

    public abstract class ItemTrade;

    public class ItemSale : ItemTrade
    {
        public short GridID { get; set; }
        public short Amount { get; set; }
    }

    public class Trade : TradeType
    {
        [Choose(0, typeof(ItemSale))]
        [Choose(1, typeof(ItemBuy))]
        public ItemTrade ItemTrade { get; set; }
    }

    public class ItemBuy : ItemTrade
    {
        public byte A { get; set; }
        public short B { get; set; }
        public short C { get; set; }
        public short D { get; set; }
    }

    public class RequestTrade
    {
        public uint NPCId { get; set; }
        [Choose((int)Commands.CMD_CM_TRADEITEM, typeof(Trade), typeof(short))]
        public TradeType TradeType { get; set; }
    }
}
