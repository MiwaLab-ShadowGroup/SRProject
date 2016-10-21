using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CIPC_CS_Unity.CLIENT;
using UnityEngine;

namespace Miwalab.ShadowGroup.Network
{
    public class CIPCClient : ISenderAndReciever
    {
        private CIPC_CS_Unity.CLIENT.CLIENT m_client;
        private int? portNum { set; get; }
        /// <summary>
        /// IPAdressを指定して初期化
        /// </summary>
        /// <param name="port"></param>
        public CIPCClient(int port, string CIPCServerIP, int CIPCServerPort, int fps, string clientName )
        {
            Bind(port, CIPCServerIP, CIPCServerPort, fps, clientName);
            portNum = port;
        }

       

        /// <summary>
        /// バインド すべてのリモートから受信
        /// </summary>
        /// <param name="port"></param>
        public void Bind(int port, string CIPCServerIP, int CIPCServerPort, int fps, string clientName)
        {
            m_client = new CIPC_CS_Unity.CLIENT.CLIENT(port,CIPCServerIP, CIPCServerPort, clientName, fps);
        }

        public void Connect(MODE mode)
        {
            m_client.Setup(mode);
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
            byte[] data = new byte[0];
            //毎度初期化しないと限定されてしまう．なんという危険物
            m_client.Update(ref data);
            return data;
        }

        /// <summary>
        /// 受信バッファにデータがあるときのみ受信
        /// </summary>
        /// <param name="availe"></param>
        /// <returns></returns>
        public byte[] Receive(ref int availe)
        {
            availe = this.m_client.IsAvailable;
            if (availe > 0)
            {
                return this.Receive();
            }
            return null;
        }

        public void Send(byte[] data)
        {
            this.m_client.Update(ref data);
        }
        

        
    }
}
