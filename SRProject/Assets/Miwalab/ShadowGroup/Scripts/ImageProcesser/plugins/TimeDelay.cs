using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using OpenCvSharp.CPlusPlus;

public class TimeDelay : AImageProcesser {

    [Range(0, 1000)]
    public int delayFrame;

    int frameCount;
    Queue<Mat> queue;


    public TimeDelay():base()
    {
        this.frameCount = 0;
        this.queue = new Queue<Mat>();
    }

    public override void Processing(Mat srcMat, ref Mat dstMat)
    {
        throw new NotImplementedException();
        Mat item = new Mat();
        srcMat.CopyTo(item);


        this.queue.Enqueue(item);
        this.frameCount++;

        if (this.frameCount > delayFrame) //この値で遅れ時間を調整(UIで変えられる)
        {
            this.queue.Dequeue().CopyTo(dstMat);

        }
    }

    public override string Name()
    {
        throw new NotImplementedException();
        return "TimeDelay";
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
