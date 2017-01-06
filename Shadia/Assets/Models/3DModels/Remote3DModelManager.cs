
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

public class Remote3DModelManager : MonoBehaviour
{
    NetworkHost _nHost;
    ThreadHost _tHost;

    #region CIPCServerSetting
    [SerializeField]
    public string CIPCServerIP;
    [SerializeField]
    public int CIPCServerPort;

    public object SyncObject_Sender = new object();
    public object SyncObject_Receiver = new object();


    public bool _IsSend = false;
    public bool _IsReceive = false;

    public ShadowMediaUIHost UIHost;

    private bool isInitialzedCIPCClient;
    #endregion

    #region　send
    public Mat _SendMat;
    AutoResetEvent _AutoResetEventSender;
#if USE_REMOTE_BONE
    public byte[] _SendBoneData;
    public UDP_PACKETS_CODER.UDP_PACKETS_ENCODER _Encorder_Bone;
#endif
    #endregion

    #region receive
    public Mat _ReceivedMat;
    AutoResetEvent _AutoResetEventReceiver;
#if USE_REMOTE_BONE
    public UDP_PACKETS_CODER.UDP_PACKETS_DECODER _Decorder_Bone;
    public AImageProcesser.DepthBody[] _ReceivedBodyData;
#endif
    #endregion


    #region ID
    public readonly NetworkSettings.NetworkSetting SENDID = NetworkSettings.SETTINGS.RemoteShadowImageManager_Sender;
    public readonly NetworkSettings.NetworkSetting RECEIVEID = NetworkSettings.SETTINGS.RemoteShadowImageManager_Receiver;
#if USE_REMOTE_BONE
    public readonly NetworkSettings.NetworkSetting SENDIDBONE = NetworkSettings.SETTINGS.RemoteShadowImageManager_Sender_Bone;
    public readonly NetworkSettings.NetworkSetting RECEIVEIDBONE = NetworkSettings.SETTINGS.RemoteShadowImageManager_Receiver_Bone;
#endif
    #endregion
    // Use this for initialization
    void Start()
    {
        isInitialzedCIPCClient = false;
        _tHost = ThreadHost.GetInstance();

        _AutoResetEventReceiver = new AutoResetEvent(true);
        _AutoResetEventSender = new AutoResetEvent(true);

        _tHost.CreateNewThread(new ContinuouslyThread(
            () => this.SendMethod()
            ), SENDID.TAG);
        _tHost.CreateNewThread(new ContinuouslyThread(
            () => this.ReceiveMethod()
            ), RECEIVEID.TAG);

        _tHost.ThreadStart(SENDID.TAG);
        _tHost.ThreadStart(RECEIVEID.TAG);


        (ShadowMediaUIHost.GetUI("Network_CIPCServerConnect") as ParameterButton).Clicked += NetworkConnect;
        (ShadowMediaUIHost.GetUI("Network_Send") as ParameterCheckbox).ValueChanged += SetIsSend;
        (ShadowMediaUIHost.GetUI("Network_Receive") as ParameterCheckbox).ValueChanged += SetIsReceive;

    }

    private void NetworkConnect(object sender, EventArgs e)
    {
        if (isInitialzedCIPCClient) return;
        CIPCServerIP = (ShadowMediaUIHost.GetUI("Network_CIPCServerIP") as ParameterText).m_valueText.text;
        CIPCServerPort = int.Parse((ShadowMediaUIHost.GetUI("Network_CIPCServerPort") as ParameterText).m_valueText.text);
        _nHost = NetworkHost.GetInstance(CIPCServerIP, CIPCServerPort);

        _nHost.AddClient(SENDID, 30);
        _nHost.AddClient(RECEIVEID, -1);
        _nHost.Connect(SENDID, CIPC_CS_Unity.CLIENT.MODE.Sender);
        _nHost.Connect(RECEIVEID, CIPC_CS_Unity.CLIENT.MODE.Receiver);

#if USE_REMOTE_BONE

        _nHost.AddClient(SENDIDBONE, 30);
        _nHost.AddClient(RECEIVEIDBONE, -1);
        _nHost.Connect(SENDIDBONE, CIPC_CS_Unity.CLIENT.MODE.Sender);
        _nHost.Connect(RECEIVEIDBONE, CIPC_CS_Unity.CLIENT.MODE.Receiver);
#endif
        this.isInitialzedCIPCClient = true;
    }

    public void SetSendMat(Mat mat)
    {
        lock (SyncObject_Sender)
        {
            if (this._SendMat == null)
            {
                this._SendMat = mat.Clone();
            }
            else
            {
                mat.CopyTo(this._SendMat);
            }
        }
    }

