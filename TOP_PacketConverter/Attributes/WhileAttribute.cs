namespace TOP_PacketConverter.Attributes
{
    public class WhileAttribute : IfAttribute
    {
        public WhileAttribute(int max, object target, Type readType) : base("", target)
        {
            Max = max;
            ReadType = readType;
        }

        public int Max { get; set; }
        public Type ReadType { get; set; }
    }
    public class WhileNotAttribute : WhileAttribute
    {
        public WhileNotAttribute(int max, object target, Type readType) : base(max, target, readType)
        {
        }

        public override bool A(object value) => !base.A(value); 
    }
}
