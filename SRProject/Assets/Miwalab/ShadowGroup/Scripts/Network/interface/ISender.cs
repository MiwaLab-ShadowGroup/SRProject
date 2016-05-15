using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Miwalab.ShadowGroup.Network
{
    public interface ISender: IClient
    {
        void SendTo(byte[] data, IPEndPoint to);
        void SendTo(byte[] data, IPEndPoint[] to);
        void SendTo(byte[] data, List<IPEndPoint> to);

        void BindPortRandomly();
    }
}
