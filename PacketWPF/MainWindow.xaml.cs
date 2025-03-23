using PacketWPF.NewFolder;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TOP_Network.Converter;
using TOP_Network.Enum;
using TOP_Network.Exceptions;
using TOP_Network.Packets;
using TOP_Packets;
using TOP_Packets.Client;
using TOP_Packets.GroupServer;
using TOP_Packets.Server;
using TOP_Packets.Server.MissionLogs;
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

            ServerSideRegister.Register();
            GroupSideRegister.Register();
            ClientSideRegister.Register();

            Run();
        }

        public async Task Run()
        {
            RecordsConfig.SetBasePath("D:\\Program Files (x86)\\Pirate King Online\\scripts\\table");
            ItemTable itemTable = new ItemTable();
            itemTable.Init("iteminfo.bin");

            var files = Directory.GetFiles(@"D:\dev\DecryptFinal\DecryptFinal\bin\Debug\net8.0\packets")
                .ToList();
            /*treeView.ItemsSource = files.Select(x => "CMD_CM_REQUESTTALK\\"+ x.Split("\\").Last());
            PacketReader.OnRead = OnRead;
            return;*/

            List<byte[]> missed = new List<byte[]>();

            while (files.Count > 0)
            {
                try
                {
                    if (files.Count % 250 == 0)
                    {
                        await Task.Delay(100);
                    }
                    // await Task.Delay(10);
                    
                    var pkt = new Packet(File.ReadAllBytes(files[0]).Take(12).ToArray());
                    if (!PacketToClass.HasCommand(pkt.Command))
                    {
                        //files.RemoveAt(0);
                        //continue;
                    }
                    /*if((/*pkt.Command == Commands.CMD_MC_CHABEGINSEE || * /pkt.Command == Commands.CMD_MC_NOTIACTION ||* / pkt.Command == Commands.CMD_MC_TEAM))
                    {
                        files.RemoveAt(0);
                        continue;
                    }*/
                    // if (PacketToClass.HasCommand(pkt.Command) && pkt.Command == Commands.CMD_MC_CHABEGINSEE)
                    {
                        pkt = new Packet(File.ReadAllBytes(files[0]));
                        var r = pkt.Convert();
                        File.Delete(files[0]);
                        files.RemoveAt(0);
                        continue;
                    }
                    /*else
                    {
                        files.RemoveAt(0);
                        continue;
                    }*/
                }
                catch(NotFullyReadException e)
                {
                    // missed.Add((byte[])e.Packet);
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
                treeView.Items.Add(string.Join("\\", files[0].Split("\\").TakeLast(1)));
                files.RemoveAt(0);
            }
            missed.Take(25).ToList().ForEach(OnRead);
            PacketReader.OnRead = OnRead;
            MessageBox.Show("Done");
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
                classV.Items.Add("Command: " + pkt.Command);
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
                /*for(int i = 1; i < t.Length; i++)
                {
                    var l = t.Length - (i);
                    var tst = l / 19d;
                    var aa = t[i - 1];

                    if(tst % 1 == 0)
                    {

                    }

                    if(aa == tst)
                    {

                    }
                }*/
                Display(t);
                /*var properties = t.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(t);
                    if (value.GetType().IsArray)
                    {
                        Array a = (Array)value;
                        for (int i = 0; i < a.Length; i++)
                        {

                        }
                    }
                    else
                    {
                        classV.Items.Add($"{property.Name}: {value}");
                    }
                }*/
            }
            catch { }
        }

        public void Display(object o, string b="")
        {
            if (o == null)
            {
                classV.Items.Add($"{b}: null");
                return;
            }
            try
            {
                var properties = o.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(o);
                    if (value == null)
                    {
                        classV.Items.Add($"{b}{property.Name}: null");
                        continue;
                    }
                    if (value.GetType().IsArray)
                    {
                        Array a = (Array)value;
                        for (int i = 0; i < a.Length; i++)
                        {
                            Display(a.GetValue(i), $"{b}{property.Name}[{i}].");
                        }
                    }
                    else
                    {
                        if (value.GetType().Assembly.GetName().Name.Contains("TOP") && !property.PropertyType.IsEnum)
                        {
                            Display(value, $"{b}{property.Name}.");
                        }
                        else
                        {
                            classV.Items.Add($"({property.PropertyType.Name}) {b}{property.Name}: {value}");
                        }
                    }
                }
            }
            catch { }
        }
    }
}