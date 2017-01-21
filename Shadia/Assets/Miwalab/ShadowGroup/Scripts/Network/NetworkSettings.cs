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
            public readonly static int SendClient1Port = 15001;
            public readonly static int SendClient2Port = 15002;
            public readonly static int ReceiveClient1Port = 15003;
            public readonly static int ReceiveClient2Port = 15004;

            public readonly static int SendMeshClient1Port = 15005;
            public readonly static int SendMeshClient2Port = 15006;
            public readonly static int ReceiveMeshClient1Port = 15007;
            public readonly static int ReceiveMeshClient2Port = 15008;

            public readonly static int ReceiveControlPort = 15009;
            public readonly static int SendControlPort = 15010;



        }
        #endregion
    }
}
