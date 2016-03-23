using UnityEngine;
using System.Collections;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using System;

public class CameraImporter : ASensorImporter
{

    public Mat mat;
    VideoCapture video;
    Window check;
    public bool isShowImage;

    // Use this for initialization
    void Start()
    {
        video = VideoCapture.FromCamera(1);
        if (!video.IsOpened())
            throw new System.Exception("capture initialization failed");

        mat = new Mat();
        check = new Window("Check");
    }

    // Update is called once per frame
    void Update()
    {
        video.Read(mat);
        if (mat.Empty())
            return;
        if (this.isShowImage)
        {
            check.ShowImage(mat);
        }
    }

    public override Mat getCvMat()
    {
        return this.mat;
    }

    void OnApplicationQuit()
    {
        Window.DestroyAllWindows();
    }
}
