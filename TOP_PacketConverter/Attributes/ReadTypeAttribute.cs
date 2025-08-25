namespace TOP_PacketConverter.Attributes
{
    public class ReadTypeAttribute : Attribute
    {
        public Type ReadType { get; set; }

        public ReadTypeAttribute(Type readType)
        {
            ReadType = readType;
        }
    }
}
