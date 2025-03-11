namespace TOP_Network.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequiredClassAttribute : Attribute
    {
        public RequiredClassAttribute(Type classType, string allias = "")
        {
            ClassType = classType;
            if(string.IsNullOrEmpty(allias))
            {
                allias = classType.Name;
            }
            Allias = allias;
        }

        public Type ClassType { get; set; }
        public string Allias { get; set; }
    }
}
