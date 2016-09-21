using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class Electrical : AShadowImageProcesser
    {
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        bool flag = false;
        int sharpness = 6;
        float ctl;
        double deleteTh;
        double contDist;
        private Mat grayimage = new Mat();
        private Mat preGrayimage;
        private Mat dstMat = new Mat();
        System.Random rand = new System.Random();
        float randomTh;
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours_Buffer = new List<List<Point>>();
        Scalar color;
        Scalar colorLine;
        Scalar colorElec;
        Scalar colorBuffer;

        List<OpenCvSharp.CPlusPlus.Vec3i> contour_Center = new List<Vec3i>();
        List<OpenCvSharp.CPlusPlus.Point> contour_Ymin = new List<Point>();
        List<OpenCvSharp.CPlusPlus.Point> contour_Ymax = new List<Point>();
        List<List<OpenCvSharp.CPlusPlus.Point>> polyLinePoint = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> deleteLinePoint = new List<List<Point>>();

        List<int> deleteNum = new List<int>();
        List<OpenCvSharp.CPlusPlus.Point> PolyPoints = new List<Point>();
        List<OpenCvSharp.CPlusPlus.Point> MinPolyPoints = new List<Point>();
        List<OpenCvSharp.CPlusPlus.Point> DeletePoints = new List<Point>();
        List<OpenCvSharp.CPlusPlus.Point> DistanceList = new List<Point>();

        OpenCvSharp.CPlusPlus.Point yMinBuffer;
        OpenCvSharp.CPlusPlus.Point yMaxBuffer;
        int distBuffer;
        bool deleteCheck = false;

        List<OpenCvSharp.CPlusPlus.Point> bezierPt = new List<Point>();
        int bezierCtl = 1;
        int LKsize = 3;
        Mat velx = new Mat();
        Mat vely = new Mat();
        int dx, dy, rows, cols;
        float flowLength;

        bool IsPair;
        bool m_UseFade;

        Mat m_buffer;

        public Electrical() : base()
        {

            (ShadowMediaUIHost.GetUI("Electrical_con_R") as ParameterSlider).ValueChanged += Electrical_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Electrical_con_G") as ParameterSlider).ValueChanged += Electrical_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Electrical_con_B") as ParameterSlider).ValueChanged += Electrical_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Electrical_bgd_R") as ParameterSlider).ValueChanged += Electrical_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Electrical_bgd_G") as ParameterSlider).ValueChanged += Electrical_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Electrical_bgd_B") as ParameterSlider).ValueChanged += Electrical_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Electrical_ctl") as ParameterSlider).ValueChanged += Electrical_ctl_ValueChanged;
            (ShadowMediaUIHost.GetUI("Electrical_deleteTh") as ParameterSlider).ValueChanged += Electrical_deleteTh_ValueChanged;
            (ShadowMediaUIHost.GetUI("Electrical_Rate") as ParameterSlider).ValueChanged += Electrical_Rate_ValueChanged;
            (ShadowMediaUIHost.GetUI("Electrical_UseFade") as ParameterCheckbox).ValueChanged += Electrical_UseFade_ValueChanged;

            (ShadowMediaUIHost.GetUI("Electrical_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Electrical_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Electrical_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Electrical_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Electrical_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Electrical_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Electrical_ctl") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Electrical_deleteTh") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Electrical_Rate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Electrical_UseFade") as ParameterCheckbox).ValueUpdate();
        }
        private void Electrical_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void Electrical_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.sharpness = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void Electrical_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.colorLine.Val1 = 50 + (double)(e as ParameterSlider.ChangedValue).Value;
            if (this.colorLine.Val1 > 255) this.colorLine.Val1 = 255;
            colorBuffer = color;

        }
        private void Electrical_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.colorLine.Val0 = 50 + (double)(e as ParameterSlider.ChangedValue).Value;
            if (this.colorLine.Val0 > 255) this.colorLine.Val0 = 255;
            colorBuffer = color;

        }
        private void Electrical_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.colorLine.Val2 = 50 + (double)(e as ParameterSlider.ChangedValue).Value;
            if (this.colorLine.Val2 > 255) this.colorLine.Val2 = 255;
            colorBuffer = color;
        }
        private void Electrical_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorElec.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }
        private void Electrical_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorElec.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }
        private void Electrical_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorElec.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Electrical_ctl_ValueChanged(object sender, EventArgs e)
        {
            this.ctl = (float)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Electrical_deleteTh_ValueChanged(object sender, EventArgs e)
        {
            this.deleteTh = (double)(e as ParameterSlider.ChangedValue).Value;

        }



        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();
            this.contour_Center.Clear();




            colorBuffer = color;

            if (m_buffer == null)
            {
                m_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, new Scalar(0,0,0));
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
            //平滑化処理中央値　21はよくわからん笑
            Cv2.MedianBlur(grayimage, grayimage, 21);
            //dstMat = new Mat(dst.Height, dst.Width, MatType.CV_8UC4,colorBack);
            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, new Scalar(0,0,0));

            //オプティカルフロー用前フレーム画像
            if (!this.flag)
            {
                Cv2.Canny(grayimage, grayimage, 1, 1, 3);
                this.preGrayimage = this.grayimage.Clone();
                cols = grayimage.Width;
                rows = grayimage.Height;
                this.velx = new Mat(rows, cols, MatType.CV_32FC1);
                this.vely = new Mat(rows, cols, MatType.CV_32FC1);

                this.flag = true;
            }




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
                    this.contour_Center.Add(new Vec3i((int)(M.M10 / M.M00), (int)(M.M01 / M.M00), i));

                    for (int j = 0; j < contour[i].Length; j += contour[i].Length / this.sharpness + 1)
                    //for (int j = 0; j < contour[i].Length; j++)
                    {
                        //絶対五回のはず 
                        CvPoints.Add(contour[i][j]);

                    }

                    this.List_Contours.Add(new List<Point>(CvPoints));

                }

            }

            //contour_Centerのソート昇順
            this.contour_Center.Sort(delegate (Vec3i p1, Vec3i p2) { return (p1.Item0 - p2.Item0); });

            ////輪郭描画
            var _contour = List_Contours.ToArray();
            Cv2.DrawContours(m_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);

            /*
            for (int i = 0; i < this.List_Contours.Count; i++)
            {
                for (int  j = 0; j < this.List_Contours[i].Count; j++)
                {
                    Cv2.Circle(m_buffer, this.List_Contours[i][j], j, new Scalar(255, 255, 255));
                }

            }
            */









            //近い輪郭点をつなぐ

            for (int i = 0; i < this.contour_Center.Count; ++i)
            {

                //for (int j = contour[this.contour_Center[i].Item2].Length / 2; j < contour[this.contour_Center[i].Item2].Length; j += UnityEngine.Random.Range((this.sharpness / 4), (this.sharpness / 3)))
                for (int j = 0; j < contour[this.contour_Center[i].Item2].Length; j += UnityEngine.Random.Range((this.sharpness / 4), (this.sharpness / 3)))
                {
                    IsPair = false;

                    //次の輪郭
                    if (i < this.contour_Center.Count - 1)
                    {
                        for (int k = 0; k < contour[this.contour_Center[i + 1].Item2].Length / 2; k += (this.sharpness / 5))
                        {

                            contDist = Math.Sqrt(Math.Pow(contour[this.contour_Center[i].Item2][j].X - contour[this.contour_Center[i + 1].Item2][k].X, 2) +
                                                Math.Pow(contour[this.contour_Center[i].Item2][j].Y - contour[this.contour_Center[i + 1].Item2][k].Y, 2));

                            randomTh = UnityEngine.Random.Range(-0.3f, 0.7f);
                            if (contDist / deleteTh < randomTh)
                            //if(Math.Abs( contour[this.contour_Center[i].Item2][j].X - contour[this.contour_Center[i + 1].Item2][k].X) < deleteTh)
                            {
                                Point midPt = new Point((contour[this.contour_Center[i].Item2][j].X + contour[this.contour_Center[i + 1].Item2][k].X) / 2 + UnityEngine.Random.Range(-(float)contDist / 4, (float)contDist / 4),
                                                        (contour[this.contour_Center[i].Item2][j].Y + contour[this.contour_Center[i + 1].Item2][k].Y) / 2 + UnityEngine.Random.Range(-(float)contDist / 4, (float)contDist / 4));
                                Cv2.Line(m_buffer, contour[this.contour_Center[i].Item2][j], midPt, colorElec, 1);
                                Cv2.Line(m_buffer, midPt, contour[this.contour_Center[i + 1].Item2][k], colorElec, 1);
                                IsPair = true;
                            }
                        }
                    }
                    //前の輪郭
                    if (i > 0)
                    {
                        for (int k = contour[this.contour_Center[i - 1].Item2].Length / 2; k < contour[this.contour_Center[i - 1].Item2].Length; k += (this.sharpness / 5))
                        {

                            contDist = Math.Sqrt(Math.Pow(contour[this.contour_Center[i].Item2][j].X - contour[this.contour_Center[i - 1].Item2][k].X, 2) +
                                                Math.Pow(contour[this.contour_Center[i].Item2][j].Y - contour[this.contour_Center[i - 1].Item2][k].Y, 2));

                            randomTh = UnityEngine.Random.Range(-0.3f, 0.7f);
                            if (contDist / deleteTh < randomTh)
                            //if(Math.Abs( contour[this.contour_Center[i].Item2][j].X - contour[this.contour_Center[i + 1].Item2][k].X) < deleteTh)
                            {
                                Point midPt = new Point((contour[this.contour_Center[i].Item2][j].X + contour[this.contour_Center[i - 1].Item2][k].X) / 2 + UnityEngine.Random.Range(-(float)contDist / 4, (float)contDist / 4),
                                                        (contour[this.contour_Center[i].Item2][j].Y + contour[this.contour_Center[i - 1].Item2][k].Y) / 2 + UnityEngine.Random.Range(-(float)contDist / 4, (float)contDist / 4));
                                Cv2.Line(m_buffer, contour[this.contour_Center[i].Item2][j], midPt, colorElec, 1);
                                Cv2.Line(m_buffer, midPt, contour[this.contour_Center[i - 1].Item2][k], colorElec, 1);
                                IsPair = true;
                            }
                        }
                    }



                    if (!IsPair)
                    {
                        if (UnityEngine.Random.Range(0,1f) > 0.7 )
                        {
                            Point farPt = new Point(contour[this.contour_Center[i].Item2][j].X + (contour[this.contour_Center[i].Item2][j].X - this.contour_Center[i].Item0) * UnityEngine.Random.Range(0.1f, 0.3f),
                                                       contour[this.contour_Center[i].Item2][j].Y + (contour[this.contour_Center[i].Item2][j].Y - this.contour_Center[i].Item1) * UnityEngine.Random.Range(0.1f, 0.3f));
                            contDist = Math.Sqrt(Math.Pow(contour[this.contour_Center[i].Item2][j].X - farPt.X, 2) +
                                                 Math.Pow(contour[this.contour_Center[i].Item2][j].Y - farPt.Y, 2));

                            Point midPt = new Point((contour[this.contour_Center[i].Item2][j].X + farPt.X) / 2 + UnityEngine.Random.Range(-(float)contDist / 4, (float)contDist / 4),
                                                    (contour[this.contour_Center[i].Item2][j].Y + farPt.Y) / 2 + UnityEngine.Random.Range(-(float)contDist / 4, (float)contDist / 4));
                            Cv2.Line(m_buffer, contour[this.contour_Center[i].Item2][j], midPt, colorElec, 1);
                            Cv2.Line(m_buffer, midPt, farPt, colorElec, 1);
                        }
                    }


                }
            }
           
            dst += m_buffer;

            this.List_Contours_Buffer = this.List_Contours;

            //preGrayimage *= 0;
            //preGrayimage += grayimage;
            preGrayimage = grayimage.Clone();

        }



        public override string ToString()
        {
            return "Electrical";
        }
        public bool IsFirstFrame { get; private set; }





        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Electrical;
        }

    }
}