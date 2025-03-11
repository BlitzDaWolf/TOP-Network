// See https://aka.ms/new-console-template for more information
using Test;
using TOP_Network.Converter;

Console.WriteLine("Hello, World!");

var v = new LoginPacket
{
    Bill = "nobill\0",
    Username = "01234567\0",
    Password = "0123456789123",
    A1 = "013207b7-94e1-4065-877c-ad359900fd48\0",
    A2 = "18-C0-4D-9C-47-80-00-00\0",
    A3 = "0025_385B_91B0_BDC6\0",
    IP = "127.0.0.1\0",
    Version= 911,
    Version2 = 171
};
var pkt = v.Convert();

var o = pkt.Clone().Convert<LoginPacket>();

var properties = typeof(LoginPacket).GetProperties();
foreach (var property in properties)
{
    var valid = $"{property.GetValue(v)} == {property.GetValue(o)}";
    Console.WriteLine($"{property.Name}: {valid}");
}
