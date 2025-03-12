using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOP_Network.Exceptions
{
    public class WrongTypeExcetion : Exception
    {
        public WrongTypeExcetion(string value)
            : base(value)
        {
            
        }
    }
}
