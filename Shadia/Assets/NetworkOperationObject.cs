using Miwalab.ShadowGroup.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkOperationObject : MonoBehaviour {

    public NetworkHost nHost;

    int Serverport;

    string ServerIP;
    public Text _serverIP;
    public Text _serverPort;


    NetworkOperation nOpe;

    NetworkSettings.NetworkSetting setting;
    byte[] senddata;

    bool isInitialized;

    // Use this for initialization
    void Start () {
        nOpe = new NetworkOperation();
        nOpe.AddObject3DOpe(new Object3DOperation(0,0));
        

	}
	
	// Update is called once per frame
	void Update () {

        if (isInitialized)
        {
            float V =  Input.GetAxis("Vertical");
            float H = Input.GetAxis("Horizontal");
            nOpe.clear();
            nOpe.AddObject3DOpe(new Object3DOperation(H, V));
            senddata = nOpe.GetBinary();

            nHost.SendCIPC(setting.TAG,senddata);
            Debug.Log(V);
            Debug.Log(H);
        }

    }


    public void Connect()
    {

        ServerIP = _serverIP.text;

        setting.TAG = Environment.MachineName + "ShadiaNOP";
        setting.PORT = Miwalab.ShadowGroup.Network.NetworkSettings.SETTINGS.SendControlPort;

        nHost = NetworkHost.GetInstance(ServerIP);

        nHost.AddCIPCClient(setting, 30);

        nHost.ConnectCIPC(setting, CIPC_CS_Unity.CLIENT.MODE.Sender);

        isInitialized = true;

    }

    public void CloseCIPC()
    {
        nHost.RemoveCIPCClient(setting.TAG);

    }

}
