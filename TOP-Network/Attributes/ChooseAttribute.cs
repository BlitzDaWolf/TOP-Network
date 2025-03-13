namespace TOP_Network.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ChooseAttribute : Attribute
    {
        public ChooseAttribute(byte value, Type dataType)
        {
            Value = value;
            DataType = dataType;
        }

        public byte Value { get; set; }
        public Type DataType { get; set; }
    }
}
