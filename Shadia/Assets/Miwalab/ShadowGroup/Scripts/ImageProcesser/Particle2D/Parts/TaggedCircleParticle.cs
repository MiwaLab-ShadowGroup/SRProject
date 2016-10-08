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
                case ResetType.Simbolic:
                    this.ResetAsSimbolic();
                    break;
                default:
                    break;
            }
            id = bodyIdList[UnityEngine.Random.Range(0, bodyIdList.Count - 1)];

        }

        private void ResetAsSimbolic()
        {
            int number = UnityEngine.Random.Range(0, 5);
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
            Simbolic,
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
