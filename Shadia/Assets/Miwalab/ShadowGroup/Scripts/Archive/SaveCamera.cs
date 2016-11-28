using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;


//[RequireComponent(typeof(ColorImporter))]
public class SaveCamera : MonoBehaviour
{

    VideoWriter vw;

    ColorImporter Camera;

    FPSAdjuster.FPSAdjuster FpsAd;

    int codec = 0;
    string filename;
    Thread thread;
    string filePath;
    Size size;

    bool IsSaveStop =false;

    //Mat resizedMat;

    void Start()
    {
        Camera = gameObject.GetComponent<ColorImporter>();

        (ShadowMediaUIHost.GetUI("ChooseCameraSaveFile") as ParameterButton).Clicked += ChooseFolder_Clicked;

        (ShadowMediaUIHost.GetUI("SaveCameraStart") as ParameterButton).Clicked += SaveStart_Clicked;
        (ShadowMediaUIHost.GetUI("SaveCameraStop") as ParameterButton).Clicked += SaveStop_Clicked;

        //resizedMat = new Mat();
        this.FpsAd = new FPSAdjuster.FPSAdjuster();
        this.FpsAd.Fps = 20;
        this.FpsAd.Start();
    }

    private void SaveStop_Clicked(object sender, EventArgs e)
    {
        this.IsSaveStop = true;

    }


    private void SaveStart_Clicked(object sender, EventArgs e)
    {
        size = new Size(1920, 1080);
        
        //Debug.Log(Camera.Colorimagematc3.Width);
        if (filename != null)
        {
            vw = new VideoWriter();
            vw.Open(filePath, codec, 20, size, true);
            thread = new Thread(new ThreadStart(SaveCameraData));
            thread.Start();
        }
    }

    private void ChooseFolder_Clicked(object sender, EventArgs e)
    {
        filename = "";
        OpenFileDialog.OpenFileDialog.Save(ref filename);
        filePath = filename + ".avi";
    }

    void SaveCameraData()
    {

        while (true)
        {
          
            if (vw != null)
            {
                vw.Write(Camera.Colorimagematc3);
                Debug.Log("ok");
            }
            if (IsSaveStop)
            {
                IsSaveStop = false;
                break;
            }
        }

        vw.Dispose();
        vw = new VideoWriter();
        thread.Abort();
    }


    void OnDestroy()
    {

        if (thread != null)
        {
            thread.Abort();
        }
        if (filename != null)
        {
            filename = null;
        }
        if(vw!= null)
        {
            vw.Dispose();
        }

    }

}

