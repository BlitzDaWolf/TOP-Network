using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;
using TOP_Packets.Server;

namespace TOP_Packets.Client
{
    public class ClientMove : NotificationAction
    {
        public byte[] Points { get; set; }
    }
    
    public class SkillAction : NotificationAction
    {
        public byte Move { get; set; }
        public byte FightID { get; set; }
        public byte[] Points { get; set; }

        public uint SkillID { get; set; }
        public uint TargetID { get; set; }
        public uint TargetHandle { get; set; }
    }

    public class ItemPickUp : NotificationAction
    {
        public uint WorldID { get; set; }
        public uint Handle { get; set; }
    }

    public class SkillPose : NotificationAction
    {
        public short Angle { get; set; }
        public short Pose { get; set; }
    }

    public class ItemUse : NotificationAction
    {
        public short GridID { get; set; }
        public short Left { get; set; }
    }

    public class ItemPosition : NotificationAction
    {
        public short SourceGridId { get; set; }
        public short SourceNumber { get; set; }
        public short TargetGridId { get; set; }
    }
    public class ItemUnfixNotification : NotificationAction
    {
        public byte LinkId { get; set; }
        public short GridID { get; set; }

        [If("GridID", (short)-1)]
        public uint X { get; set; }
        [If("GridID", (short)-1)]
        public uint Y { get; set; }
    }
    public class EventNotification : NotificationAction
    {
        public uint TargetID { get; set; }
        public uint Handle { get; set; }
        public short EventID { get; set; }
    }

    public class BeginAction
    {
        public uint CharacterID { get; set; }
        public uint PacketID { get; set; }

        [Choose(1, typeof(ClientMove))]
        [Choose(2, typeof(SkillAction))]
        [Choose(0x08, typeof(ItemPickUp))]
        [Choose(0x0B, typeof(ItemUse))]
        [Choose(0x0C, typeof(ItemPosition))]
        [Choose(0x15, typeof(SkillPose))]
        [Choose(18, typeof(EventNotification))]
        [Choose(0x0A, typeof(ItemUnfixNotification))]
        [Description("Action")]
        public NotificationAction ActionType { get; set; }
    }
}
