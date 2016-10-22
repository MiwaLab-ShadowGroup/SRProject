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

    public bool _IsSend = false;
    public bool _IsReceive = false;

    #endregion

    #region　send
    public Mat _SendMat;
    
    IPEndPoint _IPEndPoint;
    #endregion

    #region receive
    public Mat _ReceivedMat;
    AutoResetEvent _AutoResetEvent;
    #endregion


    #region ID
    public readonly NetworkSettings.NetworkSetting SENDID = NetworkSettings.SETTINGS.RemoteShadowImageManager_Sender;
    public readonly NetworkSettings.NetworkSetting RECEIVEID = NetworkSettings.SETTINGS.RemoteShadowImageManager_Receiver;
    #endregion
    // Use this for initialization
    void Start()
    {
        _nHost = NetworkHost.GetInstance(CIPCServerIP, CIPCServerPort);
        _tHost = ThreadHost.GetInstance();

        _nHost.AddClient(SENDID, 30);
        _nHost.AddClient(RECEIVEID, -1);

        _nHost.Connect(SENDID, CIPC_CS_Unity.CLIENT.MODE.Sender);
        _nHost.Connect(RECEIVEID, CIPC_CS_Unity.CLIENT.MODE.Receiver);

        _AutoResetEvent = new AutoResetEvent(true);

        _tHost.CreateNewThread(new ContinuouslyThread(
            () => this.SendMethod()
            ), SENDID.TAG);
        _tHost.CreateNewThread(new ContinuouslyThread(
            () => this.ReceiveMethod()
            ), RECEIVEID.TAG);

        _tHost.ThreadStart(SENDID.TAG);
        _tHost.ThreadStart(RECEIVEID.TAG);
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
                int available = 0;
                byte[] data = _nHost.Receive(RECEIVEID, ref available);
                if (data != null)
                {

                    _ReceivedMat = Cv2.ImDecode(data, OpenCvSharp.LoadMode.Color);
                }
                _AutoResetEvent.WaitOne();
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
                if (_SendMat != null)
                {
                    _nHost.Send(SENDID, _SendMat.ToBytes(".png"));
                }
            }
            catch
            {

            }
        }
    }

    public void SetIsSend(bool value)
    {
        _IsSend = value;
    }

    public void SetIsReceive(bool value)
    {
        _IsReceive = value;
    }

    // Update is called once per frame
    void Update()
    {

        
        _AutoResetEvent.Set();
    }
}
