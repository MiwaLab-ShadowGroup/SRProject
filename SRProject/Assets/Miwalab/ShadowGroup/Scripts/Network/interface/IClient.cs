using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.Network
{
    public interface IClient
    {
        /// <summary>
        /// 自身の情報
        /// </summary>
        /// <param name="name"></param>
        /// <param name="port"></param>
        void BindPort(int port);
        /// <summary>
        /// ファイナライズ
        /// </summary>
        void Close();
    }
}
