using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Miwalab.ShadowGroup.Network
{
    public class Client : ISenderAndReciever
    {
        private UdpClient m_client;
        private IPEndPoint m_remoteEP;
        private int? portNum { set; get; }
        /// <summary>
        /// IPAdressを指定して初期化
        /// </summary>
        /// <param name="port"></param>
        public Client(int port)
        {
            BindPort(port);
            portNum = port;
        }

        /// <summary>
        /// IPAdressを指定せずに初期化
        /// </summary>
        public Client()
        {
            BindPortRandomly();
            portNum = null;
        }

        /// <summary>
        /// バインド すべてのリモートから受信
        /// </summary>
        /// <param name="port"></param>
        public void BindPort(int port)
        {
            m_client = new UdpClient(port);
            m_remoteEP = new IPEndPoint(IPAddress.Any, 0);
        }

        public void BindPortRandomly()
        {
            m_client = new UdpClient();
            m_remoteEP = new IPEndPoint(IPAddress.Any, 0);
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
            //毎度初期化しないと限定されてしまう．なんという危険物
            m_remoteEP = new IPEndPoint(IPAddress.Any, 0);
            return m_client.Receive(ref m_remoteEP);
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

        public void SendRemote(byte[] data)
        {
            this.m_client.Send(data, data.Length, m_remoteEP);
        }

        public void SendTo(byte[] data, List<IPEndPoint> to)
        {
            foreach(var p in to)
            {
                this.SendTo(data, p);
            }

        }

        public void SendTo(byte[] data, IPEndPoint[] to)
        {
            foreach (var p in to)
            {
                this.SendTo(data, p);
            }
        }

        public void SendTo(byte[] data, IPEndPoint to)
        {
            this.m_client.Send(data, data.Length, to);
        }

    }
}
