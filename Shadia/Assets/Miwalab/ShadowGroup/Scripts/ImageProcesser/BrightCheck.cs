using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class BrightCheck : AShadowImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 6;
        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        // Mat dstMat = new Mat()
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        Scalar color;
        Scalar colorBack;
        Scalar lightRate;
        int dilateNum;
        int gaussianSize1;
        int gaussianSize2;
        int threshold;
        int findAreaTh;
        float lightAnd;



        bool contFind;

        public BrightCheck() : base()
        {
            (ShadowMediaUIHost.GetUI("PtsImg_con_R") as ParameterSlider).ValueChanged += BrightCheck_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_con_G") as ParameterSlider).ValueChanged += BrightCheck_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_con_B") as ParameterSlider).ValueChanged += BrightCheck_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_R") as ParameterSlider).ValueChanged += BrightCheck_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_G") as ParameterSlider).ValueChanged += BrightCheck_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_B") as ParameterSlider).ValueChanged += BrightCheck_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_Dilate") as ParameterSlider).ValueChanged += BrightCheck_Dilate_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_GaussianSize1") as ParameterSlider).ValueChanged += BrightCheck_GaussianSize1_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_LightAnd") as ParameterSlider).ValueChanged += BrightCheck_LightAnd_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_GaussianSize2") as ParameterSlider).ValueChanged += BrightCheck_GaussianSize2_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_Threshold") as ParameterSlider).ValueChanged += BrightCheck_Threshold_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_findAreaTh") as ParameterSlider).ValueChanged += BrightCheck_findAreaTh_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_Rate") as ParameterSlider).ValueChanged += BrightCheck_Rate_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_UseFade") as ParameterCheckbox).ValueChanged += BrightCheck_UseFade_ValueChanged;
            (ShadowMediaUIHost.GetUI("PtsImg_contFind") as ParameterCheckbox).ValueChanged += BrightCheck_contFind_ValueChanged;

            (ShadowMediaUIHost.GetUI("PtsImg_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_Dilate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_GaussianSize1") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_LightAnd") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_GaussianSize2") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_Threshold") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_findAreaTh") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_Rate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_UseFade") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PtsImg_contFind") as ParameterCheckbox).ValueUpdate();
        }


        private void BrightCheck_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void BrightCheck_contFind_ValueChanged(object sender, EventArgs e)
        {
            this.contFind = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void BrightCheck_Threshold_ValueChanged(object sender, EventArgs e)
        {
            this.threshold = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_findAreaTh_ValueChanged(object sender, EventArgs e)
        {
            this.findAreaTh = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_Dilate_ValueChanged(object sender, EventArgs e)
        {
            this.dilateNum = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_LightAnd_ValueChanged(object sender, EventArgs e)
        {
            this.lightRate.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.lightRate.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.lightRate.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_GaussianSize1_ValueChanged(object sender, EventArgs e)
        {
            this.gaussianSize1 = (int)(e as ParameterSlider.ChangedValue).Value;
            if (this.gaussianSize1 % 2 == 0)
            {
                this.gaussianSize1--;
            }

        }

        private void BrightCheck_GaussianSize2_ValueChanged(object sender, EventArgs e)
        {
            this.gaussianSize2 = (int)(e as ParameterSlider.ChangedValue).Value;
            if (this.gaussianSize2 % 2 == 0)
            {
                this.gaussianSize2--;
            }

        }

        private void BrightCheck_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.sharpness = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void BrightCheck_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void BrightCheck_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void BrightCheck_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void BrightCheck_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void BrightCheck_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }





        Mat m_buffer;
        Mat m_lightMask;
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
            m_lightMask = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, lightRate);


            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            Cv2.Dilate(grayimage, grayimage, null, null, this.dilateNum);

            //ブラーで滑らかにぼかす 一回目
            Cv2.GaussianBlur(grayimage, grayimage, new Size(this.gaussianSize1, this.gaussianSize1), 0f);


            //定数乗する
            Cv2.CvtColor(m_lightMask, m_lightMask, OpenCvSharp.ColorConversion.BgrToGray);
            grayimage = grayimage.Mul(m_lightMask);

            //ブラーで滑らかにぼかす 一回目
            Cv2.GaussianBlur(grayimage, grayimage, new Size(this.gaussianSize2, this.gaussianSize2), 0f);


            //二値化
            if (this.threshold != 0)
            {
                Cv2.Threshold(grayimage, grayimage, this.threshold, 255, OpenCvSharp.ThresholdType.Binary);
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
                Cv2.CvtColor(grayimage, grayimage, OpenCvSharp.ColorConversion.GrayToBgr);
                m_buffer += grayimage;
                //Debug.Log("nocont"   );
            }



            dst += m_buffer;

        }

        public override string ToString()
        {
            return "BrightCheck";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.BrightCheck;
        }

        public bool IsFirstFrame { get; private set; }
    }


}
