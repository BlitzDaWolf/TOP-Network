using Test;
using TOP_Network.Converter;
using TOP_Network.Packets;

var v = File.ReadAllBytes(@"D:\dev\Decrypt\Decrypt\bin\Debug\net8.0\Packets\server-3.packet");

var pkt = new Packet(v);
var log = pkt.Convert<Login>();
var r = log.Convert(pkt.Command);

r.DisplayHex();

var t = "";