using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.Kinect;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public enum ParameterType
    {
        Float,
        Double,
        Int,
        UInt,
        Byte,
        String,
        Bool,
        Other
    }
    public class Parameter
    {
        public Parameter(ParameterType Type, string Name ,Object Value)
        {
            this.Type = Type;
            this.Value = Value;
            this.Name = Name;
        }
        public ParameterType Type { set; get; }
        public Object Value { set; get; }
        public string Name { set; get; }
    }

    public abstract class AImageProcesser
    {

        protected class DepthBody
        {
            public struct DepthJoint
            {

                public DepthJoint(DepthSpacePoint dsp, TrackingState ts)
                {
                    position = dsp;
                    state = ts;
                }

                public DepthSpacePoint position;
                public TrackingState state;
            }
            private KinectSensor sensor;
            public Dictionary<Windows.Kinect.JointType,DepthJoint> JointDepth { set; get; }
            public DepthBody(Body body)
            {
                sensor = Windows.Kinect.KinectSensor.GetDefault();
                JointDepth = new Dictionary<JointType, DepthJoint>();
                
                foreach(var p in body.Joints)
                {

                    DepthJoint dj = new DepthJoint(sensor.CoordinateMapper.MapCameraPointToDepthSpace(  p.Value.Position), p.Value.TrackingState);

                    JointDepth.Add(p.Key, dj);
                }


            }
        }

        private Body[] bodydata;
        private DepthBody[] depthBodyData;
        protected Body[] BodyData
        {
            get
            {
                return bodydata;
            }
        }
        protected DepthBody[] BodyDataOnDepthImage
        {
            get
            {
                return depthBodyData;
            }
        }

        public abstract void ImageProcess(ref Mat src, ref Mat dst);
        public void SetBody(Body[] bodyData)
        {
            if(bodyData == null)
            {
                return;
            }
            if (bodydata == null)
            {
                bodydata = new Body[bodyData.Length];
            }
            bodyData.CopyTo(bodydata, 0);
            this.UpdateBody();
        }

        private void UpdateBody()
        {
            if (depthBodyData == null)
            {
                depthBodyData = new DepthBody[bodydata.Length];
            }

            UnityEngine.Assertions.Assert.IsTrue(bodydata.Length == depthBodyData.Length);

            for (int i = 0; i < bodydata.Length; ++i)
            {
                depthBodyData[i] = new DepthBody(bodydata[i]);
            }

        }

    }
    public abstract class AShadowImageProcesser : AImageProcesser
    {
        public abstract ImageProcesser.ImageProcesserType getImageProcesserType();

    }
}
