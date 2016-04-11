using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class polygon : AImageProcesser
    {
        
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 50;
        private Mat grayimage = new Mat();
        // Mat dstMat = new Mat()
        Random rand = new Random();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        Scalar color;
        Scalar colorBack;


        public polygon():base()
        {
            (UIHost.GetUI("Polygon_con_R") as ParameterSlider).ValueChanged += Polygon_con_R_ValueChanged;
            (UIHost.GetUI("Polygon_con_G") as ParameterSlider).ValueChanged += Polygon_con_G_ValueChanged;
            (UIHost.GetUI("Polygon_con_B") as ParameterSlider).ValueChanged += Polygon_con_B_ValueChanged;
            (UIHost.GetUI("Polygon_bgd_R") as ParameterSlider).ValueChanged += Polygon_bgd_R_ValueChanged;
            (UIHost.GetUI("Polygon_bgd_G") as ParameterSlider).ValueChanged += Polygon_bgd_G_ValueChanged;
            (UIHost.GetUI("Polygon_bgd_B") as ParameterSlider).ValueChanged += Polygon_bgd_B_ValueChanged;
        }

        private void Polygon_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Polygon_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Polygon_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;
            
        }

        private void Polygon_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Polygon_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Polygon_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();
            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
                          
            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);
            
            List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();
            
            for (int i = 0; i < contour.Length; i++)
            {
                if(Cv2.ContourArea(contour[i]) > 1000)
                {
                    CvPoints.Clear();

                    for (int j = 0; j < contour[i].Length; j += this.sharpness)
                    {

                        CvPoints.Add(contour[i][j]);
                    }

                    this.List_Contours.Add(CvPoints);
                    //Cv2.FillConvexPoly(dst, CvPoints, Scalar.Yellow,  OpenCvSharp.LineType.Link4, 0);
                }
                
            }
            var _contour = List_Contours.ToArray();

            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3,colorBack);
            Cv2.DrawContours(dst, _contour, 0, color, -1, OpenCvSharp.LineType.Link8);
        }

        public override string ToString()
        {
            return "Polygon";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Polygon;
        }

        public bool IsFirstFrame { get; private set; }
    }

  
}
