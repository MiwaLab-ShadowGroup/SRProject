//#define USE_REMOTE_BONE

using UnityEngine;
using System.Collections;
using Miwalab.ShadowGroup.Network;
using Miwalab.ShadowGroup.Thread;
using System;
using OpenCvSharp.CPlusPlus;
using System.Net;
using System.Threading;
using Windows.Kinect;
using Miwalab.ShadowGroup.ImageProcesser;
using System.Collections.Generic;
using Miwalab.ShadowGroup.Callibration.Network;
namespace Miwalab.ShadowGroup.Network
{
    public class RemoteShadowImageManager : MonoBehaviour
    {
        public enum SendMode
        {
            To1Client,
            To2Client,
            Nothing
        }

        public enum ReceiveMode
        {
            From1Client,
            From2Client,
            Nothing
        }


        private SendMode _sendMode;
        private ReceiveMode _receiveMode;


        ThreadHost _tHost;

        IPEndPoint _sendTarget1;
        IPEndPoint _sendTarget2;

        //一応二つずつ
        private UDPClient _sendClient1;
        private UDPClient _sendClient2;
        private UDPClient _receiveClient1;
        private UDPClient _receiveClient2;
        public object SyncObject_Receiver1 = new object();
        public object SyncObject_Receiver2 = new object();
        public bool _IsSend = false;
        public bool _IsReceive = false;

        //これは共通
        public Mat _SendMat;

        public readonly string _receiveTag1 = "receiveThread1";
        public readonly string _receiveTag2 = "receiveThread2";

        AutoResetEvent _AutoResetEventReceiver1;
        AutoResetEvent _AutoResetEventReceiver2;

        public NetworkPlane _networkPlane1;
        public NetworkPlane _networkPlane2;

        public MatAttacher _matAttacher;
        public Shader _doubleSideShader;

        // Use this for initialization
        void Start()
        {
            _sendMode = SendMode.Nothing;
            _receiveMode = ReceiveMode.Nothing;

            _sendClient1 = new UDPClient(Network.NetworkSettings.SETTINGS.SendClient1Port);
            _sendClient2 = new UDPClient(Network.NetworkSettings.SETTINGS.SendClient2Port);
            _receiveClient1 = new UDPClient(Network.NetworkSettings.SETTINGS.ReceiveClient1Port);
            _receiveClient2 = new UDPClient(Network.NetworkSettings.SETTINGS.ReceiveClient2Port);

            _networkPlane1.Initialzie(_matAttacher, _doubleSideShader);
            _networkPlane2.Initialzie(_matAttacher, _doubleSideShader);

            _tHost = ThreadHost.GetInstance();
            _AutoResetEventReceiver1 = new AutoResetEvent(true);
            _AutoResetEventReceiver2 = new AutoResetEvent(true);

            _tHost.CreateNewThread(new ContinuouslyThread(
                () => this.ReceiveMethod1()
                ), _receiveTag1);
            _tHost.CreateNewThread(new ContinuouslyThread(
                () => this.ReceiveMethod2()
                ), _receiveTag2);

            _tHost.ThreadStart(_receiveTag1);
            _tHost.ThreadStart(_receiveTag2);

            (ShadowMediaUIHost.GetUI("Network_Send") as ParameterCheckbox).ValueChanged += SetIsSend;
            (ShadowMediaUIHost.GetUI("Network_Receive") as ParameterCheckbox).ValueChanged += SetIsReceive;

        }

        public void SendAllClient(byte[] data)
        {
            //とりあえずメインスレッド
            switch (this._sendMode)
            {
                case SendMode.To1Client:
                    if (_sendTarget1 != null)
                    {
                        _sendClient1.Send(data, _sendTarget1);
                    }
                    break;
                case SendMode.To2Client:
                    if (_sendTarget1 != null && _sendTarget2 != null)
                    {
                        _sendClient1.Send(data, _sendTarget1);
                        _sendClient2.Send(data, _sendTarget2);
                    }
                    break;
                default:
                    break;
            }
        }

        public void OnApplicationQuit()
        {


        }

        private void ReceiveMethod2()
        {
            if (_IsReceive && this._receiveMode == ReceiveMode.From2Client)
            {
                try
                {
                    lock (SyncObject_Receiver2)
                    {
                        int available = 0;
                        byte[] data = _receiveClient2.Receive(ref available);
                        if (data != null)
                        {
                            _networkPlane2.SetupTexture(data);
                        }
                    }
                }
                catch
                {

                }
            }
            _AutoResetEventReceiver2.WaitOne();
        }

        private void ReceiveMethod1()
        {
            if (_IsReceive && (this._receiveMode == ReceiveMode.From1Client || this._receiveMode == ReceiveMode.From2Client))
            {
                try
                {
                    lock (SyncObject_Receiver1)
                    {
                        int available = 0;
                        byte[] data = _receiveClient1.Receive(ref available);
                        if (data != null)
                        {
                            _networkPlane1.SetupTexture(data);
                        }
                    }
                }
                catch
                {

                }
            }
            _AutoResetEventReceiver1.WaitOne();
        }

        public void SetIsSend(object sender, EventArgs e)
        {
            _IsSend = (e as ParameterCheckbox.ChangedValue).Value;
        }

        public void SetIsReceive(object sender, EventArgs e)
        {
            _IsReceive = (e as ParameterCheckbox.ChangedValue).Value;
        }

        // Update is called once per frame
        void Update()
        {
            _AutoResetEventReceiver1.Set();
            _AutoResetEventReceiver2.Set();
        }
    }
}