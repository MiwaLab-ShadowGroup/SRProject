﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Miwalab.ShadowGroup.Network
{
    public interface ISender: ICIPCClient
    {
        void Send(byte[] data);
    }
}
