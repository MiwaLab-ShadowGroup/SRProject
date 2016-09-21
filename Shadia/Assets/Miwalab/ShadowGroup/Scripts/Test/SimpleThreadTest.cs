using UnityEngine;
using System.Collections;
using Miwalab.ShadowGroup.Thread;
using System;

public class SimpleThreadTest : MonoBehaviour
{
    ThreadHost _tHost;
    int i = 0;
    // Use this for initialization
    void Start()
    {
        _tHost = ThreadHost.GetInstance();

        _tHost.CreateNewThread(new ContinuouslyThread(
            () => this.Counter()
            ), "TestThread");
        _tHost.ThreadStart("TestThread");
    }

    private void Counter()
    {
        i++;
        System.Threading.Thread.Sleep(5);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(i);
    }
}
