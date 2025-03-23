using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;
using TOP_Network.Enum;

namespace TOP_Packets.Client
{
    public abstract class NPCTalk;

    public class TalkPage : NPCTalk
    {
        public byte Command { get; set; }
    }

    public class FuncItem : NPCTalk
    {
        public byte PageID { get; set; }
        public byte Index { get; set; }
    }
    public class TradeSale : NPCTalk
    {
        public byte SaleRole { get; set; }
        /*public byte Index { get; set; }
        public byte count { get; set; }*/
    }


    public class Mission : NPCTalk
    {
        public abstract class MissionType;
        public class MissionSell : MissionType
        {
            public byte Index { get; set; }
        }
        public class MissionDelivery : MissionType
        {
            public byte SellItem { get; set; }
            public byte Param { get; set; }
        }

        [Choose(4, typeof(MissionSell))]
        [Choose(7, typeof(MissionDelivery))]
        [Choose(6, typeof(MissionDelivery))]
        public MissionType Type { get; set; }
    }

    public class RequestTalk
    {
        public uint NPCId { get; set; }

        [Choose((int)Commands.CMD_CM_TALKPAGE, typeof(TalkPage), typeof(short))]
        [Choose((int)Commands.CMD_CM_FUNCITEM, typeof(FuncItem), typeof(short))]
        [Choose((int)Commands.CMD_CM_TRADEITEM, typeof(TradeSale), typeof(short))]
        [Choose((int)Commands.CMD_CM_MISSION, typeof(Mission), typeof(short))]
        public NPCTalk TalkType { get; set; }
    }
}
