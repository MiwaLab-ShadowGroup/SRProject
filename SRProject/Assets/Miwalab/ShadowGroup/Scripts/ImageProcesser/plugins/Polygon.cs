using UnityEngine;
using System;
using System.Collections.Generic;
using OpenCvSharp.CPlusPlus;

public class Polygon : AImageProcesser {

    int sharpness = 50;
    private Mat grayimage = new Mat();
    List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();


    public override string Name()
    {
        return "Polygon";
    }

    public override void Processing(Mat srcMat, ref Mat dstMat)
    {

        this.List_Contours.Clear();
        Cv2.CvtColor(srcMat, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
        dstMat = new Mat(dstMat.Height, dstMat.Width, MatType.CV_8UC3, Scalar.Black);

        Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
        HierarchyIndex[] hierarchy;

        Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

        List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();

        for (int i = 0; i < contour.Length; i++)
        {
            CvPoints.Clear();
            if (Cv2.ContourArea(contour[i]) > 1000)
            {
                

                for (int j = 0; j < contour[i].Length; j += this.sharpness)
                {

                    CvPoints.Add(contour[i][j]);
                }

                this.List_Contours.Add(CvPoints);

                //Cv2.FillConvexPoly(dst, CvPoints, Scalar.Yellow,  OpenCvSharp.LineType.Link4, 0);
                Cv2.DrawContours(dstMat, this.List_Contours.ToArray(), 0, Scalar.Aqua, -1, OpenCvSharp.LineType.Link8);

            }

        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
