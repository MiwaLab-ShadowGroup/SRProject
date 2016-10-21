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

    #region　send
    Mat _SendMat;
    string _RemoteIP = "127.0.0.1";
    int _RemotePort;
    IPEndPoint _IPEndPoint;
    #endregion

    #region receive
    Mat _ReceivedMat;
    AutoResetEvent _AutoResetEvent;
    #endregion

    bool _IsSend = false;
    bool _IsReceive = false;

    #region ID
    public readonly NetworkSettings.NetworkSetting SENDID = NetworkSettings.SETTINGS.RemoteShadowImageManager_Sender;
    public readonly NetworkSettings.NetworkSetting RECEIVEID = NetworkSettings.SETTINGS.RemoteShadowImageManager_Receiver;
    #endregion
    // Use this for initialization
    void Start()
    {
        _nHost = NetworkHost.GetInstance();
        _tHost = ThreadHost.GetInstance();

        _nHost.AddClient(SENDID, 30);
        _nHost.AddClient(RECEIVEID, -1);

        _AutoResetEvent = new AutoResetEvent(true);

        _tHost.CreateNewThread(new ContinuouslyThread(
            () => this.SendMethod()
            ), SENDID.TAG);

        _tHost.CreateNewThread(new ContinuouslyThread(
            () => this.ReceiveMethod()
            ), RECEIVEID.TAG);


        _IPEndPoint = new IPEndPoint(IPAddress.Parse(_RemoteIP), _RemotePort);
    }

    public void SetIPAddress(string ipAddress)
    {
        _IPEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), _RemotePort);
    }

    private void ReceiveMethod()
    {
        if (_IsReceive)
        {
            try
            {
                int available = 0;
                byte[] data = _nHost.Receive(RECEIVEID, ref available);
                _ReceivedMat = Cv2.ImDecode(data, OpenCvSharp.LoadMode.Color);
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
                _nHost.Send(SENDID, _SendMat.ToBytes(".png"));
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
