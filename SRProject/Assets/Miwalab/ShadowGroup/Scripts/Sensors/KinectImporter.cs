using UnityEngine;
using System.Collections;
using Windows.Kinect;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using Miwalab.ShadowGroup.Network;
using Miwalab.ShadowGroup.Data;
using Miwalab.ShadowGroup.Thread;

public class KinectImporter : ASensorImporter
{
    public DepthSourceManager _depthManager;
    private KinectSensor m_sensor;
    private ushort[] m_depthData;
    public ushort[] m_SaveDepth;
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
    private ReadData m_readdata;
    public GameObject ReadData;


    #region 送信用
    private NetworkHost m_networkHost;
    private ThreadHost m_threadHost;
    private bool m_isGettingData = true;
    private float m_gettingPlaneHeight = 0;
    private float m_gettingHeightDiff = 0.01f;
    private const string clientName = "Importer_Sender";
    private HumanPoints m_HumanCenterPositions;
    private bool m_IsUpdatedSendData;
    public TextAsset RemoteEPSettings;
    private RemoteManager m_remoteManager;
    private CameraSpacePoint m_kinectPosition = new CameraSpacePoint();
    #endregion
    // Use this for initialization
    void Start()
    {
        this.InitializeNetwork();

        m_sensor = KinectSensor.GetDefault();
        this.m_ImagerProcesserList = new System.Collections.Generic.List<Miwalab.ShadowGroup.ImageProcesser.AShadowImageProcesser>();
        this.m_AfterEffectList = new System.Collections.Generic.List<Miwalab.ShadowGroup.AfterEffect.AAfterEffect>();
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
        //readdata = ReadData.GetComponent<ReadData>();
    }

    private void InitializeNetwork()
    {
        m_remoteManager = new RemoteManager(this.RemoteEPSettings);
        m_HumanCenterPositions = new HumanPoints();
        m_networkHost = NetworkHost.GetInstance();
        m_networkHost.AddClient(NetworkSettings.KinectImporter_PositionSendPort, clientName);
        m_threadHost = ThreadHost.GetInstance();
        m_threadHost.CreateNewThread(new ContinuouslyThread(SendMethod), clientName);
        m_threadHost.ThreadStart(clientName);

    }

