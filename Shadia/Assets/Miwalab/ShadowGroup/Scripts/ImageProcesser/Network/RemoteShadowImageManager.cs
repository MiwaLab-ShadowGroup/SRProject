using UnityEngine;
using System.Collections;
using Miwalab.ShadowGroup.Network;
using Miwalab.ShadowGroup.Thread;
using System;
using OpenCvSharp.CPlusPlus;
using System.Net;
using System.Threading;

public class RemoteShadowImageManager : MonoBehaviour
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

    IPEndPoint _IPEndPoint;
    #endregion

    #region receive
    public Mat _ReceivedMat;
    AutoResetEvent _AutoResetEventReceiver;
    AutoResetEvent _AutoResetEventSender;
    #endregion


    #region ID
    public readonly NetworkSettings.NetworkSetting SENDID = NetworkSettings.SETTINGS.RemoteShadowImageManager_Sender;
    public readonly NetworkSettings.NetworkSetting RECEIVEID = NetworkSettings.SETTINGS.RemoteShadowImageManager_Receiver;
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

    public Mat GetReceiveMat()
    {
        lock (SyncObject_Receiver)
        {

            return this._ReceivedMat;

        }
    }

    public void OnApplicationQuit()
    {
        this._nHost.RemoveClient(SENDID);
        this._nHost.RemoveClient(RECEIVEID);
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
