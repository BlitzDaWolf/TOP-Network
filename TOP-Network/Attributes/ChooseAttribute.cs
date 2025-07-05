namespace TOP_Network.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ChooseAttribute : Attribute
    {
        public ChooseAttribute(int value, Type dataType) : this(value, dataType, typeof(byte)) { }

        public ChooseAttribute(int value, Type dataType, Type readType)
        {
            Value = value;
            DataType = dataType;
            ReadType = readType;
        }

        public int Value { get; set; }
        public Type DataType { get; set; }
        public Type ReadType { get; set; }
    }
}
