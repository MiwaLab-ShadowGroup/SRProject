using UnityEngine;
using System.Collections;
using Windows.Kinect;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using Miwalab.ShadowGroup.Network;
using Miwalab.ShadowGroup.Data;
using Miwalab.ShadowGroup.Thread;

public class ColorImporter : MonoBehaviour
{

    private KinectSensor m_sensor;

    public ColorSourceManager _colormanager;
    ColorImageFormat colorImageFormat;
    ColorFrameReader colorFrameReader;
    ColorFrameSource colorframesourse;
    FrameDescription colorFrameDescription;
    byte[] colordata;
    public ushort[] colors;
    int imageWidth;
    int imageHeight;
    public Mat Colorimagematc4;
    public Mat Colorimagematc3;


    // Use this for initialization
    void Start()
    {

        m_sensor = KinectSensor.GetDefault();

        if (m_sensor != null)
        {
            //Debug.Log("The Kinect ID : " + m_sensor.UniqueKinectId);
            
                m_sensor.Open();
                colorFrameDescription = m_sensor.ColorFrameSource.FrameDescription;
                colorframesourse = m_sensor.ColorFrameSource;
                this.Colorimagematc4 = new Mat(new Size(colorFrameDescription.Width, colorFrameDescription.Height), MatType.CV_8UC4);
                this.Colorimagematc3 = new Mat(new Size(colorFrameDescription.Width, colorFrameDescription.Height), MatType.CV_8UC3);

        }

    }

    // Update is called once per frame
    void Update()
    {


        if (_colormanager == null)
        {
            return;
        }
        if(Colorimagematc4 == null)
        {
            Debug.Log("null");
            return;
        }

        this.colordata = _colormanager.GetColorData();
        //Debug.Log(colordata.Length);
        unsafe
        {
            byte* colormatdata = (byte*)Colorimagematc4.Data;
            int colorlength = colordata.Length;
            for (int i = 0; i < colorlength; i+=4)
            {

                colormatdata[i] = colordata[i];
                colormatdata[i + 1] = colordata[i + 1];
                colormatdata[i + 2] = colordata[i + 2];
                colormatdata[i + 3] = colordata[i + 3];

            }

        }

        Cv2.CvtColor(Colorimagematc4, Colorimagematc3, OpenCvSharp.ColorConversion.BgraToBgr);
    }
}
