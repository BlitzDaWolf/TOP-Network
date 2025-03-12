using TOP_Records;

namespace NetworkTest.TestClasses
{
    public class Centry : TOP_Records.Record
    {
        public int Value { get; set; }
    }

    public class CentryTable : RecordReader<Centry>
    {
        public override void Read(Centry data, BinaryReader reader)
        {
            data.Value = reader.ReadInt32();
        }
    }
}
