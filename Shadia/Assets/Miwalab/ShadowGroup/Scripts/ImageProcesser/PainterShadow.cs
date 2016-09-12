using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class PainterShadow : AShadowImageProcesser
    {
        Mat _buffer;
        Mat _grayImage =new Mat();

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.PainterShadow;
        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            this.bufferInitialize(src);
            #region Contour


            Cv2.CvtColor(src, _grayImage, OpenCvSharp.ColorConversion.BgrToGray);


            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(_grayImage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            for (int i = 0; i < contour.Length; i++)
            {

                if (Cv2.ContourArea(contour[i]) > 1000)
                {
                    Cv2.DrawContours(_buffer, new Point[][] { contour[i] }, -1, new Scalar(ColorCounter_B,ColorCounter_G,ColorCounter_R), -1, OpenCvSharp.LineType.Link8);


                }

            }
            #endregion

            if (ColorCounter_R > 255)
            {
                ColorCounter_R_Return = false;
            }
            if (ColorCounter_G > 255)
            {
                ColorCounter_G_Return = false;
            }
            if (ColorCounter_B > 255)
            {
                ColorCounter_B_Return = false;
            }

            if (ColorCounter_R < 1)
            {
                ColorCounter_R_Return = true;
            }
            if (ColorCounter_G < 1)
            {
                ColorCounter_G_Return = true;
            }
            if (ColorCounter_B < 1)
            {
                ColorCounter_B_Return = true;
            }


            ColorCounter_R = ColorCounter_R_Return ? ColorCounter_R + 1 : ColorCounter_R - 1;
            ColorCounter_G = ColorCounter_G_Return ? ColorCounter_G + 1 : ColorCounter_G - 1;
            ColorCounter_B = ColorCounter_B_Return ? ColorCounter_B + 1 : ColorCounter_B - 1;

            _buffer = _buffer.Blur(new Size(3, 3));
            _buffer.CopyTo(dst);
        }
        public int ColorCounter_R = 170;
        public int ColorCounter_G = 170;
        public int ColorCounter_B = 0;

        public bool ColorCounter_R_Return = false;
        public bool ColorCounter_G_Return = true;
        public bool ColorCounter_B_Return = true;

        private void bufferInitialize(Mat mat)
        {
            if (this._buffer == null)
            {
                this._buffer = mat.EmptyClone();
            }
        }
    }
}
