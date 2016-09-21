using UnityEngine;
using System.Collections;
using Miwalab.ShadowGroup.Network;
using Miwalab.ShadowGroup.Thread;
using System;
using OpenCvSharp.CPlusPlus;
using System.Net;

public class RemoteShadowImageManager : MonoBehaviour
{
    NetworkHost _nHost;
    ThreadHost _tHost;

    Mat mat;
    string _RemoteIP;
    int _RemotePort;
    IPEndPoint _IPEndPoint;

    bool _IsSend = false;
    bool _IsReceive = false;

    public readonly NetworkSettings.NetworkSetting SENDID= NetworkSettings.SETTINGS.RemoteShadowImageManager_Sender;
    public readonly NetworkSettings.NetworkSetting RECEIVEID= NetworkSettings.SETTINGS.RemoteShadowImageManager_Receiver;
    // Use this for initialization
    void Start()
    {
        _nHost = NetworkHost.GetInstance();
        _tHost = ThreadHost.GetInstance();

        _nHost.AddClient(SENDID);
        _nHost.AddClient(RECEIVEID);

        _tHost.CreateNewThread(new ContinuouslyThread(
            () => this.SendMethod()
            ), SENDID.TAG);

        _tHost.CreateNewThread(new ContinuouslyThread(
            () => this.ReceiveMethod()
            ), SENDID.TAG);


        _IPEndPoint = new IPEndPoint(IPAddress.Parse(_RemoteIP), _RemotePort);
    }

    private void ReceiveMethod()
    {
        if (_IsReceive)
        {
            _nHost.Receive(RECEIVEID);
        }
    }

    public void SendMethod()
    {
        if (_IsSend)
        {
            _nHost.SendTo(SENDID, mat.ToBytes(".png"), _IPEndPoint);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
