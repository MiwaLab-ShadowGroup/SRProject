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
    private CameraSpacePoint[] m_cameraSpacePoints;
    private float m_left = -1;
    private float m_right = 1;
    private float m_top = 1;
    private float m_bottom = -1;
    private float m_front = 8;
    private float m_rear = 0.5f;


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
            m_cameraSpacePoints = new CameraSpacePoint[m_frameDescription.Width * m_frameDescription.Height];
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
        m_mapper.MapDepthFrameToCameraSpace(m_depthData, m_cameraSpacePoints);
        unsafe
        {
            byte* data = (byte*)m_mat.Data;
            int length = this.m_depthData.Length * 3;
            for (int i = 0; i < length; i += 3)
            {
                CameraSpacePoint point = this.m_cameraSpacePoints[i / 3];
                if (point.X > m_left && point.X < m_right && point.Y > m_bottom && point.Y < m_top && point.Z > m_rear && point.Z < m_front)
                {
                    data[i] = 255;
                    data[i + 1] = 255;
                    data[i + 2] = 255;
                }
                else
                {
                    data[i] = 0;
                    data[i + 1] = 0;
                    data[i + 2] = 0;
                }
            }
        }
        foreach (var imageProcesser in this.m_ImagerProcesserList)
        {
            imageProcesser.ImageProcess(ref this.m_mat, ref this.m_mat);
        }
    }

    public override void setUpUI()
    {
        (UIHost.GetUI("Kinect_x_min") as ParameterSlider).ValueChanged += KinectImporter_x_min_ValueChanged;
        (UIHost.GetUI("Kinect_x_max") as ParameterSlider).ValueChanged += KinectImporter_x_max_ValueChanged;
        (UIHost.GetUI("Kinect_y_min") as ParameterSlider).ValueChanged += KinectImporter_y_min_ValueChanged;
        (UIHost.GetUI("Kinect_y_max") as ParameterSlider).ValueChanged += KinectImporter_y_max_ValueChanged;
        (UIHost.GetUI("Kinect_z_min") as ParameterSlider).ValueChanged += KinectImporter_z_min_ValueChanged;
        (UIHost.GetUI("Kinect_z_max") as ParameterSlider).ValueChanged += KinectImporter_z_max_ValueChanged;
    }

    private void KinectImporter_x_min_ValueChanged(object sender, EventArgs e)
    {
        this.m_left = (e as ParameterSlider.ChangedValue).Value;
    }
    private void KinectImporter_x_max_ValueChanged(object sender, EventArgs e)
    {
        this.m_right = (e as ParameterSlider.ChangedValue).Value;
    }
    private void KinectImporter_y_min_ValueChanged(object sender, EventArgs e)
    {
        this.m_bottom = (e as ParameterSlider.ChangedValue).Value;
    }
    private void KinectImporter_y_max_ValueChanged(object sender, EventArgs e)
    {
        this.m_top = (e as ParameterSlider.ChangedValue).Value;
    }
    private void KinectImporter_z_min_ValueChanged(object sender, EventArgs e)
    {
        this.m_rear = (e as ParameterSlider.ChangedValue).Value;
    }
    private void KinectImporter_z_max_ValueChanged(object sender, EventArgs e)
    {
        this.m_front = (e as ParameterSlider.ChangedValue).Value;
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
