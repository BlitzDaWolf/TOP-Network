using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;
using TOP_Records.Tables;

namespace TOP_Packets.Server
{
    public abstract class NotificationAction;

    public class MoveAction : NotificationAction
    {
        [Description("Movement State")]
        public short State { get; set; }

        [NotIf("State", (short)00)]
        public short A { get; set; }

        [Description("Movement point (Currentspot -> Movespot)")]
        public byte[] Points { get; set; }
    }

    public class StopState : NotificationAction
    {
        public int State { get; set; }
    }
    public class SkillPoseAction : NotificationAction
    {
        public short Angle { get; set; }
        public short Pose { get; set; }
    }
    public class LookAction : NotificationAction
    {
        public byte SyncType { get; set; }
        public short TypeID { get; set; }
        public short HairID { get; set; }
        public short A { get; set; }
        public short B { get; set; }
        public short C { get; set; }
        public short D { get; set; }
        public short E { get; set; }
    }
    public class SkillSrc : NotificationAction
    {
        public abstract class SkillState;
        public class SkillStateSelf : SkillState
        {
            public uint X { get; set; }
            public uint Y { get; set; }

            public override string ToString() => $"(X={X / 100d}, Y={Y / 100d})";
        }
        public class SkillStateOther : SkillState
        {
            public uint TargetID { get; set; }
            public uint X { get; set; }
            public uint Y { get; set; }
            public override string ToString() => $"(TargetID={TargetID}, X={X / 100d}, Y={Y / 100d})";
        }

        public byte FightId { get; set; }
        public short Angle { get; set; }
        public short State { get; set; }
        [NotIf("State", (short)0)]
        public short StopState { get; set; }

        [Description("Used skill")]
#if !DEBUG
        [ValidRecord(typeof(SkillInfoTable))]
#endif
        public uint SkillId { get; set; }
        [SmallEndean]
        public long Speed { get; set; }

        [Choose(1, typeof(SkillStateOther))]
        [Choose(2, typeof(SkillStateSelf))]
        public SkillState Skill { get; set; }
        public short ExecTime { get; set; }

        [ArraySize(typeof(short))]
        public Effect[] Effects { get; set; }

        public short StateNumber { get; set; }
    }
    public class SkillState
    {
        public short A { get; set; }
        public byte Actin { get; set; }
        [SmallEndean]
        public long LA { get; set; }
        public long LB { get; set; }

    }
    public class SkillTar : NotificationAction
    {

        public byte FightId { get; set; }
        public short State { get; set; }

        public bool DoubleAttack { get; set; }
        public bool Miss { get; set; }
        public bool BeatBack { get; set; }

        public uint SrcID { get; set; }
        public uint X { get; set; }
        public uint Y { get; set; }


        public int SkillID { get; set; }
        //public byte Skip { get; set; }

        public int TarX { get; set; }
        public int TarY { get; set; }

        public ushort ExecTime { get; set; }

        public byte Skip { get; set; }
        [ArraySize(typeof(short))]
        public Effect[] Effects { get; set; }

        /*public bool IsSkillState { get; set; }
        [If("IsSkillState", true)]
        [ArraySize(typeof(short))]
        public SkillState[] SkillState { get; set; }


        public bool IsSrcEffects { get; set; }

        [If("IsSrcEffects", true)]
        public short SrcState { get; set; }
        [If("IsSrcEffects", true)]
        public byte SrcSync { get; set; }
        [If("IsSrcEffects", true)]
        [ArraySize(typeof(short))]
        public Effect[] SrcEffects { get; set; }

        /*public bool A { get; set; }
        [If("A", true)]
        public short Test { get; set; }*/

    }

    public class ItemFailed : NotificationAction
    {
        public short ItemID { get; set; }
    }
    public class PKControll : NotificationAction
    {
        public byte A { get; set; }
    }

    public class Notification
    {
        [Description("Wich entity should be using this Action (`ComunicationID`)")]
        public uint EntityId { get; set; }
        [Description("Used to show update to an client sent packet")]
        public int packetID { get; set; }

        [Choose(1, typeof(MoveAction))]
        [Choose(3, typeof(SkillSrc))]
        [Choose(4, typeof(SkillTar))]
        [Choose(5, typeof(LookAction))]
        // [Choose(6, typeof(LookAction))]
        [Choose(15, typeof(ItemFailed))]
        [Choose(21, typeof(StopState))]
        [Choose(22, typeof(PKControll))]
        [Description("Action")]
        public NotificationAction ActionType { get; set; }
    }
}
