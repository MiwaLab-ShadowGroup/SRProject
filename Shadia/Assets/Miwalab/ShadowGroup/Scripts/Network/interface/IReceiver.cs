using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.Network
{
    public interface IReceiver:IClient
    {
        /// <summary>
        /// データを受信するまでスレッド停止
        /// </summary>
        /// <returns></returns>
        byte[] Receive();

        /// <summary>
        /// 受信
        /// 受信可能なら受信
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        byte[] Receive(ref int availe);
        

    }
}
