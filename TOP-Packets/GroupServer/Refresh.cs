using TOP_Network.Attributes;

namespace TOP_Packets.GroupServer
{
    public class Friend
    {
        public uint CharacterID { get; set; }
        public string Name { get; set; }
        public string Motto { get; set; }
        public short IconID { get; set; }
        public bool Online { get; set; }
    }

    public class FriendGroup
    {
        public string GroupName { get; set; }
        [ArraySize(typeof(short))]
        public Friend[] Friends { get; set; }
    }

    public abstract class RefreshType;

    public class RefreshStart : RefreshType
    {
        public uint CharacterID { get; set; }
        public string Name { get; set; }
        public string Motto { get; set; }
        public short IconID { get; set; }

        [ArraySize(typeof(short))]
        public FriendGroup[] GroupAmount { get; set; }
    }

    public class FriendOnline : RefreshType
    {
        public uint CharacterID { get; set; }
    }

    public class Refresh
    {
        [Choose(6, typeof(RefreshStart))]
        [Choose(1, typeof(RefreshStart))]
        [Choose(4, typeof(FriendOnline))]
        [Choose(5, typeof(FriendOnline))]
        public RefreshType Type { get; set; }
    }
}
