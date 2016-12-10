using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class PtsImgProcesser : AShadowImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 6;
        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        // Mat dstMat = new Mat()
        Random rand = new Random();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        Scalar color;
        Scalar colorBack;
        int dilateNum;
        int gaussianNum;
        int gaussianSize;
        int threshold;
        int findAreaTh;

        bool contFind;

        public PtsImgProcesser() : base()
        {
            (ShadowMediaUIHost.GetUI("PtsImg_con_R") as ParameterSlider).ValueChanged += PtsImgProcesser_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_con_G") as ParameterSlider).ValueChanged += PtsImgProcesser_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_con_B") as ParameterSlider).ValueChanged += PtsImgProcesser_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_R") as ParameterSlider).ValueChanged += PtsImgProcesser_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_G") as ParameterSlider).ValueChanged += PtsImgProcesser_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_B") as ParameterSlider).ValueChanged += PtsImgProcesser_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_Dilate") as ParameterSlider).ValueChanged += PtsImgProcesser_Dilate_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_GaussianSize") as ParameterSlider).ValueChanged += PtsImgProcesser_GaussianSize_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_GaussianNum") as ParameterSlider).ValueChanged += PtsImgProcesser_GaussianNum_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_Threshold") as ParameterSlider).ValueChanged += PtsImgProcesser_Threshold_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_findAreaTh") as ParameterSlider).ValueChanged += PtsImgProcesser_findAreaTh_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_Rate") as ParameterSlider).ValueChanged += PtsImgProcesser_Rate_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_UseFade") as ParameterCheckbox).ValueChanged += PtsImgProcesser_UseFade_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_contFind") as ParameterCheckbox).ValueChanged += PtsImgProcesser_contFind_ValueChanged;

            (ShadowMediaUIHost.GetUI("PtsImg_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_Dilate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_GaussianSize") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_GaussianNum") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_Threshold") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_findAreaTh") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_Rate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_UseFade") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_contFind") as ParameterCheckbox).ValueUpdate();
        }


        private void PtsImgProcesser_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void PtsImgProcesser_contFind_ValueChanged(object sender, EventArgs e)
        {
            this.contFind = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void PtsImgProcesser_Threshold_ValueChanged(object sender, EventArgs e)
        {
            this.threshold = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void PtsImgProcesser_findAreaTh_ValueChanged(object sender, EventArgs e)
        {
            this.findAreaTh = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        
        private void PtsImgProcesser_Dilate_ValueChanged(object sender, EventArgs e)
        {
            this.dilateNum = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void PtsImgProcesser_GaussianNum_ValueChanged(object sender, EventArgs e)
        {
            this.gaussianNum = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void PtsImgProcesser_GaussianSize_ValueChanged(object sender, EventArgs e)
        {
            this.gaussianSize = (int)(e as ParameterSlider.ChangedValue).Value;
            if (this.gaussianSize % 2 == 0 )
            {
                this.gaussianSize--;
            }

        }

        private void PtsImgProcesser_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.sharpness = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void PtsImgProcesser_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void PtsImgProcesser_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void PtsImgProcesser_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void PtsImgProcesser_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void PtsImgProcesser_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void PtsImgProcesser_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

   



        Mat m_buffer;
        bool m_UseFade;
        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();

            if (m_buffer == null)
            {
                m_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
            }
            else
            {
                if (this.m_UseFade)
                {
                    m_buffer *= 0.9;
                }
                else
                {
                    m_buffer *= 0;
                }
            }

            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            Cv2.Dilate(grayimage,grayimage,null,null, this.dilateNum );

            //ブラーで滑らかにぼかす
            
            Cv2.GaussianBlur(grayimage,grayimage, new Size(this.gaussianSize, this.gaussianSize), 0f);
            

            //二値化
            if (this.threshold != 0)
            {
                Cv2.Threshold(grayimage,grayimage,this.threshold, 255,OpenCvSharp.ThresholdType.Binary );
            }

            if (this.contFind)
            {

                dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);

                Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
                HierarchyIndex[] hierarchy;

                Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

                List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();

                for (int i = 0; i < contour.Length; i++)
                {

                    CvPoints.Clear();
                    if (Cv2.ContourArea(contour[i]) > this.findAreaTh)
                    {

                        for (int j = 0; j < contour[i].Length; j += contour[i].Length / this.sharpness + 1)
                        {

                            //絶対五回のはず
                            CvPoints.Add(contour[i][j]);
                        }

                        this.List_Contours.Add(new List<Point>(CvPoints));

                    }

                }
                var _contour = List_Contours.ToArray();

                Cv2.DrawContours(m_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);
            }
            else
            {
                Cv2.CvtColor(grayimage,m_buffer, OpenCvSharp.ColorConversion.GrayToBgr);
            }



            dst += m_buffer;

        }

        public override string ToString()
        {
            return "PtsImgProcesser";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.PtsImgProcesser;
        }

        public bool IsFirstFrame { get; private set; }
    }


}
