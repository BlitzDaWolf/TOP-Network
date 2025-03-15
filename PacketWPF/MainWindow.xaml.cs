using PacketWPF.NewFolder;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TOP_Network.Converter;
using TOP_Network.Enum;
using TOP_Network.Exceptions;
using TOP_Network.Packets;
using TOP_Packets.Server;
using TOP_Records.Tables;

namespace PacketWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<GroupRefrence> Refrences { get; set; }
        public List<Packet> Notifications { get; }
        public int offset { get; set; } = 0;

        public MainWindow()
        {
            InitializeComponent();

            PacketToClass.AddType<MissionLog>(Commands.CMD_MC_MISLOG);
            PacketToClass.AddType<MissionPage>(Commands.CMD_MC_MISPAGE);
            PacketToClass.AddType<MissionLogInfo>(Commands.CMD_MC_MISLOGINFO);
            PacketToClass.AddType<NpcStateChange>(Commands.CMD_MC_NPCSTATECHG);
            PacketToClass.AddType<FuncPage>(Commands.CMD_MC_FUNCPAGE);
            PacketToClass.AddType<SystemInformation>(Commands.CMD_MC_SYSINFO);
            PacketToClass.AddType<Notification>(Commands.CMD_MC_NOTIACTION);

            PacketReader.OnRead = OnRead;
            Run();
        }

        public async Task Run()
        {
            var files = Directory.GetDirectories(@"D:\dev\DecryptFinal\DecryptFinal\bin\Debug\net8.0\packets").SelectMany(Directory.GetFiles).ToList();
            while (files.Count > 0)
            {
                try
                {
                    var pkt = new Packet(File.ReadAllBytes(files[0]));
                    var r = pkt.Convert();
                    await Task.Delay(1000);
                }
                catch
                {

                }
                files.RemoveAt(0);
            }
        }

        private void OnRead(byte[] obj)
        {
            GroupRefrence groupRefrence = new GroupRefrence();
            for (int i = 0; i < obj.Length; i++)
            {
                groupRefrence.Data.Add(new DataRefrences { Data = obj[i], Group = groupRefrence });
            }
            viewer.add(groupRefrence);
        }
    }
}