using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
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
        public Parameter(ParameterType Type, string Name, object Value)
        {
            this.Type = Type;
            this.Value = Value;
            this.Name = Name;
        }
        public ParameterType Type { set; get; }
        public object Value { set; get; }
        public string Name { set; get; }
    }

    public abstract class AImageProcesser
    {

        public class DepthBody
        {
            public class DepthJoint
            {

                public DepthJoint(CameraSpacePoint dsp, TrackingState ts, KinectSensor sensor)
                {
                    InitializePosition = new Vector3(dsp.X, dsp.Y);
                    localPosition = new Vector3(dsp.X, dsp.Y);
                    vellocity = new Vector3(dsp.X, dsp.Y);
                    acceleration = new Vector3(dsp.X, dsp.Y);
                    vellocity_front = new Vector3(dsp.X, dsp.Y);
                    position_front = new Vector3(dsp.X, dsp.Y);
                    state = ts;

                    _sensor = sensor;
                }
                public DepthJoint(Vector3 initialPosition,  KinectSensor sensor)
                {
                    InitializePosition = initialPosition;
                    localPosition = initialPosition;
                    vellocity = initialPosition;
                    acceleration = initialPosition;
                    vellocity_front = initialPosition;
                    position_front = initialPosition;

                    _sensor = sensor;
                }

                private KinectSensor _sensor;
                public Vector3 localPosition;
                public Vector3 position
                {
                    get
                    {
                        CameraSpacePoint point = new CameraSpacePoint();
                        point.X = localPosition.x;
                        point.Y = localPosition.y;
                        point.Z = localPosition.z;
                        var position = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(point);
                        return new Vector3(
                            position.X,
                            position.Y
                            );
                    }
                }

                public Vector3 velocity_transformed
                {
                    get
                    {
                        CameraSpacePoint point = new CameraSpacePoint();
                        point.X = vellocity.x;
                        point.Y = vellocity.y;
                        point.Z = vellocity.z;
                        var position = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(point);
                        return new Vector3(
                            position.X,
                            position.Y
                            );
                    }
                }

                public Vector3 vellocity_upperCorrect
                {
                    get
                    {
                        var temp = vellocity;
                        temp.y *= -1f;
                        return temp;
                    }
                }

                public Vector3 acceleration_upperCorrect
                {
                    get
                    {
                        var temp = acceleration;
                        temp.x *= -1f;
                        return temp;
                    }
                }

                public TrackingState state;
                public override string ToString()
                {
                    return vellocity.x + ", " + vellocity.y;
                }

                public Vector3 vellocity;
                private Vector3 vellocity_front;
                private Vector3 position_front;
                public Vector3 acceleration;
                public bool IsFirstFrame { set; get; }
                public bool IsSecondFrame { set; get; }

                public readonly Vector3 InitializePosition;

                public void update(CameraSpacePoint dsp, TrackingState ts)
                {
                    localPosition = new Vector3(dsp.X, dsp.Y, dsp.Z);
                    this.state = ts;
                    this.CalcVelAndAccel();
                }

                //通信用
                public void update(Vector3 position, TrackingState ts)
                {
                    localPosition = position;
                    this.state = ts;
                    this.CalcVelAndAccel();
                }

                public void CalcVelAndAccel()
                {
                    if (position_front != InitializePosition)
                    {
                        vellocity = localPosition - position_front;
                        if (vellocity_front != InitializePosition)
                        {
                            acceleration = vellocity - vellocity_front;
                        }
                    }
                    position_front = localPosition;
                    vellocity_front = vellocity;
                }
            }


            public bool IsCaptured
            {
                get
                {
                    return _body.IsTracked;
                }
            }
            private KinectSensor _sensor;
            private Body _body;
            public Dictionary<Windows.Kinect.JointType, DepthJoint> JointDepth { set; get; }
            public DepthBody(Body body)
            {
                _sensor = Windows.Kinect.KinectSensor.GetDefault();
                JointDepth = new Dictionary<JointType, DepthJoint>();
                _body = body;
                foreach (var p in body.Joints)
                {

                    DepthJoint dj = new DepthJoint(p.Value.Position, p.Value.TrackingState, _sensor);

                    JointDepth.Add(p.Key, dj);
                }
            }

            /// <summary>
            /// 通信受信用
            /// </summary>
            /// <param name="jointLength"></param>
            public DepthBody(int jointLength, Vector3 initialPosition)
            {
                _sensor = Windows.Kinect.KinectSensor.GetDefault();
                JointDepth = new Dictionary<JointType, DepthJoint>();
                for (int i =0;  i < jointLength; ++i)
                {

                    DepthJoint dj = new DepthJoint(initialPosition, _sensor);

                    JointDepth.Add((JointType)i, dj);
                }
            }


            public void Update(Body body)
            {
                foreach (var p in body.Joints)
                {
                    JointDepth[p.Key].update(p.Value.Position, p.Value.TrackingState);
                }
            }

        }

        private Body[] bodydata;
        public DepthBody[] depthBodyData;
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
            if (bodyData == null)
            {
                return;
            }
            if (bodydata == null)
            {
                bodydata = new Body[bodyData.Length];
            }
            //通信などの影響によりBodyの長さが変わる恐れあり
            //そのため，再度初期化する
            if (bodyData.Length != bodydata.Length)
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
            //通信などの影響によりBodyの長さが変わる恐れあり
            //そのため，再度初期化する
            if (bodydata.Length > depthBodyData.Length)
            {
                depthBodyData = new DepthBody[bodydata.Length];
            }
            for (int i = 0; i < bodydata.Length; ++i)
            {
                if (depthBodyData[i] == null)
                {
                    depthBodyData[i] = new DepthBody(bodydata[i]);
                    depthBodyData[i].Update(bodydata[i]);
                }
                else
                {
                    depthBodyData[i].Update(bodydata[i]);
                }
            }

        }

        public void UpdateBodyIndexList()
        {
            if (depthBodyData == null) return;
            bodyIdList.Clear();
            for (int i = 0; i < depthBodyData.Length; ++i)
            {
                if (depthBodyData[i].JointDepth[JointType.SpineBase].state != TrackingState.NotTracked) this.bodyIdList.Add(i);

            }
            BodyCount = bodyIdList.Count;
            if (BodyCount != BodyCountFront)
            {
                OnChangedHumanCount(BodyCount);
            }

            BodyCountFront = BodyCount;
        }

        public List<int> bodyIdList = new List<int>();
        public int BodyCount;
        private int BodyCountFront;

        public delegate void ChangedHumanCountHandler(int count);
        public event ChangedHumanCountHandler ChangeHumanCount;
        private void OnChangedHumanCount(int count)
        {
            if (ChangeHumanCount == null) return;
            ChangeHumanCount(count);
        }


    }
    public abstract class AShadowImageProcesser : AImageProcesser
    {
        public abstract ImageProcesser.ImageProcesserType getImageProcesserType();

    }
}
