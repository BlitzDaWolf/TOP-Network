using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace TOP_Network.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IfAttribute : Attribute
    {
        public string v1;
        public object Target;

        public IfAttribute(string v1, object target)
        {
            this.v1 = v1;
            Target= target;
        }

        public virtual bool A(object value)
        {
            if (value.GetType() != Target.GetType()) return false;
            var h = value.GetHashCode();
            return Target.GetHashCode() == h;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NotIfAttribute : IfAttribute
    {
        public NotIfAttribute(string v1, object target) : base(v1, target) { }

        public override bool A(object value) => !base.A(value);
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class StopIfAttribute : Attribute
    {
        public StopIfAttribute(string v1, object target) { }
    }
}
