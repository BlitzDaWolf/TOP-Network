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
using TOP_Records;
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
            PacketToClass.AddType<CharacterBeginSee>(Commands.CMD_MC_CHABEGINSEE);

            Run();
        }

        public async Task Run()
        {
            RecordsConfig.SetBasePath("D:\\Program Files (x86)\\Pirate King Online\\scripts\\table");
            ItemTable itemTable = new ItemTable();
            itemTable.Init("iteminfo.bin");

            var files = Directory.GetFiles(@"D:\dev\DecryptFinal\DecryptFinal\bin\Debug\net8.0\packets\CMD_MC_NOTIACTION")
                .Where(x => x.EndsWith(".packet"))
                .ToList();
            // treeView.ItemsSource = files.Select(x => x.Split("\\").Last());
            // return;
            while (files.Count > 0)
            {
                try
                {
                    if (files.Count % 500 == 0)
                    {
                        await Task.Delay(10);
                    }
                    // await Task.Delay(10);
                    viewer.Reset();
                    classV.Items.Clear();
                    var pkt = new Packet(File.ReadAllBytes(files[0]));
                    var r = pkt.Convert();

                    if(((Notification)r).ActionType is SkillTar)
                    {
                        File.Move(files[0], files[0].Replace(".packet", ".tar"));
                    }
                    files.RemoveAt(0);
                    continue;
                }
                catch(NotFullyReadException e)
                {

                    if (((Notification)e.Packet).ActionType is SkillTar)
                    {
                        File.Move(files[0], files[0].Replace(".packet", ".tar"));
                    }
                    if (((Notification)e.Packet).ActionType is SkillTar)
                    {
                        treeView.Items.Add(files[0].Split("\\").Last());
                    }
                    /*var t = ((CharacterBeginSee)e.Packet).Entity.Look.Items[9];
                    var properties = t.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(t);
                        classV.Items.Add($"{property.Name}: {value}");
                    }
                    break;*/
                }
                catch
                {
                    //treeView.Items.Add(files[0].Split("\\").Last());
                }
                treeView.Items.Add(files[0].Split("\\").Last());
                files.RemoveAt(0);
            }
            PacketReader.OnRead = OnRead;
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

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var v = e.NewValue as string;

            viewer.Reset();
            classV.Items.Clear();

            try
            {
                var pkt = new Packet(File.ReadAllBytes(@"D:\dev\DecryptFinal\DecryptFinal\bin\Debug\net8.0\packets\CMD_MC_NOTIACTION\" + v));
                var r = (Notification)pkt.Convert()!;
                var t = r.ActionType;
                var properties = t.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(t);
                    classV.Items.Add($"{property.Name}: {value}");
                }
            }
            catch (NotFullyReadException ex)
            {
                var t = ((Notification)ex.Packet).ActionType;
                if (t == null) return;
                var properties = t.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(t);
                    classV.Items.Add($"{property.Name}: {value}");
                }
            }
            catch { }
        }
    }
}