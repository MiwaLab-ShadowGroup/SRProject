using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.ImageProcesser.Network
{
    public abstract class ANetworkShadowImageProcesser : AShadowImageProcesser
    {
        private Miwalab.ShadowGroup.Thread.ThreadHost _tHost;
        private Miwalab.ShadowGroup.Network.NetworkHost _nHost;
        public void SetupHosts()
        {
            _nHost = ShadowGroup.Network.NetworkHost.GetInstance();
            _tHost = ShadowGroup.Thread.ThreadHost.GetInstance();
            
        }

    }
}
