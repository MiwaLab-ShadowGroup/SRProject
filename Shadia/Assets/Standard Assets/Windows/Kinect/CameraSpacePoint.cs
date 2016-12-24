using RootSystem = System;
using System.Linq;
using System.Collections.Generic;
namespace Windows.Kinect
{
    //
    // Windows.Kinect.CameraSpacePoint
    //
    [RootSystem.Runtime.InteropServices.StructLayout(RootSystem.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct CameraSpacePoint
    {
        public float X;
        public float Y;
        public float Z;

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CameraSpacePoint))
            {
                return false;
            }

            return this.Equals((CameraSpacePoint)obj);
        }

        public bool Equals(CameraSpacePoint obj)
        {
            return X.Equals(obj.X) && Y.Equals(obj.Y) && Z.Equals(obj.Z);
        }

        public static bool operator ==(CameraSpacePoint a, CameraSpacePoint b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CameraSpacePoint a, CameraSpacePoint b)
        {
            return !(a.Equals(b));
        }

        /// <summary>
        /// å∏éZ
        /// </summary>
        /// <param name="point"></param>
        public void decrease(ref CameraSpacePoint point)
        {
            this.X -= point.X;
            this.Y -= point.Y;
            this.Z -= point.Z;
        }

        /// <summary>
        /// é¿êîî{
        /// </summary>
        /// <param name="point"></param>
        public void multiply(float value)
        {
            this.X *= value;
            this.Y *= value;
            this.Z *= value;
        }
    }

}
