using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using TOP_Network.Converter;
using TOP_Network.Enum;
using TOP_Network.Exceptions;
using TOP_Network.Packets;
using TOP_Packets;
using TOP_Packets.Client;
using TOP_Packets.Server;

namespace PacketReverse
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServerSideRegister.Register();
            ClientSideRegister.Register();

            var files = Directory.GetDirectories(@"D:\dev\DecryptFinal\DecryptFinal\bin\Debug\net8.0\").SelectMany(Directory.GetFiles).Where(x => x.EndsWith(".packet")).ToList();

            int totalSize = files.Count;
            int invalid = 0;
            int notRead = 0;

            while (files.Count > 0)
            {
                if (files[0].Contains("CMD_MC_CHABEGINSEE"))
                {

                }
                Console.Title = $"[{invalid}/{notRead}/{totalSize - (invalid + notRead)}]";
                var pkt = new Packet(File.ReadAllBytes(files[0]));
                if(pkt.Command == Commands.CMD_MC_HELPINFO)
                {

                }

                try
                {
                    if (pkt.Command == Commands.CMD_MC_NEWCHA)
                    {

                    }
                    var r = pkt.Convert();

                    LoginAccount account = null;

                    if (pkt.Command == Commands.CMD_MC_LOGIN)
                    {
                        var lr = (LoginResponse)r;
                        account = lr.Accounts[0];
                        Console.WriteLine(BitConverter.ToString(lr.ChatKey));
                    }
                    else if (pkt.Command == Commands.CMD_MC_ENDPLAY)
                    {
                        var lr = (EndPlay)r;
                        account = lr.Accounts[0];
                    }
                    if(account != null)
                    {
                        var links = account._Look.Links;
                    }
                    if (pkt.Command == Commands.CMD_MC_NOTIACTION)
                    {

                    }



                    notRead++;
                    // await Task.Delay(10);
                }
                catch (NotFullyReadException)
                {
                    notRead++;
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("CMD_"))
                    {
                        // Console.WriteLine(pkt.Size + "\t| " + e.Message);
                    }
                    //Console.WriteLine(e.Message);
                    invalid++;
                }
                // Console.WriteLine($"[{invalid}/{files.Count}/{totalSize}]");
                files.RemoveAt(0);
            }
            Console.WriteLine($"[{invalid}/{notRead}/{totalSize}]");
        }

        static void t()
        {
            var t = Assembly.GetAssembly(typeof(FuncPage))!.GetTypes();

            var basePath = @"D:\dev\PKO-Wiki\Network\Packets";

            List<string> shared = [];

            foreach (var item in t.Where(x => x.IsAbstract).ToList())
            {
                List<string> data = [$"# {item.Name}", ""];

                foreach (var imp in t.Where(x => x.BaseType == item))
                {
                    data.Add($"## {imp.Name}");
                    data.Add("");

                    var properties = imp.GetProperties();
                    data.Add($"|Name|Type|Description|");
                    data.Add($"|---|---|---|");
                    foreach (var p in properties)
                    {
                        var v = p.GetCustomAttribute<DescriptionAttribute>();
                        string dsc = v != null ? $"{v.Description}" : "";
                        if (p.PropertyType.Namespace!.Contains("System"))
                        {
                            data.Add($"|{p.Name}|{p.PropertyType.Name}|{dsc}|");
                        }
                        else
                        {
                            data.Add($"|{p.Name}|[{p.PropertyType.Name}](./{p.PropertyType.Name}.md)|{dsc}|");
                        }
                    }
                    data.Add("");
                    data.Add("");
                }

                var pth = Path.Combine(basePath, "shared", item.Name + ".md");
                File.WriteAllLines(pth, data);
            }

            foreach (var item in t.Where(x => !x.IsAbstract).ToList())
            {
                if (item.BaseType == typeof(object))
                {
                    List<string> data = [$"# {item.Name}", ""];

                    var properties = item.GetProperties();
                    data.Add($"|Name|Type|Description|");
                    data.Add($"|---|---|---|");
                    foreach (var p in properties)
                    {
                        var v = p.GetCustomAttribute<DescriptionAttribute>();
                        string dsc = v != null ? $"{v.Description}" : "";
                        if (p.PropertyType.Namespace!.Contains("System"))
                        {
                            data.Add($"|{p.Name}|{p.PropertyType.Name}|{dsc}|");
                        }
                        else
                        {
                            if (p.PropertyType.IsAbstract)
                            {
                                data.Add($"|{p.Name}|[{p.PropertyType.Name}](../shared/{p.PropertyType.Name.Replace("[]", "")}.md)|{dsc}|");
                            }
                            else
                            {
                                data.Add($"|{p.Name}|[{p.PropertyType.Name}](./{p.PropertyType.Name.Replace("[]", "")}.md)|{dsc}|");
                            }
                        }
                    }

                    var pth = Path.Combine(basePath, "all", item.Name + ".md");
                    File.WriteAllLines(pth, data);
                }
                else
                {

                }
            }
        }
    }
}