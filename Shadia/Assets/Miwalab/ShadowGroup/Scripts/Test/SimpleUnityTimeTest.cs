using UnityEngine;
using System.Collections;
using Miwalab.ShadowGroup.Thread;




public class SimpleUnityTimeTest : MonoBehaviour {
    ThreadHost _tHost;
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    long counter = 0;

	// Use this for initialization
	void Start () {

        sw.Start();
        _tHost = ThreadHost.GetInstance();
        _tHost.CreateNewThread(new ContinuouslyThread(() => Log()), "Test");
        _tHost.ThreadStart("Test");
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(counter);
	}

    void Log()
    {
        long t = sw.ElapsedMilliseconds;
        ++counter;
    }
}
