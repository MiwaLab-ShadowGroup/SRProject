using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Kinect;

namespace Miwalab.ShadowGroup.ImageProcesser.Particle2D
{
    public class TaggedCircleParticle : SerializedCircleParticle
    {
        public JointType jointType { set; get; }
        public int id { set; get; }
        public bool Setupped = false;

        public void DrawDebug(ref Mat mat)
        {
            if (this.Alive)
            {
                if (Size >= 0)
                {
                    point.X = (int)Position.x;
                    point.Y = (int)Position.y;
                    OpenCvSharp.CPlusPlus.Cv2.PutText(mat,((int)jointType).ToString(), point, OpenCvSharp.FontFace.HersheyPlain ,0.5, Color);
                }
            }

        }

        public void AutoReset(ResetType type, List<int> bodyIdList)
        {
            this.Setupped = true;
            switch (type)
            {
                case ResetType.All:
                    this.ResetAsAll();
                    break;
                case ResetType.Only:
                    this.ResetAsSpineBase();
                    break;
                case ResetType.Simbolic6:
                    this.ResetAsSimbolic6();
                    break;
                case ResetType.Simbolic3:
                    this.ResetAsSimbolic3();
                    break;
                default:
                    break;
            }
            if (bodyIdList.Count == 0)
            {
                id = -1;
                return;
            }
            id = bodyIdList[UnityEngine.Random.Range(0, bodyIdList.Count)];

        }

        private void ResetAsSimbolic3()
        {
            int number = UnityEngine.Random.Range(0, 3);
            switch (number)
            {
                case 1:
                    jointType = JointType.HandLeft;
                    break;
                case 2:
                    jointType = JointType.HandRight;
                    break;
                case 0:
                    jointType = JointType.SpineBase;
                    break;;
            }
        }
        private void ResetAsSimbolic6()
        {
            int number = UnityEngine.Random.Range(0, 6);
            switch (number)
            {
                case 0:
                    jointType = JointType.Head;
                    break;
                case 1:
                    jointType = JointType.HandLeft;
                    break;
                case 2:
                    jointType = JointType.HandRight;
                    break;
                case 3:
                    jointType = JointType.SpineBase;
                    break;
                case 4:
                    jointType = JointType.FootLeft;
                    break;
                case 5:
                    jointType = JointType.FootRight;
                    break;
            }
        }

        private void ResetAsSpineBase()
        {
            jointType = JointType.SpineBase;
        }

        private void ResetAsAll()
        {
            jointType = (JointType)UnityEngine.Random.Range(0, 25);
        }

        public enum ResetType
        {
            /// <summary>
            /// 6個　手　足　頭　腰
            /// </summary>
            Simbolic6,

            /// <summary>
            /// 手　腰
            /// </summary>
            Simbolic3,
            /// <summary>
            /// すべて
            /// </summary>
            All,
            /// <summary>
            /// 腰
            /// </summary>
            Only,
        }
    }
}
