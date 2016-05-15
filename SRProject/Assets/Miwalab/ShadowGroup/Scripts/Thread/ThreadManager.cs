using Miwalab.ShadowGroup.Thread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    ThreadHost m_threadHost;
    void Start()
    {
        m_threadHost = ThreadHost.GetInstance();
    }
    void Update()
    {
        m_threadHost.Update();
    }

    void OnApplicationQuit()
    {
        m_threadHost.AllThreadAbort();
    }
}

