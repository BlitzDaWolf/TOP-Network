using Test;
using TOP_Network.Converter;
using TOP_Network.Packets;
using TOP_Records;
using TOP_Records.Tables;

var v = File.ReadAllBytes(@"D:\dev\Decrypt\Decrypt\bin\Debug\net8.0\Packets\server-3.packet");

RecordsConfig.SetBasePath("D:\\Program Files (x86)\\Pirate King Online\\scripts\\table");
ItemTable itemTable = new ItemTable();
itemTable.Init("iteminfo.bin");

var pkt = new Packet(v);
var log = pkt.Convert<Login>();
var r = log.Convert(pkt.Command);

r.DisplayHex();

var t = "";