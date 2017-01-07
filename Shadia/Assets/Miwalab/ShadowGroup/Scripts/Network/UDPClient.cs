using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Miwalab.ShadowGroup.Network
{
    public class UDPClient : IClient
    {
        private System.Net.Sockets.UdpClient m_client;
        private int? portNum { set; get; }
        /// <summary>
        /// IPAdressを指定して初期化
        /// </summary>
        /// <param name="port"></param>
        public UDPClient(int port)
        {
            Bind(port);
            portNum = port;
        }



        /// <summary>
        /// バインド すべてのリモートから受信
        /// </summary>
        /// <param name="port"></param>
        public void Bind(int port)
        {
            m_client = new System.Net.Sockets.UdpClient(port);
        }
        
        /// <summary>
        /// 終了
        /// </summary>
        public void Close()
        {
            m_client.Close();
        }

        /// <summary>
        /// 受信する
        /// 受信するまでスレッド停止
        /// </summary>
        /// <returns></returns>
        public byte[] Receive()
        {
            //全部受信
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            //毎度初期化しないと限定されてしまう．なんという危険物
            return m_client.Receive(ref remoteEP);
        }

        /// <summary>
        /// 受信バッファにデータがあるときのみ受信
        /// </summary>
        /// <param name="availe"></param>
        /// <returns></returns>
        public byte[] Receive(ref int availe)
        {
            availe = this.m_client.Available;
            if (availe > 0)
            {
                return this.Receive();
            }
            return null;
        }

        public void Send(byte[] data, IPEndPoint target)
        {
            this.m_client.Send(data,data.Length, target);
        }



    }
}
