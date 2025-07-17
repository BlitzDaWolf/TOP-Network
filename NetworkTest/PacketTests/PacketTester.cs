using TOP_Network.Enum;
using TOP_Network.Packets;

namespace NetworkTest.PacketTests
{
    public class PacketTester
    {
        [Fact]
        public void Fact_NoReader()
        {
            Packet pkt = new Packet();
            Assert.Throws<Exception>(() => pkt.GetStream());
        }

        [Fact]
        public void Fact_Readpacket()
        {
            Packet packet = new Packet([0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x03, 0xA3]);
            Assert.Equal(Commands.CMD_MC_LOGIN, packet.Command);
        }

        [Fact]
        public void Fact_DisplayHex()
        {
            Packet packet = new Packet([0x00, 0x00, 0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x03, 0xA3]);
            Assert.Equal("00 00 00 0A 80 00 00 00 03 A3", packet.DisplayHex());
        }

        [Fact]
        public void Fact_ValidGnack()
        {
            Packet packet = new Packet([0x00, 0x00, 0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x03, 0xA3]);
            Assert.True(packet.ValidGnack);
        }
        [Fact]
        public void Fact_InValidGnack()
        {
            Packet packet = new Packet([0x00, 0x00, 0x00, 0x0A, 0x08, 0x00, 0x00, 0x00, 0x03, 0xA3]);
            Assert.False(packet.ValidGnack);
        }
    }
}
