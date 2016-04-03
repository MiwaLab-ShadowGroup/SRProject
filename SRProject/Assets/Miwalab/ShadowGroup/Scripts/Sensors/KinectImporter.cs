using UnityEngine;
using System.Collections;
using Windows.Kinect;
using OpenCvSharp.CPlusPlus;
using System;

public class KinectImporter : ASensorImporter
{
    public DepthSourceManager _depthManager;
    private KinectSensor m_sensor;
    private ushort[] m_depthData;
    private CoordinateMapper m_mapper;
    private Mat m_mat;
    public MatType m_matType = MatType.CV_8UC3;
    private FrameDescription m_frameDescription;
    private DepthFrameSource m_depthFrameSource;

    // Use this for initialization
    void Start()
    {
        m_sensor = KinectSensor.GetDefault();
        this.m_ImagerProcesserList = new System.Collections.Generic.List<Miwalab.ShadowGroup.ImageProcesser.AImageProcesser>();
        if (m_sensor != null)
        {
            Debug.Log("The Kinect ID : " + m_sensor.UniqueKinectId);
            m_mapper = this.m_sensor.CoordinateMapper;
            if (!m_sensor.IsOpen)
            {
                m_sensor.Open();
                m_frameDescription = m_sensor.DepthFrameSource.FrameDescription;
                m_depthFrameSource = m_sensor.DepthFrameSource;
                this.m_mat = new Mat(new Size(m_frameDescription.Width, m_frameDescription.Height), this.m_matType);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (_depthManager == null)
        {
            return;
        }
        m_depthData = _depthManager.GetData();
        unsafe
        {
            byte* data = (byte*)m_mat.Data;
            int length = this.m_depthData.Length * 3;
            ushort maxDistance = 8000;
            ushort minDistance = 500;
            for (int i = 0; i < length; i += 3)
            {
                ushort distance = this.m_depthData[i / 3];
                if (distance < maxDistance && distance > minDistance)
                {
                    byte _depthData = (byte)((float)distance / maxDistance * byte.MaxValue);
                    data[i] = _depthData;
                    data[i + 1] = _depthData;
                    data[i + 2] = _depthData;
                }
                else
                {
                    data[i] = 0;
                    data[i + 1] = 0;
                    data[i + 2] = 0;
                }


            }
        }
        foreach(var imageProcesser in this.m_ImagerProcesserList)
        {
            imageProcesser.ImageProcess(ref this.m_mat, ref this.m_mat);
        }
    }

    public override Mat getCvMat()
    {
        return this.m_mat;
    }

    public override MatType getMatType()
    {
        return this.m_matType;
    }
}
