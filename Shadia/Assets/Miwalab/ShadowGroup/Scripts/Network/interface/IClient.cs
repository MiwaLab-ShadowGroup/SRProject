using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.Network
{
    public interface IClient
    {

    }

    public interface ICIPCClient : IClient
    {
        /// <summary>
        /// 自身の情報
        /// </summary>
        /// <param name="name"></param>
        /// <param name="port"></param>
        void Bind(int port, string CIPCServerIP, int CIPCServerPort, int fps, string clientName);

        void Connect(CIPC_CS_Unity.CLIENT.MODE mode);

        /// <summary>
        /// ファイナライズ
        /// </summary>
        void Close();
    }
}
