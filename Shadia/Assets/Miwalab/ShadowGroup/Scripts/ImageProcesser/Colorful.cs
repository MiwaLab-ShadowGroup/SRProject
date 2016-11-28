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
    class Colorful : AShadowImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 200;
        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        // Mat dstMat = new Mat()
        //Random rand = new Random();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours_Buffer = new List<List<Point>>();
        Scalar color;
        Scalar colorBack;
        DepthBody db;

        double changeX;
        double changeY;
        float count;

        List<Vec3d> contour_Center = new List<Vec3d>();
        List<OpenCvSharp.CPlusPlus.Point> contour_Xmin = new List<Point>();
        List<OpenCvSharp.CPlusPlus.Point> contour_Xmax = new List<Point>();
        List<OpenCvSharp.CPlusPlus.Point> betweenPt = new List<Point>();


        public Colorful() : base()
        {
            (ShadowMediaUIHost.GetUI("Colorful_con_R") as ParameterSlider).ValueChanged += Colorful_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Colorful_con_G") as ParameterSlider).ValueChanged += Colorful_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Colorful_con_B") as ParameterSlider).ValueChanged += Colorful_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Colorful_bgd_R") as ParameterSlider).ValueChanged += Colorful_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Colorful_bgd_G") as ParameterSlider).ValueChanged += Colorful_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Colorful_bgd_B") as ParameterSlider).ValueChanged += Colorful_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Colorful_change_x") as ParameterSlider).ValueChanged += Colorful_change_x_ValueChanged;
            (ShadowMediaUIHost.GetUI("Colorful_change_y") as ParameterSlider).ValueChanged += Colorful_change_y_ValueChanged;
            (ShadowMediaUIHost.GetUI("Colorful_Rate") as ParameterSlider).ValueChanged += Colorful_Rate_ValueChanged;
            (ShadowMediaUIHost.GetUI("Colorful_UseFade") as ParameterCheckbox).ValueChanged += Colorful_UseFade_ValueChanged;

            (ShadowMediaUIHost.GetUI("Colorful_CC_Blue") as ParameterButton).Clicked += Colorful_CC_Blue_Clicked;
            (ShadowMediaUIHost.GetUI("Colorful_CC_Orange") as ParameterButton).Clicked += Colorful_CC_Orange_Clicked;
            (ShadowMediaUIHost.GetUI("Colorful_CC_Yellow") as ParameterButton).Clicked += Colorful_CC_Yellow_Clicked;
            (ShadowMediaUIHost.GetUI("Colorful_CC_Pink") as ParameterButton).Clicked += Colorful_CC_Pink_Clicked;
            (ShadowMediaUIHost.GetUI("Colorful_CC_Green") as ParameterButton).Clicked += Colorful_CC_Green_Clicked;


            (ShadowMediaUIHost.GetUI("Colorful_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Colorful_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Colorful_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Colorful_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Colorful_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Colorful_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Colorful_change_x") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Colorful_change_y") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Colorful_Rate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Colorful_UseFade") as ParameterCheckbox).ValueUpdate();
        }

        private void Colorful_CC_Green_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Colorful_con_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Colorful_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Colorful_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void Colorful_CC_Pink_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Colorful_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Colorful_con_G") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Colorful_con_B") as ParameterSlider).m_slider.value = 255;
        }

        private void Colorful_CC_Yellow_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Colorful_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Colorful_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Colorful_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void Colorful_CC_Orange_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Colorful_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Colorful_con_G") as ParameterSlider).m_slider.value = 125;
            (ShadowMediaUIHost.GetUI("Colorful_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void Colorful_CC_Blue_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Colorful_con_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Colorful_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Colorful_con_B") as ParameterSlider).m_slider.value = 255;
        }

        private void Colorful_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void Colorful_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.sharpness = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Colorful_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Colorful_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Colorful_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Colorful_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Colorful_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Colorful_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Colorful_change_x_ValueChanged(object sender, EventArgs e)
        {
            this.changeX = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Colorful_change_y_ValueChanged(object sender, EventArgs e)
        {
            this.changeY = (double)(e as ParameterSlider.ChangedValue).Value;

        }



        Mat m_buffer;
        Mat m_Addcolor;
        Mat m_Staycolor;
        Mat m_mask_buffer;
        Mat m_face_buffer;
        bool m_UseFade;
        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();
            this.contour_Center.Clear();
            this.contour_Xmin.Clear();
            this.contour_Xmax.Clear();
            this.betweenPt.Clear();

            if (m_buffer == null)
            {
                m_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
                m_mask_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
                m_face_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
                m_Addcolor = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
                m_Staycolor = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
                count = 0;
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
            m_mask_buffer *= 0;
            m_face_buffer *= 0;
            m_Addcolor *= 0;
            

            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            //Cv2.MedianBlur(grayimage, grayimage, 9);
            //dstMat = new Mat(dst.Height, dst.Width, MatType.CV_8UC4,colorBack);
            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);

            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();

            for (int i = 0; i < contour.Length; i++)
            {

                CvPoints.Clear();
                if (Cv2.ContourArea(contour[i]) > 1000)
                {
                    //重心検出処理
                    var cont = contour[i].ToArray();

                    var M = Cv2.Moments(cont);
                    this.contour_Center.Add(new Vec3d((M.M10 / M.M00), (M.M01 / M.M00), Cv2.ContourArea(contour[i]) ));

                    for (int j = 0; j < contour[i].Length; j += contour[i].Length / this.sharpness + 1)
                    {

                        CvPoints.Add(contour[i][j]);

                    }
                    this.List_Contours.Add(new List<Point>(CvPoints));

                }

            }

            //Xmin,Xmax,contour_Centerのソート昇順
            this.contour_Xmin.Sort(delegate (Point p1, Point p2) { return p1.X - p2.X; });
            this.contour_Xmax.Sort(delegate (Point p1, Point p2) { return p1.X - p2.X; });
            //this.contour_Center.Sort(delegate (Point p1, Point p2) { return p1.X - p2.X; });
            this.contour_Center.Sort(delegate (Vec3d v1, Vec3d v2) { return (int)v1.Item0 - (int)v2.Item0; });


            //人と人の間のポイントを決定
            for (int i = 0; i < this.contour_Center.Count - 1; ++i)
            {
                
                this.betweenPt.Add(new Point((this.contour_Center[i].Item0 + this.contour_Center[i + 1].Item0) / 2 + UnityEngine.Random.Range(-10, 10),
                                             (this.contour_Center[i].Item1 + this.contour_Center[i + 1].Item1) / 2 + UnityEngine.Random.Range(-10, 10)));
            }


            //輪郭の描画
            var _contour = List_Contours.ToArray();
            Cv2.DrawContours(m_Addcolor, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);
            Cv2.DrawContours(m_mask_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);

            Cv2.CvtColor(m_mask_buffer, m_mask_buffer, OpenCvSharp.ColorConversion.BgrToGray);
            Cv2.Threshold(m_mask_buffer, m_mask_buffer, 1, 255, OpenCvSharp.ThresholdType.BinaryInv);
            Cv2.CvtColor(m_mask_buffer, m_mask_buffer, OpenCvSharp.ColorConversion.GrayToBgr);

            //いろんな色を付けていく
            /*
            Cv2.CvtColor(m_buffer, m_buffer, OpenCvSharp.ColorConversion.BgrToHsv);
            for (int i = 0; i < BodyData.Length; i++)
            {
                if (BodyData[i].IsTracked)
                {
                    
                    //場所によって色が変わるやつ
                    float bodyHue = 180 * BodyDataOnDepthImage[i].JointDepth[Windows.Kinect.JointType.SpineBase].position.x / src.Width;
                    Vector2 vec = new Vector2(BodyDataOnDepthImage[i].JointDepth[Windows.Kinect.JointType.SpineMid].position.x - BodyDataOnDepthImage[i].JointDepth[Windows.Kinect.JointType.Head].position.x,
                                              BodyDataOnDepthImage[i].JointDepth[Windows.Kinect.JointType.SpineMid].position.y - BodyDataOnDepthImage[i].JointDepth[Windows.Kinect.JointType.Head].position.y);

                    int radius = (int)vec.magnitude;
                    if (radius == 0)
                    {
                        radius = 1;
                    }


                    for (Windows.Kinect.JointType jt = Windows.Kinect.JointType.SpineBase; jt <= Windows.Kinect.JointType.ThumbRight; jt++)
                    {
                        if (BodyData[i].Joints[jt].Position != null)
                        {
                            Cv2.Circle(m_buffer, (int)BodyDataOnDepthImage[i].JointDepth[jt].position.x,
                                                 (int)BodyDataOnDepthImage[i].JointDepth[jt].position.y, radius,
                                                 //new Scalar(255 - 10 * BodyData[i].Joints[jt].Position.Z, 255 - 10 * BodyData[i].Joints[jt].Position.Z, 255 - 10 * BodyData[i].Joints[jt].Position.Z));
                                                 new Scalar(bodyHue + UnityEngine.Random.Range(-15, 15), 230, 230), -1);

                        }
                    }
                    
                }
            }

            Cv2.CvtColor(m_buffer, m_buffer, OpenCvSharp.ColorConversion.HsvToBgr);

            */
          
            //人の中に円を出す
            Cv2.CvtColor(m_Addcolor, m_Addcolor, OpenCvSharp.ColorConversion.BgrToHsv);
            for (int i = 0; i < this.contour_Center.Count; ++i)
            {
                //半径の決定
                //Vector2 vec = new Vector2(BodyDataOnDepthImage[i].JointDepth[Windows.Kinect.JointType.SpineMid].position.x - BodyDataOnDepthImage[i].JointDepth[Windows.Kinect.JointType.Head].position.x,
                //                          BodyDataOnDepthImage[i].JointDepth[Windows.Kinect.JointType.SpineMid].position.y - BodyDataOnDepthImage[i].JointDepth[Windows.Kinect.JointType.Head].position.y);

                //int radius = (int)vec.magnitude;
                Cv2.Circle(m_Addcolor, (int)this.contour_Center[i].Item0, (int)this.contour_Center[i].Item1, (int) (this.contour_Center[i].Item2 * 0.01), new Scalar(count, 200 + 50 * Math.Sin(count ), 200 + 50 * Math.Cos(count)), -1);
            }
            Cv2.CvtColor(m_Addcolor, m_Addcolor, OpenCvSharp.ColorConversion.HsvToBgr);

            //人に中心と人の間に色を出す

            for (int i = 0; i < this.betweenPt.Count; ++i)
            {
                Cv2.Circle(m_Addcolor, this.betweenPt[i], 100, new Scalar(50, 50, 220), -1);

            }


            Cv2.AddWeighted(m_Staycolor, 0.8, m_Addcolor, 0.2, 0, m_Staycolor);

            //ぶらす
            Cv2.GaussianBlur(m_Staycolor, m_Staycolor, new Size(99, 99), 0f);

            //マスクをかぶせる(マイナスする　※値が行き過ぎても自動的に最大最小値になる)
            
            m_face_buffer += m_Staycolor;
            m_face_buffer -= m_mask_buffer;

            //ここでブラーがかかっている奴にたす
            m_buffer += m_face_buffer;

            dst += m_buffer;
            Cv2.GaussianBlur(dst,dst, new Size(9, 9), 0f);


            //Cv.InitFont( );


            this.List_Contours_Buffer = this.List_Contours;
            //Cv2.CvtColor(dstMat, dst, OpenCvSharp.ColorConversion.BgraToBgr);

            //カウントの処理
            count += 1;
            if (count == 360) count = 0;

        }

        public override string ToString()
        {
            return "Colorful";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Colorful;
        }

        public bool IsFirstFrame { get; private set; }
    }


}
