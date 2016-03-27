using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using OpenCvSharp.CPlusPlus;

public class Vector : AImageProcesser {

    List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
    private Mat grayimage = new Mat();

    public override string Name()
    {
        return "Vector";
    }

    public override void Processing(Mat srcMat, ref Mat dstMat)
    {
        //輪郭検出
        this.List_Contours.Clear();
        Cv2.CvtColor(srcMat, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
        dstMat = new Mat(dstMat.Height, dstMat.Width, MatType.CV_8UC3, Scalar.Black);

        Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
        HierarchyIndex[] hierarchy;

        Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

        List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();

        //ベクター描画
        foreach(var p in contour)
        {
            CvPoints.Clear();

            
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
