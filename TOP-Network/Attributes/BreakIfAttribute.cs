using System.Runtime.InteropServices.ObjectiveC;

namespace TOP_Network.Attributes
{
    public class BreakIfAttribute : Attribute
    {
        public readonly object v;

        public BreakIfAttribute(object v)
        {
            this.v = v;
        }

        public virtual bool Check(object a)
        {
            return a.GetHashCode() == v.GetHashCode();
        }
    }
    public class BreakNotIfAttribute : BreakIfAttribute
    {
        public BreakNotIfAttribute(object v) : base(v) { }
        public override bool Check(object a) => !base.Check(a);
    }
}