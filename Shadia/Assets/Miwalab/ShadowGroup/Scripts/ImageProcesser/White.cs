using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser
{

    public class White : AShadowImageProcesser
    {
        Mat WhiteMat;

        public White() : base()
        {

        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            WhiteMat = new Mat(src.Height, src.Width, MatType.CV_8UC3, new Scalar(255, 255, 255));

            dst = WhiteMat;
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.White;
        }

        public override string ToString()
        {
            return "White";
        }

    }
}
