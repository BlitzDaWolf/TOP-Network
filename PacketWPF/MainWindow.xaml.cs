using PacketWPF.NewFolder;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TOP_Network.Converter;
using TOP_Network.Enum;
using TOP_Network.Exceptions;
using TOP_Network.Packets;
using TOP_Packets.Client;
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

            // Done
            PacketToClass.AddType<Notification>(Commands.CMD_MC_NOTIACTION);
            PacketToClass.AddType<FuncPage>(Commands.CMD_MC_FUNCPAGE);
            PacketToClass.AddType<MissionLog>(Commands.CMD_MC_MISLOG);
            PacketToClass.AddType<MissionPage>(Commands.CMD_MC_MISPAGE);
            PacketToClass.AddType<MissionLogInfo>(Commands.CMD_MC_MISLOGINFO);
            PacketToClass.AddType<NpcStateChange>(Commands.CMD_MC_NPCSTATECHG);
            PacketToClass.AddType<SystemInformation>(Commands.CMD_MC_SYSINFO);
            PacketToClass.AddType<SyncAtt>(Commands.CMD_MC_SYNATTR);
            PacketToClass.AddType<SyncSkillState>(Commands.CMD_MC_SYNASKILLSTATE);
            PacketToClass.AddType<AStateBeginSee>(Commands.CMD_MC_ASTATEBEGINSEE);

            PacketToClass.AddType<CharacterEndSee>(Commands.CMD_MC_CHAENDSEE);
            PacketToClass.AddType<ItemEndSee>(Commands.CMD_MC_ITEMENDSEE);

            PacketToClass.AddType<BeginAction>(Commands.CMD_CM_BEGINACTION);
            PacketToClass.AddType<BeginPlay>(Commands.CMD_CM_BGNPLAY);
            PacketToClass.AddType<ClientPing>(Commands.CMD_CM_CHECK_PING);
            PacketToClass.AddType<DieReturn>(Commands.CMD_CM_DIE_RETURN);
            PacketToClass.AddType<MapMask>(Commands.CMD_CM_MAP_MASK);
            PacketToClass.AddType<RequestTalk>(Commands.CMD_CM_REQUESTTALK);

            PacketToClass.AddType<ItemBeginSee>(Commands.CMD_MC_ITEMBEGINSEE);
            PacketToClass.AddType<AsteEndSee>(Commands.CMD_MC_ASTATEENDSEE);

            // Look
            PacketToClass.AddType<CharacterBeginSee>(Commands.CMD_MC_CHABEGINSEE);

            Run();
        }

        public async Task Run()
        {
            RecordsConfig.SetBasePath("D:\\Program Files (x86)\\Pirate King Online\\scripts\\table");
            ItemTable itemTable = new ItemTable();
            itemTable.Init("iteminfo.bin");

            var files = Directory.GetDirectories(@"D:\dev\DecryptFinal\DecryptFinal\bin\Debug\net8.0\packets").SelectMany(x => Directory.GetFiles(x))
                .Where(x => !x.Contains("CMD_MC_NOTIACTION"))
                .Where(x => !x.Contains("CMD_MC_FUNCPAGE"))
                .Where(x => !x.Contains("CMD_MC_MISLOG"))
                .Where(x => !x.Contains("CMD_MC_MISPAGE"))
                .Where(x => !x.Contains("CMD_MC_MISLOGINFO"))
                .Where(x => !x.Contains("CMD_MC_NPCSTATECHG"))
                .Where(x => !x.Contains("CMD_MC_SYSINFO"))
                .Where(x => !x.Contains("CMD_MC_SYNATTR"))
                .Where(x => !x.Contains("CMD_MC_SYNASKILLSTATE"))
                .Where(x => !x.Contains("CMD_MC_ASTATEBEGINSEE"))
                .Where(x => !x.Contains("CMD_MC_CHAENDSEE"))
                .Where(x => !x.Contains("CMD_MC_ITEMENDSEE"))
                .Where(x => !x.Contains("CMD_CM_BEGINACTION"))
                .Where(x => !x.Contains("CMD_CM_BGNPLAY"))
                .Where(x => !x.Contains("CMD_CM_CHECK_PING"))
                .Where(x => !x.Contains("CMD_CM_DIE_RETURN"))
                .Where(x => !x.Contains("CMD_CM_MAP_MASK"))
                .Where(x => !x.Contains("CMD_CM_REQUESTTALK"))
                .Where(x => !x.Contains("CMD_MC_CHABEGINSEE"))

                .Where(x => !x.Contains("CMD_MC_CHABEGINSEE"))
                .ToList();
            // treeView.ItemsSource = files.Select(x => x.Split("\\").Last());
            // return;
            while (files.Count > 0)
            {
                try
                {
                    /*if (files.Count % 250 == 0)
                    {
                        await Task.Delay(100);
                    }*/
                    // await Task.Delay(10);
                    
                    var pkt = new Packet(File.ReadAllBytes(files[0]).Take(12).ToArray());
                    if (PacketToClass.HasCommand(pkt.Command))
                    {
                        pkt = new Packet(File.ReadAllBytes(files[0]));
                        var r = pkt.Convert();
                        files.RemoveAt(0);
                        continue;
                    }
                }
                catch(NotFullyReadException e)
                {
                    /*if (((Notification)e.Packet).ActionType is SkillTar)
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
                treeView.Items.Add(string.Join("\\", files[0].Split("\\").TakeLast(2)));
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
                var pkt = new Packet(File.ReadAllBytes(@"D:\dev\DecryptFinal\DecryptFinal\bin\Debug\net8.0\packets\" + v));
                var r = pkt.Convert()!;
                var t = r;
                var properties = t.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(t);
                    classV.Items.Add($"{property.Name}: {value}");
                }
            }
            catch (NotFullyReadException ex)
            {
                var t = (ex.Packet);
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