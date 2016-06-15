using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class Black : AShadowImageProcesser
    {
        Mat BlackMat;

        public Black():base()
        {

        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            BlackMat = new Mat(src.Height, src.Width, MatType.CV_8UC3, new Scalar(0, 0, 0));

            dst = BlackMat;
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Black;
        }

        public override string ToString()
        {
            return "Black";
        }

    }
}
