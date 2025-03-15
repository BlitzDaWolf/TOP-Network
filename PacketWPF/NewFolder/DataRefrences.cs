using System.Windows.Controls;

namespace PacketWPF.NewFolder
{
    public class DataRefrences
    {
        public byte Data { get; set; }

        public string Display => BitConverter.ToString([Data]);

        public GroupRefrence Group { get; set; }
        public Label LabelRefrence { get; set; }
    }
}
