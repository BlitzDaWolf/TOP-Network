using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOP_Network.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ArraySizeAttribute : Attribute
    {
        public Type ReadType { get; set; }

        public ArraySizeAttribute(Type readType)
        {
            ReadType = readType;
        }
    }
}
