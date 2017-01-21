
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

    public object SyncObject_Receiver = new object();


    public bool _IsReceive = false;

    public ShadowMediaUIHost UIHost;

    private bool isInitialzedCIPCClient;
    #endregion


    #region receive
    public NetworkOperation networkOpe { set; get; }
    AutoResetEvent _AutoResetEventReceiver;
    #endregion


    #region ID
    public readonly NetworkSettings.NetworkSetting RECEIVEID = new NetworkSettings.NetworkSetting("Conrtol_rcv"+Environment.MachineName, NetworkSettings.SETTINGS.ReceiveControlPort);
    #endregion
    // Use this for initialization
    void Start()
    {
        isInitialzedCIPCClient = false;
        _tHost = ThreadHost.GetInstance();

        _AutoResetEventReceiver = new AutoResetEvent(true);

        _tHost.CreateNewThread(new ContinuouslyThread(
            () => this.ReceiveMethod()
            ), RECEIVEID.TAG);

        _tHost.ThreadStart(RECEIVEID.TAG);

        networkOpe = new NetworkOperation();

        (ShadowMediaUIHost.GetUI("R3DMM_CIPCServerConnect") as ParameterButton).Clicked += NetworkConnect;
        (ShadowMediaUIHost.GetUI("R3DMM_3DObjectControlReceive") as ParameterCheckbox).ValueChanged += SetIsReceive;

    }

    private void NetworkConnect(object sender, EventArgs e)
    {
        if (isInitialzedCIPCClient) return;
        CIPCServerIP = (ShadowMediaUIHost.GetUI("R3DMM_CIPCServerIP") as ParameterText).m_valueText.text;
        CIPCServerPort = int.Parse((ShadowMediaUIHost.GetUI("R3DMM_CIPCServerPort") as ParameterText).m_valueText.text);
        _nHost = NetworkHost.GetInstance(CIPCServerIP, CIPCServerPort);

        _nHost.AddCIPCClient(RECEIVEID, -1);
        _nHost.ConnectCIPC(RECEIVEID, CIPC_CS_Unity.CLIENT.MODE.Receiver);
        
        this.isInitialzedCIPCClient = true;
    }
    
    
    public Object3DOperation GetReceiveed3DOpe()
    {
        lock (SyncObject_Receiver)
        {

            
            return this.networkOpe.getObject3DOpe(0);

        }
    }

    public void OnApplicationQuit()
    {
        if (_nHost == null) return;
        this._nHost.RemoveCIPCClient(RECEIVEID);

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
                    byte[] data = _nHost.ReceiveCIPC(RECEIVEID, ref available);
                    if (data != null)
                    {
                        networkOpe.SetFromBinary(data);

                    }
                    
                }
                _AutoResetEventReceiver.WaitOne();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public void SetIsReceive(object sender, EventArgs e)
    {
        _IsReceive = (e as ParameterCheckbox.ChangedValue).Value;
    }

    // Update is called once per frame
    void Update()
    {
        _AutoResetEventReceiver.Set();
    }
}