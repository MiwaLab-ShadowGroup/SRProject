using UnityEngine;
using System.Collections;
using Miwalab.ShadowGroup.Network;
using Miwalab.ShadowGroup.Thread;

public class RemoteShadowImageManager : MonoBehaviour
{
    NetworkHost _nHost;
    ThreadHost _tHost;

    int _myPort;
    int _remotePort;

    // Use this for initialization
    void Start()
    {
        _nHost = NetworkHost.GetInstance();
        _tHost = ThreadHost.GetInstance();
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