    public void SetSendSkeletons(ref AImageProcesser.DepthBody[] skeletonData, Body[] capturedBody)
    {
#if USE_REMOTE_BONE
        if (skeletonData == null) return;
        if (capturedBody == null) return;
        lock (SyncObject_Sender)
        {
            _Encorder_Bone = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            byte type;
            float position_x = skeletonData[0].JointDepth[JointType.SpineBase].InitializePosition.x;
            float position_y = skeletonData[0].JointDepth[JointType.SpineBase].InitializePosition.y;
            float position_z = skeletonData[0].JointDepth[JointType.SpineBase].InitializePosition.z;
            byte State;

            _Encorder_Bone += position_x;
            _Encorder_Bone += position_y;
            _Encorder_Bone += position_z;

            _Encorder_Bone += capturedBody.Length;
            for (int i = 0; i < capturedBody.Length; ++i)
            {
                var p = skeletonData[i];
                _Encorder_Bone += p.JointDepth.Count;
                foreach (var q in p.JointDepth)
                {
                    type = (byte)q.Key;
                    position_x = (float)q.Value.localPosition.x;
                    position_y = (float)q.Value.localPosition.y;
                    position_z = (float)q.Value.localPosition.z;
                    State = (byte)q.Value.state;
                    _Encorder_Bone += type;
                    _Encorder_Bone += position_x;
                    _Encorder_Bone += position_y;
                    _Encorder_Bone += position_z;
                    _Encorder_Bone += State;
                }
            }
            this._SendBoneData = _Encorder_Bone.data;
        }
#endif
    }
    public void GetReceivedSkeletons(ref AImageProcesser.DepthBody[] skeletonData, Body[] bodyLength)
    {
#if USE_REMOTE_BONE
        if (this._ReceivedBodyData == null)
        {
            return;
        }
        lock (SyncObject_Receiver)
        {
            if (bodyLength.Length + _ReceivedBodyData.Length != skeletonData.Length)
            {
                Array.Resize(ref skeletonData, bodyLength.Length + _ReceivedBodyData.Length);
            }
            Array.Copy(_ReceivedBodyData, 0, skeletonData, bodyLength.Length, _ReceivedBodyData.Length);
        }
#endif
    }

    public Mat GetReceiveMat()
    {
        lock (SyncObject_Receiver)
        {

            return this._ReceivedMat;

        }
    }

    public void OnApplicationQuit()
    {
        if (_nHost == null) return;
        this._nHost.RemoveClient(SENDID);
        this._nHost.RemoveClient(RECEIVEID);
#if USE_REMOTE_BONE
        this._nHost.RemoveClient(SENDIDBONE);
        this._nHost.RemoveClient(RECEIVEIDBONE);
#endif
    }

    private void ReceiveMethod()
    {
        if (_IsReceive)
        {
            try
            {
                lock (SyncObject_Receiver)
                {
                    int available = 0;
                    byte[] data = _nHost.Receive(RECEIVEID, ref available);
                    if (data != null)
                    {

                        _ReceivedMat = Cv2.ImDecode(data, OpenCvSharp.LoadMode.Color);
                    }

#if USE_REMOTE_BONE

                    byte[] dataBone = _nHost.Receive(RECEIVEIDBONE, ref available);
                    if (dataBone != null)
                    {
                        this._Decorder_Bone = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                        _Decorder_Bone.Source = dataBone;
                        float position_x = _Decorder_Bone.get_float();
                        float position_y = _Decorder_Bone.get_float();
                        float position_z = _Decorder_Bone.get_float();
                        Vector3 initial = new Vector3();
                        Vector3 Buffer = new Vector3();
                        int lengthHuman = _Decorder_Bone.get_int();//1
                        if (this._ReceivedBodyData == null)
                        {
                            this._ReceivedBodyData = new AImageProcesser.DepthBody[lengthHuman];
                        }
                        for (int i = 0; i < lengthHuman; ++i)
                        {
                            int lengthJoints = _Decorder_Bone.get_int();
                            if (this._ReceivedBodyData[i] == null)
                            {
                                this._ReceivedBodyData[i] = new AImageProcesser.DepthBody(lengthJoints, initial);
                            }
                            for (int j = 0; j < lengthJoints; ++j)
                            {
                                JointType type = (JointType)_Decorder_Bone.get_byte();
                                Buffer.x = (float)_Decorder_Bone.get_float();
                                Buffer.y = (float)_Decorder_Bone.get_float();
                                Buffer.z = (float)_Decorder_Bone.get_float();
                                TrackingState State = (TrackingState)_Decorder_Bone.get_byte();

                                this._ReceivedBodyData[i].JointDepth[type].update(Buffer, State);
                            }
                        }

                    }
#endif


                }
                _AutoResetEventReceiver.WaitOne();
            }
            catch
            {

            }
        }
    }

    public void SendMethod()
    {
        if (_IsSend)
        {
            try
            {
                lock (SyncObject_Sender)
                {
                    if (_SendMat != null)
                    {
                        _nHost.Send(SENDID, _SendMat.ToBytes(".png"));
                    }
#if USE_REMOTE_BONE
                    if (_SendBoneData != null)
                    {
                        _nHost.Send(SENDIDBONE, _SendBoneData);
                    }
#endif
                }
                _AutoResetEventSender.WaitOne();
            }
            catch
            {

            }
        }
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
        _AutoResetEventSender.Set();
        _AutoResetEventReceiver.Set();
    }
}