    /// <summary>
    /// 送信用関数
    /// </summary>
    private void SendMethod()
    {
        if (m_IsUpdatedSendData == true)
        {
            byte[] data = this.m_HumanCenterPositions.getData();
            if (data == null)
            {
                return;
            }
            m_networkHost.SendTo(clientName, data, m_remoteManager.RemoteEPs);
            this.m_IsUpdatedSendData = false;
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
        m_SaveDepth = m_depthData;
        m_mapper.MapDepthFrameToCameraSpace(m_depthData, m_cameraSpacePoints);

        List<CameraSpacePoint> points = new List<CameraSpacePoint>();
        List<int> counts = new List<int>();
        //接続しているか否かのフラグ
        bool _flag = false;
        unsafe
        {
            byte* data = (byte*)m_mat.Data;
            int length = this.m_depthData.Length * 3;
            CameraSpacePoint _point;
            int _count;
            for (int i = 0; i < length; i += 3)
            {
                CameraSpacePoint point = this.m_cameraSpacePoints[i / 3];
                if (point.X > m_left && point.X < m_right && point.Y > m_bottom && point.Y < m_top && point.Z > m_rear && point.Z < m_front)
                {
                    data[i] = 255;
                    data[i + 1] = 255;
                    data[i + 2] = 255;
                    //断面取得
                    #region 断面
                    if (this.m_isGettingData)
                    {
                        if (point.Y > m_gettingPlaneHeight && point.Y < m_gettingPlaneHeight + m_gettingHeightDiff)
                        {
                            if (_flag == false)
                            {
                                points.Add(point);
                                counts.Add(0);
                            }
                            _count = counts[counts.Count - 1];
                            _point = points[points.Count - 1];
                            point.X = (point.X * (_count + m_kinectPosition.X) + _point.X) / (_count + 1);
                            point.Y = (point.Y * (_count + m_kinectPosition.Y) + _point.Y) / (_count + 1);
                            point.Z = (point.Z * (_count + m_kinectPosition.Z) + _point.Z) / (_count + 1);
                            points[points.Count - 1] = point;
                            counts[counts.Count - 1]++;

                            _flag = true;
                        }
                        else
                        {
                            _flag = false;
                        }
                    }
                    #endregion
                }
                else
                {
                    data[i] = 0;
                    data[i + 1] = 0;
                    data[i + 2] = 0;
                }
            }
        }


        this.m_HumanCenterPositions.setData(points);
        this.m_IsUpdatedSendData = true;


        //if (readdata.IsRead)
        //{
        //    m_mat = readdata.playmat;
        //}

        foreach (var imageProcesser in this.m_ImagerProcesserList)
        {
            imageProcesser.ImageProcess(ref this.m_mat, ref this.m_mat);

        }

        for (int i = 0; i < this.m_AfterEffectList.Count; ++i)
        {
            var afterEffect = this.m_AfterEffectList[i];
            afterEffect.ImageProcess(ref this.m_mat, ref this.m_mat);
        }
    }

    public override void setUpUI()
    {
        (ShadowMediaUIHost.GetUI("Kinect_x_min") as ParameterSlider).ValueChanged += KinectImporter_x_min_ValueChanged;
        (ShadowMediaUIHost.GetUI("Kinect_x_max") as ParameterSlider).ValueChanged += KinectImporter_x_max_ValueChanged;
        (ShadowMediaUIHost.GetUI("Kinect_y_min") as ParameterSlider).ValueChanged += KinectImporter_y_min_ValueChanged;
        (ShadowMediaUIHost.GetUI("Kinect_y_max") as ParameterSlider).ValueChanged += KinectImporter_y_max_ValueChanged;
        (ShadowMediaUIHost.GetUI("Kinect_z_min") as ParameterSlider).ValueChanged += KinectImporter_z_min_ValueChanged;
        (ShadowMediaUIHost.GetUI("Kinect_z_max") as ParameterSlider).ValueChanged += KinectImporter_z_max_ValueChanged;


        (ShadowMediaUIHost.GetUI("Kinect_pos_x") as ParameterSlider).ValueChanged += KinectImporter_pos_x_ValueChanged;
        (ShadowMediaUIHost.GetUI("Kinect_pos_y") as ParameterSlider).ValueChanged += KinectImporter_pos_y_ValueChanged;
        (ShadowMediaUIHost.GetUI("Kinect_pos_z") as ParameterSlider).ValueChanged += KinectImporter_pos_z_ValueChanged;

        (ShadowMediaUIHost.GetUI("Kinect_Cut_y") as ParameterSlider).ValueChanged += KinectImporter_Cut_y_ValueChanged;
        (ShadowMediaUIHost.GetUI("Kinect_Cut_diff") as ParameterSlider).ValueChanged += KinectImporter_Cut_diff_ValueChanged;




        (ShadowMediaUIHost.GetUI("Kinect_x_min") as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Kinect_x_max") as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Kinect_y_min") as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Kinect_y_max") as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Kinect_z_min") as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Kinect_z_max") as ParameterSlider).ValueUpdate();

        (ShadowMediaUIHost.GetUI("Kinect_pos_x") as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Kinect_pos_y") as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Kinect_pos_z") as ParameterSlider).ValueUpdate();

        (ShadowMediaUIHost.GetUI("Kinect_Cut_y") as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Kinect_Cut_diff") as ParameterSlider).ValueUpdate();
    }

    private void KinectImporter_pos_x_ValueChanged(object sender, EventArgs e)
    {
        this.m_kinectPosition.X = (e as ParameterSlider.ChangedValue).Value;
    }

    private void KinectImporter_pos_y_ValueChanged(object sender, EventArgs e)
    {
        this.m_kinectPosition.Y = (e as ParameterSlider.ChangedValue).Value;
    }

    private void KinectImporter_pos_z_ValueChanged(object sender, EventArgs e)
    {
        this.m_kinectPosition.Z = (e as ParameterSlider.ChangedValue).Value;
    }

    private void KinectImporter_Cut_diff_ValueChanged(object sender, EventArgs e)
    {
        this.m_gettingHeightDiff = (e as ParameterSlider.ChangedValue).Value;
    }

    private void KinectImporter_Cut_y_ValueChanged(object sender, EventArgs e)
    {
        this.m_gettingPlaneHeight = (e as ParameterSlider.ChangedValue).Value;
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