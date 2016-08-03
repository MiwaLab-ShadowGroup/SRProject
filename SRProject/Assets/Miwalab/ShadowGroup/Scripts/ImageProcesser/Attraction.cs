using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class Attraction : AShadowImageProcesser
    {
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 6;
        float ctl;
        double deleteTh;
        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        System.Random rand = new System.Random();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours_Buffer = new List<List<Point>>();
        Scalar color;
        Scalar colorLine;
        Scalar colorBack;

        List<OpenCvSharp.CPlusPlus.Point> contour_Center = new List<Point>();
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

        Mat m_buffer;

        public Attraction() : base()
        {

            (ShadowMediaUIHost.GetUI("Attraction_con_R") as ParameterSlider).ValueChanged += Attraction_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Attraction_con_G") as ParameterSlider).ValueChanged += Attraction_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Attraction_con_B") as ParameterSlider).ValueChanged += Attraction_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Attraction_bgd_R") as ParameterSlider).ValueChanged += Attraction_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Attraction_bgd_G") as ParameterSlider).ValueChanged += Attraction_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Attraction_bgd_B") as ParameterSlider).ValueChanged += Attraction_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Attraction_ctl") as ParameterSlider).ValueChanged += Attraction_ctl_ValueChanged;
            (ShadowMediaUIHost.GetUI("Attraction_deleteTh") as ParameterSlider).ValueChanged += Attraction_deleteTh_ValueChanged;
            (ShadowMediaUIHost.GetUI("Attraction_Rate") as ParameterSlider).ValueChanged += Attraction_Rate_ValueChanged;

            (ShadowMediaUIHost.GetUI("Attraction_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Attraction_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Attraction_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Attraction_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Attraction_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Attraction_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Attraction_ctl") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Attraction_deleteTh") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Attraction_Rate") as ParameterSlider).ValueUpdate();
        }

        private void Attraction_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.sharpness = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void Attraction_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.colorLine.Val1 = 50 + (double)(e as ParameterSlider.ChangedValue).Value;
            if (this.colorLine.Val1 > 255) this.colorLine.Val1 = 255;

        }
        private void Attraction_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.colorLine.Val0 = 50 + (double)(e as ParameterSlider.ChangedValue).Value;
            if (this.colorLine.Val0 > 255) this.colorLine.Val0 = 255;

        }
        private void Attraction_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.colorLine.Val2 = 50 + (double)(e as ParameterSlider.ChangedValue).Value;
            if (this.colorLine.Val2 > 255) this.colorLine.Val2 = 255;
        }
        private void Attraction_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }
        private void Attraction_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }
        private void Attraction_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Attraction_ctl_ValueChanged(object sender, EventArgs e)
        {
            this.ctl = (float)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Attraction_deleteTh_ValueChanged(object sender, EventArgs e)
        {
            this.deleteTh = (float)(e as ParameterSlider.ChangedValue).Value;

        }



        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();
            this.contour_Center.Clear();
            this.contour_Ymin.Clear();
            this.contour_Ymax.Clear();
            this.PolyPoints.Clear();
            this.polyLinePoint.Clear();
            this.DeletePoints.Clear();
            this.deleteLinePoint.Clear();
            this.deleteNum.Clear();
            this.MinPolyPoints.Clear();
            this.DistanceList.Clear();


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
                yMinBuffer = new Point(0, 0);
                yMaxBuffer = new Point(0, 0);

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

                        //各輪郭のYminを取得
                        if (yMinBuffer == new Point(0, 0)) yMinBuffer = contour[i][j];
                        if (yMinBuffer.Y > contour[i][j].Y)
                        {
                            yMinBuffer = contour[i][j];
                        }

                        //各輪郭のXmaxを取得
                        if (yMaxBuffer == new Point(0, 0)) yMaxBuffer = contour[i][j];
                        if (yMaxBuffer.Y < contour[i][j].Y)
                        {
                            yMaxBuffer = contour[i][j];
                        }

                    }
                    //
                    this.contour_Ymin.Add(yMinBuffer);
                    this.contour_Ymax.Add(yMaxBuffer);

                    this.List_Contours.Add(new List<Point>(CvPoints));



                }

            }

            //Xmin,Xmax,contour_Centerのソート昇順
            this.contour_Ymin.Sort(delegate (Point p1, Point p2) { return p1.X - p2.X; });
            this.contour_Ymax.Sort(delegate (Point p1, Point p2) { return p1.X - p2.X; });
            this.contour_Center.Sort(delegate (Point p1, Point p2) { return p1.X - p2.X; });

            ////折れ線コネクタの描画計算
            //var _contour = List_Contours.ToArray();

            //Cv2.DrawContours(m_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);

            //体と体をつなぐベジェ曲線を描画
            for (int i = 0; i < this.contour_Ymin.Count - 1; i++)
            {
                this.PolyPoints.Clear();
                this.DeletePoints.Clear();
                this.deleteNum.Clear();
                this.MinPolyPoints.Clear();
                this.DistanceList.Clear();

                //bezierCtlの設定
                int distance = (this.contour_Center[i + 1].X - this.contour_Center[i].X);

                this.bezierPt.Clear();
                this.bezierPt.Add(this.contour_Ymax[i]);
                this.bezierPt.Add(new Point((this.contour_Center[i].X + this.contour_Center[i + 1].X) / 2,
                                            (this.contour_Center[i].Y + this.contour_Center[i + 1].Y) / 2 - distance * ctl));
                this.bezierPt.Add(this.contour_Ymax[i + 1]); ;

                this.bezierPt.Add(this.contour_Ymin[i + 1]);
                this.bezierPt.Add(new Point((this.contour_Center[i].X + this.contour_Center[i + 1].X) / 2,
                                            (this.contour_Center[i].Y + this.contour_Center[i + 1].Y) / 2 + distance * ctl));
                this.bezierPt.Add(this.contour_Ymin[i]); ;

                Point newPt;


                //Ymaxの曲線描画　左上→右上
                newPt.X = this.bezierPt[0].X;
                newPt.Y = this.bezierPt[0].Y;

                for (double t = 0; t <= 1; t += 0.01)  //初期値t=0.005　両方変えるのを忘れずに
                {

                    newPt.X = (int)(Math.Pow(1 - t, 2) * this.bezierPt[0].X + 2 * (1 - t) * t * this.bezierPt[1].X + t * t * this.bezierPt[2].X);
                    newPt.Y = (int)(Math.Pow(1 - t, 2) * this.bezierPt[0].Y + 2 * (1 - t) * t * this.bezierPt[1].Y + t * t * this.bezierPt[2].Y);
                    PolyPoints.Add(newPt);

                }

                //Yminの曲線描画 右下→左下
                newPt.X = this.bezierPt[3].X;
                newPt.Y = this.bezierPt[3].Y;

                for (double t = 0; t <= 1; t += 0.01)  //初期値t=0.005 両方変えるのを忘れずに
                {

                    newPt.X = (int)(Math.Pow(1 - t, 2) * this.bezierPt[3].X + 2 * (1 - t) * t * this.bezierPt[4].X + t * t * this.bezierPt[5].X);
                    newPt.Y = (int)(Math.Pow(1 - t, 2) * this.bezierPt[3].Y + 2 * (1 - t) * t * this.bezierPt[4].Y + t * t * this.bezierPt[5].Y);

                    PolyPoints.Add(newPt);
                    MinPolyPoints.Add(newPt);

                }
                //交点のチェック
                MinPolyPoints.Reverse();
                deleteCheck = false;
                //Min > Max になる点を検出
                for (int j = 0; j <= PolyPoints.Count/2 -1; j++)
                {

                    if (PolyPoints[PolyPoints.Count - 1 - j].Y > PolyPoints[j].Y)  //min > max
                    {
                        this.DeletePoints.Add(PolyPoints[PolyPoints.Count - 1 - j]);
                        this.DeletePoints.Insert(0, PolyPoints[j]);
                        //消失確認用の丸描画
                        //Cv2.Circle(m_buffer, this.PolyPoints[PolyPoints.Count - 1 - j], 5, colorLine);
                        //Cv2.Circle(m_buffer, this.PolyPoints[j], 5, colorLine);

                        deleteCheck = true;
                    }

                }

                //リストinリストにin
                this.PolyPoints.Insert(this.PolyPoints.Count/2, this.contour_Center[i + 1]);
                this.PolyPoints.Insert(0, this.contour_Center[i]);   //なんかこれをいれるとうまくいかない

                this.polyLinePoint.Add(new List<Point>(PolyPoints));
                if(deleteCheck) this.deleteLinePoint.Add(new List<Point>(DeletePoints));
            }

            var _contour = List_Contours.ToArray();
            var _poly = polyLinePoint.ToArray();

            //Cv2.FillPoly(m_buffer, _poly, color, OpenCvSharp.LineType.Link8);
            // Cv2.FillPoly(m_buffer, _deletePoly, colorBack, OpenCvSharp.LineType.Link8);
            Cv2.DrawContours(m_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);
            Cv2.DrawContours(m_buffer, _poly, -1, color, -1, OpenCvSharp.LineType.Link8);
         
            if (deleteLinePoint != null)
            {
                var _delete_poly = deleteLinePoint.ToArray();
                Cv2.DrawContours(m_buffer, _delete_poly, -1, colorBack, -1, OpenCvSharp.LineType.Link8);
            }
            
            dst += m_buffer;

            this.List_Contours_Buffer = this.List_Contours;


        }



        public override string ToString()
        {
            return "Attraction";
        }
        public bool IsFirstFrame { get; private set; }





        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Attraction;
        }

    }
}