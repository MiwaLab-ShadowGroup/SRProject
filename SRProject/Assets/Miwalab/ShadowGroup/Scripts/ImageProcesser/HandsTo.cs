using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class HandsTo : AShadowImageProcesser
    {
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        // Mat dstMat = new Mat()
        //System.Random rand = new System.Random();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours_Buffer = new List<List<Point>>();
        Scalar color;
        Scalar colorLine;
        Scalar colorBack;

        List<OpenCvSharp.CPlusPlus.Point> contour_Center = new List<Point>();
        List<OpenCvSharp.CPlusPlus.Point> contour_Xmin = new List<Point>();
        List<OpenCvSharp.CPlusPlus.Point> contour_Xmax = new List<Point>();
        List<List<OpenCvSharp.CPlusPlus.Point>> polyLinePoint = new List<List<Point>>();
        List<int> thickness = new List<int>();
        List<OpenCvSharp.CPlusPlus.Point> PolyPoints = new List<Point>();

        OpenCvSharp.CPlusPlus.Point xMinBuffer;
        OpenCvSharp.CPlusPlus.Point xMaxBuffer;

        List<OpenCvSharp.CPlusPlus.Point> bezierPt = new List<Point>();
        int bezierCtl = 1;

        List<OpenCvSharp.CPlusPlus.Point> preXmin = new List<Point>();
        List<OpenCvSharp.CPlusPlus.Point> preXmax = new List<Point>();

        double xMinDist;
        double xMaxDist;
        int th;
        double moveThreshold;
        int centerCtl;



        Mat m_buffer;

        public HandsTo() : base()
        {

            (ShadowMediaUIHost.GetUI("HandsTo_con_R") as ParameterSlider).ValueChanged += HandsTo_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandsTo_con_G") as ParameterSlider).ValueChanged += HandsTo_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandsTo_con_B") as ParameterSlider).ValueChanged += HandsTo_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandsTo_bgd_R") as ParameterSlider).ValueChanged += HandsTo_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandsTo_bgd_G") as ParameterSlider).ValueChanged += HandsTo_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandsTo_bgd_B") as ParameterSlider).ValueChanged += HandsTo_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandsTo_moveTh") as ParameterSlider).ValueChanged += HandsTo_moveTh_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandsTo_centerCtl") as ParameterSlider).ValueChanged += HandsTo_moveTh_ValueChanged;

            (ShadowMediaUIHost.GetUI("HandsTo_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandsTo_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandsTo_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandsTo_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandsTo_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandsTo_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandsTo_moveTh") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandsTo_centerCtl") as ParameterSlider).ValueUpdate();
        }

        private void HandsTo_moveTh_ValueChanged(object sender, EventArgs e)
        {
            this.moveThreshold = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void HandsTo_centerCtl_ValueChanged(object sender, EventArgs e)
        {
            this.centerCtl = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void HandsTo_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.colorLine.Val1 = 50 + (double)(e as ParameterSlider.ChangedValue).Value;
            if (this.colorLine.Val1 > 255) this.colorLine.Val1 = 255;

        }
        private void HandsTo_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.colorLine.Val0 = 50 + (double)(e as ParameterSlider.ChangedValue).Value;
            if (this.colorLine.Val0 > 255) this.colorLine.Val0 = 255;

        }
        private void HandsTo_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.colorLine.Val2 = 50 + (double)(e as ParameterSlider.ChangedValue).Value;
            if (this.colorLine.Val2 > 255) this.colorLine.Val2 = 255;
        }
        private void HandsTo_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }
        private void HandsTo_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }
        private void HandsTo_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }



        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();
            this.contour_Center.Clear();
            this.contour_Xmin.Clear();
            this.contour_Xmax.Clear();
            this.PolyPoints.Clear();

            if (m_buffer == null)
            {
                m_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
            }
            m_buffer *= 0;

            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            //平滑化処理中央値　21はよくわからん笑
            Cv2.MedianBlur(grayimage, grayimage, 21);
            //dstMat = new Mat(dst.Height, dst.Width, MatType.CV_8UC4,colorBack);
            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);

            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();

            for (int i = 0; i < contour.Length; i++)
            {

                CvPoints.Clear();
                //Pointがnullを許容しないため仕方ないからゼロベクトルで
                xMinBuffer = new Point(0, 0);
                xMaxBuffer = new Point(0, 0);

                if (Cv2.ContourArea(contour[i]) > 1000)
                {
                    //重心検出処理
                    var cont = contour[i].ToArray();
                    
                    var M = Cv2.Moments(cont);
                    this.contour_Center.Add(new Point((M.M10 / M.M00), (M.M01 / M.M00)));

                    //for (int j = 0; j < contour[i].Length; j += contour[i].Length / this.sharpness + 1)
                    for (int j = 0; j < contour[i].Length; j++)
                    {
                        //絶対五回のはず 
                        CvPoints.Add(contour[i][j]);

                        //各輪郭のXminを取得
                        if (xMinBuffer == new Point(0, 0)) xMinBuffer = contour[i][j];
                        if (xMinBuffer.X > contour[i][j].X)
                        {
                            xMinBuffer = contour[i][j];
                        }
                        //else if(xMinBuffer.X == contour[i][j].X)
                        //{
                        //    if(xMinBuffer.Y < contour[i][j].Y)  xMinBuffer = contour[i][j]; ;
                        //}

                        //各輪郭のXmaxを取得
                        if (xMaxBuffer == new Point(0, 0)) xMaxBuffer = contour[i][j];
                        if (xMaxBuffer.X < contour[i][j].X)
                        {
                            xMaxBuffer = contour[i][j];
                        }
                        //else if(xMaxBuffer.X == contour[i][j].X)
                        //{
                        //    if (xMaxBuffer.Y < contour[i][j].Y) xMaxBuffer = contour[i][j];
                        //}
                    }
                    //
                    this.contour_Xmin.Add(xMinBuffer);
                    this.contour_Xmax.Add(xMaxBuffer);

                    this.List_Contours.Add(new List<Point>(CvPoints));
                }

            }

            //Xmin,Xmax,contour_Centerのソート昇順
            this.contour_Xmin.Sort(delegate (Point p1, Point p2) { return p1.X - p2.X; });
            this.contour_Xmax.Sort(delegate (Point p1, Point p2) { return p1.X - p2.X; });
            this.contour_Center.Sort(delegate (Point p1, Point p2) { return p1.X - p2.X; });

            //折れ線コネクタの描画計算
            var _contour = List_Contours.ToArray();

            Cv2.DrawContours(m_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);

            //手と手をつなぐベジェ曲線を描画
            for (int i = 0; i < this.contour_Xmin.Count - 1; i++)
            {
                ////閾値以上の移動なら線を描かない
                //xMaxDist =100* Math.Sqrt(Math.Pow(this.contour_Xmax[i].X - this.preXmax[i].X, 2) +
                //                      Math.Pow(this.contour_Xmax[i].Y - this.preXmax[i].Y, 2));
                //xMinDist =100* Math.Sqrt(Math.Pow(this.contour_Xmin[i + 1].X - this.preXmin[i + 1].X, 2) +
                //                      Math.Pow(this.contour_Xmin[i + 1].Y - this.preXmin[i + 1].Y, 2));
                //Debug.Log("distmax  " + xMaxDist);
                //Debug.Log("distmin  " + xMaxDist);

                //if (xMaxDist < moveThreshold || xMinDist < moveThreshold)
                //{
                //距離確認用
                //Debug.Log("dist  " + (this.contour_Xmin[i + 1].X - this.contour_Xmax[i].X));
                if (this.contour_Xmin[i + 1].X - this.contour_Xmax[i].X < moveThreshold)
                    {

                    //bezierCtlでコントロールポイントを設定
                    this.bezierPt.Clear();
                    this.bezierPt.Add(this.contour_Xmax[i]);
                    this.bezierPt.Add(this.contour_Xmax[i] + (this.contour_Xmax[i] - this.contour_Center[i]) * this.bezierCtl);
                    this.bezierPt.Add(this.contour_Xmin[i + 1] + (this.contour_Xmin[i + 1] - this.contour_Center[i + 1]) * this.bezierCtl);
                    this.bezierPt.Add(this.contour_Xmin[i + 1]); ;

                    //制御点に球を描画
                    //Cv2.Circle(m_buffer, bezierPt[0], 5, colorLine);
                    //Cv2.Circle(m_buffer, bezierPt[1], 5, colorLine);
                    //Cv2.Circle(m_buffer, bezierPt[2], 5, colorLine);
                    //Cv2.Circle(m_buffer, bezierPt[3], 5, colorLine);

                    Point prePt;
                    Point newPt;
                    newPt.X = this.bezierPt[0].X;
                    newPt.Y = this.bezierPt[0].Y;

                    for (double t = 0; t <= 1; t += 0.005)  //初期値t=0.005
                    {

                        prePt = newPt;

                        newPt.X = (int)(Math.Pow((1 - t), 3) * this.bezierPt[0].X + 3 * Math.Pow(1 - t, 2) * t * this.bezierPt[1].X + 3 * (1 - t) * t * t * this.bezierPt[2].X + t * t * t * this.bezierPt[3].X);
                        newPt.Y = (int)(Math.Pow((1 - t), 3) * this.bezierPt[0].Y + 3 * Math.Pow(1 - t, 2) * t * this.bezierPt[1].Y + 3 * (1 - t) * t * t * this.bezierPt[2].Y + t * t * t * this.bezierPt[3].Y);

                        //int th = (int)Math.Abs(0.5 - t)*15 + 10;
                        //二次関数的に真ん中が細くなる
                        th = (int)(4 * 5 * (t * t - t + 0.25)) + 3;

                        //繋ぐラインが点々になる
                        //if (UnityEngine.Random.value > 0.8f)
                        //{
                        //    Cv2.Line(m_buffer, prePt, newPt, color, th);
                        //}
                        Cv2.Line(m_buffer, prePt, newPt, color, th);
                    }

                    //直線描画
                    //Cv2.Line(m_buffer, this.contour_Xmax[i], this.contour_Xmin[i + 1], colorLine, 10);
                    // Cv2.Line(grayimage, this.contour_Xmax[i], this.contour_Xmin[i + 1], colorLine, 10);
                }
            }

            dst += m_buffer;

            this.preXmin = this.contour_Xmin;
            this.preXmax = this.contour_Xmax;

            this.List_Contours_Buffer = this.List_Contours;

        }

        public override string ToString()
        {
            return "HandsTo";
        }
        public bool IsFirstFrame { get; private set; }





        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.HandsTo;
        }

    }
}
