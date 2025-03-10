using System.Reflection;

namespace TOP_Network.Converter
{
    public static class ClassList
    {
        public static void GetList<T>()
        {
            Dictionary<string, PropertyInfo> v = new Dictionary<string, PropertyInfo>();
            GetList(typeof(T), v);
        }

        public static void GetList(Type type, Dictionary<string, PropertyInfo> v, string baseName = "")
        {
            var properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType.IsArray)
                {
                    if (!property.PropertyType.GetElementType()!.IsPrimitive)
                    {
                        GetList(property.PropertyType.GetElementType()!, v, $"{baseName}{property.Name}[].");
                    }
                    else
                    {
                        v.Add(baseName + $"{baseName}{property.Name}[]", property);
                    }
                }
                else
                {
                    if (!property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                    {
                        GetList(property.PropertyType, v, $"{baseName}{property.Name}.");
                    }
                    else
                    {
                        v.Add(baseName + property.Name, property);
                    }
                }
            }
        }
    }
}
