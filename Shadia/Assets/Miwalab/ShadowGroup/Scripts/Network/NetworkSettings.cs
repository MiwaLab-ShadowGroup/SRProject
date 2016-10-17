using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.Network
{
    public class NetworkSettings
    {
        #region 
        public struct NetworkSetting
        {
            public NetworkSetting(string TAG, int PORT)
            {
                this.TAG = TAG;
                this.PORT = PORT;
            }

            public string TAG;
            public int PORT;
        }
        #endregion

        #region SETTINGS
        public class SETTINGS
        {
            public readonly static NetworkSetting RemoteShadowImageManager_Sender
                = new NetworkSetting(
                    "RemoteShadowImageManager_Sender",
                    15000
                    );

            public readonly static NetworkSetting RemoteShadowImageManager_Receiver
                = new NetworkSetting(
                    "RemoteShadowImageManager_Receiver",
                    15001
                );
        }
        #endregion
    }
}
