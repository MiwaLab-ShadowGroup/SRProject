using UnityEngine;
using System.Collections;
using Windows.Kinect;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

public class KinectDepthImporter : ASensorImporter{

    //public
    public GameObject kinect;
    public float roof;
    public Vector2 width;
    public Vector2 depth;
    
    //private
    KinectSensor kinectSensor;
    DepthFrameReader depthFrameReader;
    ushort[] depthData;
    CameraSpacePoint[] cameraSpacePoints;
    int imageWidth;
    int imageHeight;
    Mat DepthImage;

	// Use this for initialization
	void Start ()
    {
        this.kinectSensor = KinectSensor.GetDefault();
        this.depthFrameReader = this.kinectSensor.DepthFrameSource.OpenReader();

        this.imageHeight = this.depthFrameReader.DepthFrameSource.FrameDescription.Height;
        this.imageWidth = this.depthFrameReader.DepthFrameSource.FrameDescription.Width;

        this.depthData = new ushort[this.imageWidth * this.imageHeight];
        this.cameraSpacePoints = new CameraSpacePoint[this.depthData.Length];

        this.depthFrameReader.FrameArrived += this.DepthFrameArrived;

        this.kinectSensor.Open();
	}
	
    void DepthFrameArrived(object sender, DepthFrameArrivedEventArgs e)
    {
        try
        {
            var frame = e.FrameReference.AcquireFrame();
            frame.CopyFrameDataToArray(this.depthData);
            kinectSensor.CoordinateMapper.MapDepthFrameToCameraSpace(this.depthData, this.cameraSpacePoints);
            this.srcMat = this.DrawDepthImage(this.cameraSpacePoints);
        }
        catch
        {
            Debug.Log("Error:DepthFrameArrive");
        }
    }
    
    Mat DrawDepthImage(CameraSpacePoint[] data)
    {
        try
        {
            Mat depthImage = new Mat(this.imageHeight, this.imageWidth, MatType.CV_8UC1);

            //舞台内ならば白　背景は黒
            for (int i= 0; i < data.Length; i ++)
            {
                var p = data[i];
                if (p.X > this.width.x && p.X < this.width.y && p.Y > -this.kinect.transform.position.y && p.Y < this.roof && p.Z > this.depth.x && p.Z < this.depth.y)
                {
                    //要検討
                    DepthImage.Set<int>(i, 255);
                }
                else
                {
                    //要検討
                    DepthImage.Set<int>(i, 0);

                }
            }
            return DepthImage;
        }
        catch { Debug.Log("Error:MakeDepthImage");  return null; }
    }

    public override Mat getCvMat()
    {
        return this.DepthImage;
    }
	// Update is called once per frame
	void Update () {
	
	}
}
