namespace TOP_Network.Exceptions
{
    public class BinaryException : Exception
    {
        public Type RefrenceType { get; set; }

        public BinaryException(Type refrenceType)
        {
            RefrenceType = refrenceType;
        }
    }
}
