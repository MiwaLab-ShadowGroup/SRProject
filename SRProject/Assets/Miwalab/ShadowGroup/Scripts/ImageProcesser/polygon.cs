﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class Polygon : AShadowImageProcesser
    {
        
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 50;
        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        // Mat dstMat = new Mat()
        Random rand = new Random();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours_Buffer = new List<List<Point>>();
        Scalar color;
        Scalar colorBack;


        public Polygon():base()
        {
            (UIHost.GetUI("Polygon_con_R") as ParameterSlider).ValueChanged += Polygon_con_R_ValueChanged;
            (UIHost.GetUI("Polygon_con_G") as ParameterSlider).ValueChanged += Polygon_con_G_ValueChanged;
            (UIHost.GetUI("Polygon_con_B") as ParameterSlider).ValueChanged += Polygon_con_B_ValueChanged;
            (UIHost.GetUI("Polygon_bgd_R") as ParameterSlider).ValueChanged += Polygon_bgd_R_ValueChanged;
            (UIHost.GetUI("Polygon_bgd_G") as ParameterSlider).ValueChanged += Polygon_bgd_G_ValueChanged;
            (UIHost.GetUI("Polygon_bgd_B") as ParameterSlider).ValueChanged += Polygon_bgd_B_ValueChanged;
            (UIHost.GetUI("Polygon_Rate") as ParameterSlider).ValueChanged += Polygon_Rate_ValueChanged;


            (UIHost.GetUI("Polygon_con_R") as ParameterSlider).ValueUpdate();
            (UIHost.GetUI("Polygon_con_G") as ParameterSlider).ValueUpdate();
            (UIHost.GetUI("Polygon_con_B") as ParameterSlider).ValueUpdate();
            (UIHost.GetUI("Polygon_bgd_R") as ParameterSlider).ValueUpdate();
            (UIHost.GetUI("Polygon_bgd_G") as ParameterSlider).ValueUpdate();
            (UIHost.GetUI("Polygon_bgd_B") as ParameterSlider).ValueUpdate();
            (UIHost.GetUI("Polygon_Rate") as ParameterSlider).ValueUpdate();
        }

        private void Polygon_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.sharpness = (int)(e as ParameterSlider.ChangedValue).Value;
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
                      
            //dstMat = new Mat(dst.Height, dst.Width, MatType.CV_8UC4,colorBack);
            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3 , colorBack);

            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);
            
            List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();
            
            for (int i = 0; i < contour.Length; i++)
            {

                CvPoints.Clear();
                if (Cv2.ContourArea(contour[i]) > 1000)
                {

                    for (int j = 0; j < contour[i].Length; j += contour[i].Length/5)
                    {

                        //絶対五回のはず
                        CvPoints.Add(contour[i][j]);
                    }

                    this.List_Contours.Add(new List<Point>(CvPoints));
                    
                }
                
            }
            var _contour = List_Contours.ToArray();

            Cv2.DrawContours(dst, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);
            this.List_Contours_Buffer = this.List_Contours;
            //Cv2.CvtColor(dstMat, dst, OpenCvSharp.ColorConversion.BgraToBgr);

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
