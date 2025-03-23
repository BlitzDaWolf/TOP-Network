using System.Reflection.Emit;

namespace TOP_Network.Extention
{
    public static class TypeExtention
    {
        public static int SizeOf(this Type type)
        {
            var dynamicMethod = new DynamicMethod("SizeOf", typeof(int), Type.EmptyTypes);
            var generator = dynamicMethod.GetILGenerator();

            generator.Emit(OpCodes.Sizeof, type);
            generator.Emit(OpCodes.Ret);

            var function = (Func<int>)dynamicMethod.CreateDelegate(typeof(Func<int>));
            return function();
        }

        public static object? GetValue(this object o, string search)
        {
            var properties = o.GetType().GetProperties().FirstOrDefault(x => x.Name == search);
            if (properties == null) return null;
            return properties.GetValue(o);
        }
    }
}
