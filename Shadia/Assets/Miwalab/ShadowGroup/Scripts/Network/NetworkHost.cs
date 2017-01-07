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
        private Dictionary<string, CIPCClient> m_cipcClientList;
        private List<int> m_portList;

        string CIPCServerIP;
        int CIPCServerPort;

        /// <summary>
        /// sinleton
        /// </summary>
        private NetworkHost(string CIPCServerIP, int CIPCServerPort)
        {
            m_cipcClientList = new Dictionary<string, CIPCClient>();
            m_portList = new List<int>();
            this.CIPCServerIP = CIPCServerIP;
            this.CIPCServerPort = CIPCServerPort;
        }

        /// <summary>
        /// 初回呼び出し時に初期化
        /// それ以降は同一の実態を使用
        /// </summary>
        /// <returns></returns>
        public static NetworkHost GetInstance(string CIPCServerIP, int CIPCServerPort = 12000)
        {
            if (m_actual == null)
            {
                m_actual = new NetworkHost(CIPCServerIP, CIPCServerPort);
            }
            return m_actual;
        }

        public void AddCIPCClient(int port, string tag, int fps)
        {
            if (m_cipcClientList.ContainsKey(tag))
            {
                Debug.Log("tag:" + tag + "は存在します．");
                return;
            }
            if (m_portList.Contains(port))
            {
                ++port;
                //再設定
                AddCIPCClient(port, tag, fps);
            }
            Debug.Log("port番号:" + port + "が初期化されます.");
            try
            {
                CIPCClient client = new CIPCClient(port, CIPCServerIP, CIPCServerPort, fps, tag);
                //タグをつけて記憶
                m_cipcClientList.Add(tag, client);
                m_portList.Add(port);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
            return;
        }

        public void AddCIPCClient(NetworkSettings.NetworkSetting setting, int fps)
        {
            this.AddCIPCClient(setting.PORT, setting.TAG, fps);
        }

        public void ConnectCIPC(NetworkSettings.NetworkSetting setting, CIPC_CS_Unity.CLIENT.MODE mode)
        {
            this.ConnectCIPC(setting.TAG, mode);
        }

        private void ConnectCIPC(string tag, MODE mode)
        {
            if (!this.m_cipcClientList.ContainsKey(tag))
            {
                return;
            }
            this.m_cipcClientList[tag].Connect(mode);
        }

        /// <summary>
        /// Response などで用いる
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="data"></param>
        public void SendCIPC(string tag, byte[] data)
        {
            if (!this.m_cipcClientList.ContainsKey(tag))
            {
                return;
            }
            this.m_cipcClientList[tag].Send(data);
        }

        public void SendCIPC(NetworkSettings.NetworkSetting setting, byte[] data)
        {
            this.SendCIPC(setting.TAG, data);
        }


        public void RemoveCIPCClient(string tag)
        {
            if (!this.m_cipcClientList.ContainsKey(tag))
            {
                return;
            }
            this.m_cipcClientList[tag].Close();
            this.m_cipcClientList.Remove(tag);
        }
        public void RemoveCIPCClient(NetworkSettings.NetworkSetting setting)
        {
            this.RemoveCIPCClient(setting.TAG);
        }


        public CIPCClient GetCIPCClient(string tag)
        {
            if (!this.m_cipcClientList.ContainsKey(tag))
            {
                Debug.Log("No tag as" + tag);
                return null;
            }
            return this.m_cipcClientList[tag];
        }
        public CIPCClient GetCIPCClient(NetworkSettings.NetworkSetting setting)
        {
            return this.GetCIPCClient(setting.TAG);
        }

        public byte[] ReceiveCIPC(string tag)
        {
            if (!this.m_cipcClientList.ContainsKey(tag))
            {
                Debug.Log("No tag as" + tag);
                return null;
            }
            return this.m_cipcClientList[tag].Receive();
        }
        public byte[] ReceiveCIPC(NetworkSettings.NetworkSetting setting)
        {
            return ReceiveCIPC(setting.TAG);
        }

        public byte[] ReceiveCIPC(string tag, ref int available)
        {
            if (!this.m_cipcClientList.ContainsKey(tag))
            {
                Debug.Log("No tag as" + tag);
                return null;
            }
            return this.m_cipcClientList[tag].Receive(ref available);
        }
        public byte[] ReceiveCIPC(NetworkSettings.NetworkSetting setting, ref int available)
        {
            return ReceiveCIPC(setting.TAG, ref available);
        }

        public void ResetCIPC()
        {
            foreach (var p in this.m_cipcClientList)
            {
                p.Value.Close();
            }
            this.m_cipcClientList.Clear();
            this.m_portList.Clear();
        }
    }
}
