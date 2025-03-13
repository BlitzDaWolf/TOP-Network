using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Packets;

namespace TOP_Network.Exceptions
{
    public class NotFullyReadException : Exception
    {
        public NotFullyReadException(Packet packet)
        {
            Packet = packet;
        }

        public Packet Packet { get; }
    }
}
