namespace TOP_Network.Attributes
{
    public class ArrayLengthAttribute : Attribute
    {
        public int Length { get; set; }

        public ArrayLengthAttribute(int length)
        {
            Length = length;
        }
    }
}
