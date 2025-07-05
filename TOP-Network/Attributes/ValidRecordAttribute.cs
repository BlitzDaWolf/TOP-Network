using TOP_Records;

namespace TOP_Network.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidRecordAttribute : Attribute
    {
        public readonly Type RecoredTable;

        public ValidRecordAttribute(Type recoredTable)
        {
            if (recoredTable.IsAssignableFrom(typeof(RecordReader))) throw new Exception("This is not a valid `RecordReader` type");
            RecoredTable = recoredTable;
        }

        public Record? GetRecord(int id)
        {
            return RecorReaders.GetRecord(RecoredTable, id);
        }
    }
}
