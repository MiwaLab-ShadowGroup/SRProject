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

    // Use this for initialization
    void Start () {
        
        

	}
	
	// Update is called once per frame
	void Update () {

        if (nHost != null)
        {
            
            senddata = nOpe.GetBinary();

            nHost.SendCIPC(setting.TAG,senddata);

        }

    }


    public void Connect()
    {

        ServerIP = _serverIP.text;
        Serverport = int.Parse(_serverPort.text);

        setting.TAG = Environment.MachineName + "ShadiaNOP";
        setting.PORT = Serverport;

        nHost = NetworkHost.GetInstance(ServerIP);

        nHost.AddCIPCClient(setting, 30);

        nHost.ConnectCIPC(setting, CIPC_CS_Unity.CLIENT.MODE.Sender);


    }

    public void CloseCIPC()
    {
        nHost.RemoveCIPCClient(setting.TAG);

    }

}
