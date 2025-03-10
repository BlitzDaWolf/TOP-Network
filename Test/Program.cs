// See https://aka.ms/new-console-template for more information
using System.Buffers.Binary;
using Test;
using TOP_Network.Converter;
using TOP_Network.Extention;

Console.WriteLine("Hello, World!");

var t = new subsubclass();

t.MyProperty = 1;
BinaryWriter writer = new BinaryWriter(File.Open("./test.bin", FileMode.OpenOrCreate));
writer.WriteType(t.MyProperty);
writer.WriteType("Testing");

writer.Flush();