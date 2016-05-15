using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Kinect;

namespace Miwalab.ShadowGroup.Data
{
    /// <summary>
    /// 非同期読み込み　書き出しを行うためのデータ
    /// </summary>
    public class HumanPoints : List<CameraSpacePoint>
    {
        public HumanPoints() : base() { }
        public HumanPoints(int capacity) : base(capacity) { }
        public HumanPoints(IEnumerable<CameraSpacePoint> collection) : base(collection) { }

        public void setData(List<CameraSpacePoint> items)
        {
            lock (this)
            {
                this.Clear();
                AddRange(items);
            }
        }

        /// <summary>
        /// 保持された三次元位置をByte配列に変換
        /// </summary>
        /// <returns></returns>
        public byte[] getData()
        {
            if (this.Count == 0)
            {
                return null;
            }
            //YXZXZXZXZXZXZXZXZ....
            byte[] data = new byte[(this.Count * 2 + 1) * sizeof(float)];

            lock (this)
            {
                //個数　×　floatの長さ　×　（X　Z(Yは一定と仮定)）
                int counter = 0;

                Array.Copy(BitConverter.GetBytes(this[0].Y), 0, data, counter, sizeof(float));
                counter += sizeof(float);

                foreach (var p in this)
                {
                    Array.Copy(BitConverter.GetBytes(p.X), 0, data, counter, sizeof(float));
                    counter += sizeof(float);
                    Array.Copy(BitConverter.GetBytes(p.Z), 0, data, counter, sizeof(float));
                    counter += sizeof(float);
                }
            }
            return data;

        }


        public void setData(byte[] data)
        {
            lock (this)
            {
                this.Clear();
                //個数　×　floatの長さ　×　（X　Z(Yは一定と仮定)）
                int counter = 0;
                float Y = BitConverter.ToSingle(data, counter);
                counter += sizeof(float);
                foreach (var p in this)
                {
                    CameraSpacePoint point = new CameraSpacePoint();
                    point.X = BitConverter.ToSingle(data, counter);
                    counter += sizeof(float);
                    point.Z = BitConverter.ToSingle(data, counter);
                    counter += sizeof(float);
                    point.Y = Y;

                    this.Add(point);

                }
            }
            return;
        }

    }
}
