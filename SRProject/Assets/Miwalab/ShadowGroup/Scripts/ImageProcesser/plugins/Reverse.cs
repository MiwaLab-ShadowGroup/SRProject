using UnityEngine;
using System.Collections;
using System;
using OpenCvSharp.CPlusPlus;

public class Reverse : AImageProcesser{

    

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override string Name()
    {
        return "Reverse";
    }

    public override void Processing(Mat srcMat, ref Mat dstMat)
    {
        dstMat = ~srcMat;
    }

}
