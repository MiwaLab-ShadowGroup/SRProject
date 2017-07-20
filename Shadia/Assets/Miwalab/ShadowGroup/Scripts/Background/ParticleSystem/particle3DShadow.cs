using Miwalab.ShadowGroup.Background.ParticleSystem;
using Miwalab.ShadowGroup.Network;
using Miwalab.ShadowGroup.Thread;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class particle3DShadow : MonoBehaviour
{


    public GameObject _particleOriginal;

    public int _num;

    public List<GameObject> _particleObjects;
    public List<particleMover> _particleShadowScripts;

    public Vector3 _scale;
    public Vector3 _acceralate;

    public float[] _HumanVelList_X;
    public float[] _HumanVelList_Y;
    public int PreHumanCount = -2;
    public int HumanCount;

    public readonly int MaxHumanNum = 100;

    public ThreadHost _threadManager;
    AutoResetEvent _AutoResetEventReceiver;
    public object SyncObject_Receiver = new object();

    public NetworkHost _nHost;

    public bool _IsReceive;

    public readonly NetworkSettings.NetworkSetting RECEIVEID = new NetworkSettings.NetworkSetting("particle3D" + Environment.MachineName, NetworkSettings.SETTINGS.ReceiveControlPort);
    public readonly NetworkSettings.NetworkSetting SENDID = new NetworkSettings.NetworkSetting("particle3D_Send" + Environment.MachineName, NetworkSettings.SETTINGS.SendParticlePort);

    private UDP_PACKETS_CODER.UDP_PACKETS_DECODER _dec;
    private UDP_PACKETS_CODER.UDP_PACKETS_ENCODER _enc;
    private bool isInitialzedCIPCClient;
    private string CIPCServerIP;
    private int CIPCServerPort;


    // Use this for initialization
    void Start()
    {
        isInitialzedCIPCClient = false;
        _HumanVelList_X = new float[MaxHumanNum];
        _HumanVelList_Y = new float[MaxHumanNum];
        _particleObjects = new List<GameObject>();
        _particleShadowScripts = new List<particleMover>();
        _AutoResetEventReceiver = new AutoResetEvent(true);
        _threadManager = ThreadHost.GetInstance();
        _threadManager.CreateNewThread(new ContinuouslyThread(this.receiveMethod), RECEIVEID.TAG);
        _threadManager.ThreadStart(RECEIVEID.TAG);
        _num = 1500;
        (ShadowMediaUIHost.GetUI("PRT3D_CIPCServerConnect") as ParameterButton).Clicked += NetworkConnect;
        (ShadowMediaUIHost.GetUI("PRT3D_3DObjectControlReceive") as ParameterCheckbox).ValueChanged += SetIsReceive;
        (ShadowMediaUIHost.GetUI("PRT3D_CIPCServerPort") as ParameterText).m_valueText.text = "12000";

        _acceralate.x = 0.05f;

    }

    private void SetIsReceive(object sender, EventArgs e)
    {
        this._IsReceive = (e as ParameterCheckbox.ChangedValue).Value;
    }

    private void NetworkConnect(object sender, EventArgs e)
    {
        if (isInitialzedCIPCClient) return;
        CIPCServerIP = (ShadowMediaUIHost.GetUI("PRT3D_CIPCServerIP") as ParameterText).m_valueText.text;
        CIPCServerPort = int.Parse((ShadowMediaUIHost.GetUI("PRT3D_CIPCServerPort") as ParameterText).m_valueText.text);
        _nHost = NetworkHost.GetInstance(CIPCServerIP, CIPCServerPort);

        _nHost.AddCIPCClient(RECEIVEID, -1);
        _nHost.ConnectCIPC(RECEIVEID, CIPC_CS_Unity.CLIENT.MODE.Receiver);
        Thread.Sleep(100);
        _nHost.AddCIPCClient(SENDID, 30);
        _nHost.ConnectCIPC(SENDID, CIPC_CS_Unity.CLIENT.MODE.Sender);

        this.isInitialzedCIPCClient = true;
    }

    private void NetworkClose()
    {
        if (!this.isInitialzedCIPCClient) return;
        this._nHost.RemoveCIPCClient(RECEIVEID);
        this._nHost.RemoveCIPCClient(SENDID);
        this.isInitialzedCIPCClient = false;
    }

    private void receiveMethod()
    {
        if (_IsReceive)
        {
            try
            {
                int available = 0;
                byte[] data = _nHost.ReceiveCIPC(RECEIVEID.TAG, ref available);
                if (data != null)
                {
                    this.SetupHumanVelocity(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public void OnApplicationQuit()
    {
        if (_nHost == null) return;
        this._nHost.RemoveCIPCClient(RECEIVEID);
        this._nHost.RemoveCIPCClient(SENDID);

    }

    // Update is called once per frame
    void Update()
    {
        if (_num < 0) _num = 0;
        if (this._particleObjects.Count > _num)
        {
            for (int i = _num; i < this._particleObjects.Count; ++i)
            {
                GameObject.DestroyImmediate(this._particleObjects[i]);
            }
            this._particleObjects.RemoveRange(_num, this._particleObjects.Count - _num);
            this._particleShadowScripts.RemoveRange(_num, this._particleShadowScripts.Count - _num);
        }
        if (this._particleObjects.Count < _num)
        {
            for (int i = 0; i < this._num - this._particleObjects.Count; ++i)
            {
                var _object = Instantiate(_particleOriginal);
                _object.transform.localScale = _scale;
                _object.transform.SetParent(this.transform);
                this._particleObjects.Add(_object);
                this._particleShadowScripts.Add(_object.GetComponent<particleMover>());
            }
        }
        
        if (PreHumanCount != HumanCount)
        {
            PreHumanCount = HumanCount;
            this.ChangeIDs();
        }

        for (int i = 0; i < this._particleShadowScripts.Count; ++i)
        {
            var target = this._particleShadowScripts[i];

            target.AddForce(_acceralate);

            
            //_particleShadowScripts[i]._RHAngleacc = _acceralate;
            //target.AddForce(_HumanVelList_X[target.ID], _HumanVelList_Y[target.ID]);
        }


    }
    private void ChangeIDs()
    {
        
        foreach (var p in this._particleObjects)
        {
            if (HumanCount == 0)
            {
                p.GetComponent<particleMover>().ID = -1;
            }
            p.GetComponent<particleMover>().ID = UnityEngine.Random.Range(0, HumanCount - 1);
        }
    }
    public unsafe void SetupHumanVelocity(byte[] buffer)
    {
        _dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
        _dec.Source = buffer;

        this.HumanCount = _dec.get_int();

        for (int i = 0; i < this.HumanCount; ++i)
        {
            this._HumanVelList_X[i] = _dec.get_float();
            this._HumanVelList_Y[i] = _dec.get_float();
        }
    }

    public unsafe void SendHumanVelocity(int count, float[] datax, float[] datay)
    {
        if (!this.isInitialzedCIPCClient) return;
        _enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();

        _enc += count;

        for (int i = 0; i < count; ++i)
        {
            _enc += datax[i];
            _enc += datay[i];
        }
        byte[] data = _enc.data;
        _nHost.SendCIPC(SENDID, data);
    }
}
