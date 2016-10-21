using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using CIPC_CS_Unity.CLIENT;

namespace Miwalab.ShadowGroup.Network
{
    /// <summary>
    /// Singleton
    /// </summary>
    public class NetworkHost
    {
        /// <summary>
        /// singleton
        /// </summary>
        private static NetworkHost m_actual;


        /// <summary>
        /// クライアントのリスト
        /// </summary>
        private Dictionary<string, CIPCClient> m_clientList;
        private List<int> m_portList;

        string CIPCServerIP;
        int CIPCServerPort;

        /// <summary>
        /// sinleton
        /// </summary>
        private NetworkHost(string CIPCServerIP, int CIPCServerPort)
        {
            m_clientList = new Dictionary<string, CIPCClient>();
            m_portList = new List<int>();
            this.CIPCServerIP = CIPCServerIP;
            this.CIPCServerPort = CIPCServerPort;
        }

        /// <summary>
        /// 初回呼び出し時に初期化
        /// それ以降は同一の実態を使用
        /// </summary>
        /// <returns></returns>
        public static NetworkHost GetInstance(string CIPCServerIP, int CIPCServerPort)
        {
            if (m_actual == null)
            {
                m_actual = new NetworkHost(CIPCServerIP, CIPCServerPort);
            }
            return m_actual;
        }

        public void AddClient(int port, string tag, int fps)
        {
            if (m_clientList.ContainsKey(tag))
            {
                Debug.Log("tag:" + tag + "は存在します．");
                return;
            }
            if (m_portList.Contains(port))
            {
                ++port;
                //再設定
                AddClient(port, tag, fps);
            }
            Debug.Log("port番号:" + port + "が初期化されます.");
            try
            {
                CIPCClient client = new CIPCClient(port, CIPCServerIP, CIPCServerPort, fps, tag + port.ToString());
                //タグをつけて記憶
                m_clientList.Add(tag, client);
                m_portList.Add(port);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
            return;
        }

        public void AddClient(NetworkSettings.NetworkSetting setting, int fps)
        {
            this.AddClient(setting.PORT, setting.TAG, fps);
        }

        public void Connect(NetworkSettings.NetworkSetting setting, CIPC_CS_Unity.CLIENT.MODE mode)
        {
            this.Connect(setting.TAG, mode);
        }

        private void Connect(string tag, MODE mode)
        {
            if (!this.m_clientList.ContainsKey(tag))
            {
                return;
            }
            this.m_clientList[tag].Connect(mode);
        }

        /// <summary>
        /// Response などで用いる
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="data"></param>
        public void Send(string tag, byte[] data)
        {
            if (!this.m_clientList.ContainsKey(tag))
            {
                return;
            }
            this.m_clientList[tag].Send(data);
        }

        public void Send(NetworkSettings.NetworkSetting setting, byte[] data)
        {
            this.Send(setting.TAG, data);
        }


        public void RemoveClient(string tag)
        {
            if (!this.m_clientList.ContainsKey(tag))
            {
                return;
            }
            this.m_clientList[tag].Close();
            this.m_clientList.Remove(tag);
        }
        public void RemoveClient(NetworkSettings.NetworkSetting setting)
        {
            this.RemoveClient(setting.TAG);
        }


        public CIPCClient GetClient(string tag)
        {
            if (!this.m_clientList.ContainsKey(tag))
            {
                Debug.Log("No tag as" + tag);
                return null;
            }
            return this.m_clientList[tag];
        }
        public CIPCClient GetClient(NetworkSettings.NetworkSetting setting)
        {
            return this.GetClient(setting.TAG);
        }

        public byte[] Receive(string tag)
        {
            if (!this.m_clientList.ContainsKey(tag))
            {
                Debug.Log("No tag as" + tag);
                return null;
            }
            return this.m_clientList[tag].Receive();
        }
        public byte[] Receive(NetworkSettings.NetworkSetting setting)
        {
            return Receive(setting.TAG);
        }

        public byte[] Receive(string tag, ref int available)
        {
            if (!this.m_clientList.ContainsKey(tag))
            {
                Debug.Log("No tag as" + tag);
                return null;
            }
            return this.m_clientList[tag].Receive(ref available);
        }
        public byte[] Receive(NetworkSettings.NetworkSetting setting, ref int available)
        {
            return Receive(setting.TAG, ref available);
        }

        public void Reset()
        {
            foreach (var p in this.m_clientList)
            {
                p.Value.Close();
            }
            this.m_clientList.Clear();
            this.m_portList.Clear();
        }
    }
}
