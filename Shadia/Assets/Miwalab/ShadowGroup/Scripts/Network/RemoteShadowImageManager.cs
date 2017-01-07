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

        public enum TargetIndex
        {
            first,
            second
        }

        private SendMode _sendMode;
        private ReceiveMode _receiveMode;


        ThreadHost _tHost;

        TargetIndex _sendTargetIndex1 = TargetIndex.first;
        TargetIndex _sendTargetIndex2 = TargetIndex.second;
        IPEndPoint _sendTarget1;
        IPEndPoint _sendTarget2;
        IPEndPoint _sendTargetMesh1;
        IPEndPoint _sendTargetMesh2;

        //一応二つずつ
        private UDPClient _sendClient1;
        private UDPClient _sendClient2;
        private UDPClient _receiveClient1;
        private UDPClient _receiveClient2;

        private UDPClient _sendMeshClient1;
        private UDPClient _sendMeshClient2;
        private UDPClient _receiveMeshClient1;
        private UDPClient _receiveMeshClient2;


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

            _receiveMeshClient1 = new UDPClient(NetworkSettings.SETTINGS.ReceiveMeshClient1Port);
            _receiveMeshClient2 = new UDPClient(NetworkSettings.SETTINGS.ReceiveMeshClient2Port);

            _sendMeshClient1 = new UDPClient(NetworkSettings.SETTINGS.SendMeshClient1Port);
            _sendMeshClient2 = new UDPClient(NetworkSettings.SETTINGS.SendMeshClient2Port);

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

            (ShadowMediaUIHost.GetUI("RSIM_SenderMode") as ParameterDropdown).ValueChanged += SenderMode_ValueChanged;
            (ShadowMediaUIHost.GetUI("RSIM_ReceiverMode") as ParameterDropdown).ValueChanged += ReceiveMode_ValueChanged;

            (ShadowMediaUIHost.GetUI("RSIM_Sender1_TargetIndex") as ParameterDropdown).ValueChanged += TargetIndex1_ValueChanged;

            (ShadowMediaUIHost.GetUI("RSIM_Sender2_TargetIndex") as ParameterDropdown).ValueChanged += TargetIndex2_ValueChanged;
            (ShadowMediaUIHost.GetUI("RSIM_SettingUpdate") as ParameterButton).Clicked += SettingUpdate_Clicked;


            (ShadowMediaUIHost.GetUI("RSIM_1_x") as ParameterSlider).ValueChanged += posX_1_ValueChanged;
            (ShadowMediaUIHost.GetUI("RSIM_1_y") as ParameterSlider).ValueChanged += posY_1_ValueChanged;
            (ShadowMediaUIHost.GetUI("RSIM_1_z") as ParameterSlider).ValueChanged += posZ_1_ValueChanged;
            (ShadowMediaUIHost.GetUI("RSIM_1_rot") as ParameterSlider).ValueChanged += rot_1_ValueChanged;

            (ShadowMediaUIHost.GetUI("RSIM_2_x") as ParameterSlider).ValueChanged += posX_2_ValueChanged;
            (ShadowMediaUIHost.GetUI("RSIM_2_y") as ParameterSlider).ValueChanged += posY_2_ValueChanged;
            (ShadowMediaUIHost.GetUI("RSIM_2_z") as ParameterSlider).ValueChanged += posZ_2_ValueChanged;
            (ShadowMediaUIHost.GetUI("RSIM_2_rot") as ParameterSlider).ValueChanged += rot_2_ValueChanged;


        }


        private void rot_2_ValueChanged(object sender, EventArgs e)
        {
            var value = (e as ParameterSlider.ChangedValue).Value;
            _networkPlane2.transform.rotation = Quaternion.Euler(0, value, 0);
        }

        private void posZ_2_ValueChanged(object sender, EventArgs e)
        {
            var pos = _networkPlane2.transform.position;
            var value = (e as ParameterSlider.ChangedValue).Value;
            _networkPlane2.transform.position = new Vector3(pos.x, pos.y, value);
        }

        private void posY_2_ValueChanged(object sender, EventArgs e)
        {
            var pos = _networkPlane2.transform.position;
            var value = (e as ParameterSlider.ChangedValue).Value;
            _networkPlane2.transform.position = new Vector3(pos.x, value, pos.z);
        }

        private void posX_2_ValueChanged(object sender, EventArgs e)
        {
            var pos = _networkPlane2.transform.position;
            var value = (e as ParameterSlider.ChangedValue).Value;
            _networkPlane2.transform.position = new Vector3(value, pos.y, pos.z);
        }

        private void rot_1_ValueChanged(object sender, EventArgs e)
        {
            var value = (e as ParameterSlider.ChangedValue).Value;
            _networkPlane1.transform.rotation = Quaternion.Euler(0, value, 0);
        }

        private void posZ_1_ValueChanged(object sender, EventArgs e)
        {
            var pos = _networkPlane1.transform.position;
            var value = (e as ParameterSlider.ChangedValue).Value;
            _networkPlane1.transform.position = new Vector3(pos.x, pos.y, value);
        }

        private void posY_1_ValueChanged(object sender, EventArgs e)
        {
            var pos = _networkPlane1.transform.position;
            var value = (e as ParameterSlider.ChangedValue).Value;
            _networkPlane1.transform.position = new Vector3(pos.x, value, pos.z);
        }

        private void posX_1_ValueChanged(object sender, EventArgs e)
        {
            var pos = _networkPlane1.transform.position;
            var value = (e as ParameterSlider.ChangedValue).Value;
            _networkPlane1.transform.position = new Vector3(value,pos.y,pos.z);
        }

        private void SettingUpdate_Clicked(object sender, EventArgs e)
        {
            int targetPort1 = _sendTargetIndex1 == TargetIndex.first ? Network.NetworkSettings.SETTINGS.ReceiveClient1Port
                : Network.NetworkSettings.SETTINGS.ReceiveClient2Port;
            int targetPort2 = _sendTargetIndex2 == TargetIndex.first ? Network.NetworkSettings.SETTINGS.ReceiveClient1Port
                : Network.NetworkSettings.SETTINGS.ReceiveClient2Port;

            int targetMeshPort1 = _sendTargetIndex1 == TargetIndex.first ? Network.NetworkSettings.SETTINGS.ReceiveMeshClient1Port
                : Network.NetworkSettings.SETTINGS.ReceiveMeshClient2Port;
            int targetMeshPort2 = _sendTargetIndex2 == TargetIndex.first ? Network.NetworkSettings.SETTINGS.ReceiveMeshClient1Port
                : Network.NetworkSettings.SETTINGS.ReceiveMeshClient2Port;


            _sendTarget1 = new IPEndPoint(IPAddress.Parse((ShadowMediaUIHost.GetUI("RSIM_Sender1_TargetIP") as ParameterText).m_valueText.text), targetPort1);
            _sendTarget2 = new IPEndPoint(IPAddress.Parse((ShadowMediaUIHost.GetUI("RSIM_Sender2_TargetIP") as ParameterText).m_valueText.text), targetPort2);
            _sendTargetMesh1 = new IPEndPoint(IPAddress.Parse((ShadowMediaUIHost.GetUI("RSIM_Sender1_TargetIP") as ParameterText).m_valueText.text), targetMeshPort1);
            _sendTargetMesh2 = new IPEndPoint(IPAddress.Parse((ShadowMediaUIHost.GetUI("RSIM_Sender2_TargetIP") as ParameterText).m_valueText.text), targetMeshPort2);

        }

        private void TargetIndex2_ValueChanged(object sender, EventArgs e)
        {
            this._sendTargetIndex2 = (TargetIndex)(e as ParameterDropdown.ChangedValue).Value;
        }

        private void TargetIndex1_ValueChanged(object sender, EventArgs e)
        {
            this._sendTargetIndex1 = (TargetIndex)(e as ParameterDropdown.ChangedValue).Value;
        }

        private void SenderMode_ValueChanged(object sender, EventArgs e)
        {
            this._sendMode = (SendMode)(e as ParameterDropdown.ChangedValue).Value;
        }
        private void ReceiveMode_ValueChanged(object sender, EventArgs e)
        {
            this._receiveMode = (ReceiveMode)(e as ParameterDropdown.ChangedValue).Value;
        }

        public void SendAllClient(byte[] data)
        {
            if (!_IsSend) return;
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
        public void SendAllClientMesh(Vector3[] vertices, Vector2[] uvs)
        {
            if (!_IsSend) return;
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            for(int i = 0; i < vertices.Length; ++i)
            {
                float x,y,z,u,v;
                x = vertices[i].x;
                y = vertices[i].y;
                z = vertices[i].z;
                u = uvs[i].x;
                v = uvs[i].y;
                enc += x;
                enc += y;
                enc += z;

                enc += u;
                enc += v;
            }
            byte[] data = enc.data;

            //とりあえずメインスレッド
            switch (this._sendMode)
            {
                case SendMode.To1Client:
                    if (_sendTargetMesh1 != null)
                    {
                        _sendMeshClient1.Send(data, _sendTargetMesh1);
                    }
                    break;
                case SendMode.To2Client:
                    if (_sendTargetMesh1 != null && _sendTargetMesh2 != null)
                    {
                        _sendMeshClient1.Send(data, _sendTargetMesh1);
                        _sendMeshClient2.Send(data, _sendTargetMesh2);
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
                    int available = 0;
                    byte[] data = _receiveClient2.Receive(ref available);
                    if (data != null)
                    {
                        _networkPlane2.SetupTexture(data);
                    }

                    byte[] meshData = _receiveMeshClient2.Receive(ref available);
                    if (data != null)
                    {
                        _networkPlane2.SetupMesh(meshData);
                    }
                }
                catch
                {
                    Console.WriteLine("nanikamazui");
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
                    int available = 0;
                    byte[] data = _receiveClient1.Receive(ref available);
                    if (data != null)
                    {
                        _networkPlane1.SetupTexture(data);
                    }
                    byte[] meshData = _receiveMeshClient1.Receive(ref available);
                    if (data != null)
                    {
                        _networkPlane1.SetupMesh(meshData);
                    }
                }
                catch
                {
                    Console.WriteLine("nanikamazui");
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