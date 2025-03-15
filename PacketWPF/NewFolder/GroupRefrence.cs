using System.Windows.Media;

namespace PacketWPF.NewFolder
{
    public class GroupRefrence
    {
        public List<DataRefrences> Data { get; set; } = new List<DataRefrences>();

        public Brush b { get; set; } = new SolidColorBrush {
            Color = new Color {
                R = (byte)Random.Shared.Next(128, 255),
                G = (byte)Random.Shared.Next(128, 255),
                B = (byte)Random.Shared.Next(128, 255),
                A = 255
            }
        };
    }
}
