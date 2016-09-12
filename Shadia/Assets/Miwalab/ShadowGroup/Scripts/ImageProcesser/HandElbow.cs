using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using UnityEngine;
using Kinect = Windows.Kinect;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class HandElbow : AShadowImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 6;
        double ctlRate = 0;
        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        // Mat dstMat = new Mat()
        List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours_Buffer = new List<List<Point>>();
        Scalar color;
        Scalar colorBack;


        List<Point> contour_Center = new List<Point>();
        //List<Point> leftElbow = new List<Point>();
        //List<Point> leftHand = new List<Point>();
        //List<Point> leftShoulder = new List<Point>();
        //List<Vec2f> elbowToHand = new List<Vec2f>();
        //List<Vec2f> betweenElbow = new List<Vec2f>();
        //List<Vec2f> elbowToShoulder = new List<Vec2f>();
        //List<int> TrackedBodyNum = new List<int>();

        //List<Vec2f> elbowBisector = new List<Vec2f>();
        bool inCircle;
        //Point? nearestContour;
        //int nearestNum;
        int contNumBuf = 0;
        //List<Point> circlePoint = new List<Point>();
        //List<Point> nearElbow = new List<Point>();
        List<BodyContor> bodyConts = new List<BodyContor>();
        //List<Windows.Kinect.JointType> keyList = new List<Windows.Kinect.JointType>();
        float toBoneLength;
        float toBoneMin;
        List<Windows.Kinect.JointType> nearList = new List<Windows.Kinect.JointType>();
        Windows.Kinect.JointType nearBone;

        List<List<BodyContor>> bodyContList = new List<List<BodyContor>>();
        double rotShoulder;

        //Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> _BoneMap;
        //Dictionary<int, Windows.Kinect.JointType> _BoneKeyTable;
        Mat preMat = Mat.Eye(3, 3, MatType.CV_8SC1);
        //Mat scaleMat;
        //Mat rotMat;
        //Mat transMat;  //scale -> rot-> trans?
        //Mat transGo;
        //Mat transBack;
        //int[,,] scaleM;
        List<List<BoneMat>> List_BoneMat = new List<List<BoneMat>>();
        Dictionary<Kinect.JointType, Point> BoneDictionary = new Dictionary<Kinect.JointType, Point>();
        List<Dictionary<Kinect.JointType, Point>> List_BoneDictionary = new List<Dictionary<Kinect.JointType, Point>>();
        List<Dictionary<Kinect.JointType, Point>> List_preBoneDictionary = new List<Dictionary<Kinect.JointType, Point>>();

        Dictionary<Kinect.JointType, Point> MoveMatDictionary = new Dictionary<Kinect.JointType, Point>();
        List<Dictionary<Kinect.JointType, Point>> List_moveMat = new List<Dictionary<Kinect.JointType, Point>>();

        DepthBody[] preBodyData ;




        public HandElbow() : base()
        {
            (ShadowMediaUIHost.GetUI("HandElbow_con_R") as ParameterSlider).ValueChanged += HandElbow_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_con_G") as ParameterSlider).ValueChanged += HandElbow_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_con_B") as ParameterSlider).ValueChanged += HandElbow_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_rot_S") as ParameterSlider).ValueChanged += HandElbow_rot_S_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_bgd_G") as ParameterSlider).ValueChanged += HandElbow_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_bgd_B") as ParameterSlider).ValueChanged += HandElbow_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_Rate") as ParameterSlider).ValueChanged += HandElbow_Rate_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_CtlRate") as ParameterSlider).ValueChanged += HandElbow_CtlRate_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_UseFade") as ParameterCheckbox).ValueChanged += HandElbow_UseFade_ValueChanged;

            (ShadowMediaUIHost.GetUI("HandElbow_CC_Blue") as ParameterButton).Clicked += HandElbow_CC_Blue_Clicked;
            (ShadowMediaUIHost.GetUI("HandElbow_CC_Orange") as ParameterButton).Clicked += HandElbow_CC_Orange_Clicked;
            (ShadowMediaUIHost.GetUI("HandElbow_CC_Yellow") as ParameterButton).Clicked += HandElbow_CC_Yellow_Clicked;
            (ShadowMediaUIHost.GetUI("HandElbow_CC_Pink") as ParameterButton).Clicked += HandElbow_CC_Pink_Clicked;
            (ShadowMediaUIHost.GetUI("HandElbow_CC_Green") as ParameterButton).Clicked += HandElbow_CC_Green_Clicked;


            (ShadowMediaUIHost.GetUI("HandElbow_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_rot_S") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_Rate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_CtlRate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_UseFade") as ParameterCheckbox).ValueUpdate();
        }

        private void HandElbow_CC_Green_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("HandElbow_con_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("HandElbow_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("HandElbow_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void HandElbow_CC_Pink_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("HandElbow_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("HandElbow_con_G") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("HandElbow_con_B") as ParameterSlider).m_slider.value = 255;
        }

        private void HandElbow_CC_Yellow_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("HandElbow_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("HandElbow_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("HandElbow_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void HandElbow_CC_Orange_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("HandElbow_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("HandElbow_con_G") as ParameterSlider).m_slider.value = 125;
            (ShadowMediaUIHost.GetUI("HandElbow_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void HandElbow_CC_Blue_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("HandElbow_con_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("HandElbow_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("HandElbow_con_B") as ParameterSlider).m_slider.value = 255;
        }

        private void HandElbow_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void HandElbow_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.sharpness = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void HandElbow_CtlRate_ValueChanged(object sender, EventArgs e)
        {
            this.ctlRate = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void HandElbow_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void HandElbow_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void HandElbow_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void HandElbow_rot_S_ValueChanged(object sender, EventArgs e)
        {
            this.rotShoulder = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void HandElbow_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void HandElbow_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        Mat m_buffer;
        bool m_UseFade;
        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();
            //this.elbowToHand.Clear();
            //this.elbowToShoulder.Clear();
            //this.betweenElbow.Clear();
            //this.TrackedBodyNum.Clear();
            //this.circlePoint.Clear();
            //this.elbowBisector.Clear();
            //this.nearElbow.Clear();
            this.contour_Center.Clear();
            this.bodyContList.Clear();

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

            if (preBodyData == null)
            {
                preBodyData = BodyDataOnDepthImage;
            }

            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            Cv2.MedianBlur(grayimage, grayimage, 21);
            //dstMat = new Mat(dst.Height, dst.Width, MatType.CV_8UC4,colorBack);
            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);


            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);



            for (int i = 0; i < contour.Length; i++)
            {

                CvPoints.Clear();
                if (Cv2.ContourArea(contour[i]) > 1000)
                {

                    var cont = contour[i].ToArray();
                    var M = Cv2.Moments(cont);
                    this.contour_Center.Add(new Point((M.M10 / M.M00), (M.M01 / M.M00)));

                    //for (int j = 0; j < contour[i].Length; j += contour[i].Length / this.sharpness + 1)
                    for (int j = 0; j < contour[i].Length; j += contour[i].Length / this.sharpness + 1)
                    {

                        //絶対五回のはず
                        CvPoints.Add(contour[i][j]);
                    }

                    this.List_Contours.Add(new List<Point>(CvPoints));

                }

            }


            //輪郭の描画
            //var _contour = List_Contours.ToArray();
            //Cv2.DrawContours(m_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);



            //輪郭点の分類を開始
            contNumBuf = 0;

            for (int i = 0; i < BodyData.Length; i++)
            {
                bodyConts.Clear();
                BoneDictionary.Clear();
                if (BodyData[i].IsTracked)
                {
                    //Boneの位置情報リスト作成
                    for (int j = 0; j < _TrackigBoneList.Count; j++)
                    {

                        BoneDictionary.Add(_TrackigBoneList[j], JointToDepthPoint(i, _TrackigBoneList[j]));
                    }
                    this.List_BoneDictionary.Add(new Dictionary<Windows.Kinect.JointType, Point>(BoneDictionary));




                    //トラッキングされているボーンにつき最も近い輪郭点群を探し番号を取得
                    for (int j = 0; j < this.contour_Center.Count; j++)
                    {
                        if (PointsLength(JointToDepthPoint(i, Windows.Kinect.JointType.SpineBase), this.contour_Center[contNumBuf]) >
                            PointsLength(JointToDepthPoint(i, Windows.Kinect.JointType.SpineBase), this.contour_Center[j]))
                        {

                            contNumBuf = j;
                        }
                    }


                    //見つけた輪郭点群に対して、その中の点が最も近いボーンを探す

                    //Debug.Log("centerNum  " + this.contour_Center.Count);
                    //Debug.Log("contourNum  " + this.List_Contours.Count);
                    for (int k = 0; k < this.List_Contours[contNumBuf].Count; k++)
                    {

                        toBoneMin = 5000;
                        for (int m = 0; m < _BoneKeyTable.Count; m++)
                        {
                            toBoneLength = PointsLength(JointToDepthPoint(i, this._BoneMap[this._BoneKeyTable[m]]) - this.List_Contours[contNumBuf][k] + JointToDepthPoint(i, this._BoneKeyTable[m]) - this.List_Contours[contNumBuf][k]);

                            if (toBoneMin > toBoneLength)
                            {
                                toBoneMin = toBoneLength;
                                nearBone = _BoneKeyTable[m];
                            }

                        }
                        //Cv2.Circle(m_buffer, this.List_Contours[contNumBuf][k], 5, new Scalar(0, 255, 255), 2);
                        this.bodyConts.Add(new BodyContor(this.List_Contours[contNumBuf][k], contNumBuf, i, nearBone));
                    }

                    this.bodyContList.Add(new List<BodyContor>(bodyConts));
                    //Cv2.Line(m_buffer, JointToDepthPoint(i, Windows.Kinect.JointType.SpineBase), this.contour_Center[contNumBuf], new Scalar(255, 200, 100));

                }
            }

            //ボーンごとの変換行列を作成    拡大＝一度原点に戻すしかない？と仮定
            for (int i = 0; i < this.List_preBoneDictionary.Count; i++)
            {
                Mat preMat;


                float[,] transMatArr = {{ 1,0, -JointToDepthPoint(i, Kinect.JointType.SpineBase).X -   this.List_preBoneDictionary[i][Kinect.JointType.SpineBase].X },
                                        { 0,1, -JointToDepthPoint(i, Kinect.JointType.SpineBase).Y -   this.List_preBoneDictionary[i][Kinect.JointType.SpineBase].Y },
                                        { 0,0,1 }};
                Mat transMat = new Mat(3, 3, MatType.CV_32FC1, transMatArr);

                float scaleRate = PointsLength(JointToDepthPoint(i, Kinect.JointType.SpineBase), JointToDepthPoint(i, Kinect.JointType.SpineMid)) /
                                  PointsLength(this.List_preBoneDictionary[i][Kinect.JointType.SpineBase], this.List_preBoneDictionary[i][Kinect.JointType.SpineMid]);
                float[,] scaleMatArr = { { scaleRate  ,0, 0   },
                                         { 0,scaleRate, 0 },
                                         { 0,0,1 } };
                Mat scaleMat = new Mat(3, 3, MatType.CV_32FC1, scaleMatArr);


                float[,] goOriMatArr = { {1,0, -this.List_preBoneDictionary[i][Kinect.JointType.SpineBase].X },
                                         {0,1, -this.List_preBoneDictionary[i][Kinect.JointType.SpineBase].Y },
                                         {0,0,1 } };
                Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

                float[,] backOriMatArr = { {1,0,this.List_preBoneDictionary[i][Kinect.JointType.SpineBase].X  },
                                           {0,1,this.List_preBoneDictionary[i][Kinect.JointType.SpineBase].Y  },
                                           {0,0,1 } };
                Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);

                //内積から角度を計算
                double boneRad = Math.Acos( (double) ( (JointToDepthPoint(i, Kinect.JointType.SpineMid).X - JointToDepthPoint(i,Kinect.JointType.SpineBase).X) * (this.List_preBoneDictionary[i][Kinect.JointType.SpineMid].X - this.List_preBoneDictionary[i][Kinect.JointType.SpineBase].X) +
                                                       (JointToDepthPoint(i, Kinect.JointType.SpineMid).Y - JointToDepthPoint(i,Kinect.JointType.SpineBase).Y) * (this.List_preBoneDictionary[i][Kinect.JointType.SpineMid].Y - this.List_preBoneDictionary[i][Kinect.JointType.SpineBase].Y)  )/
                                                       PointsLength(JointToDepthPoint(i, Kinect.JointType.SpineMid), JointToDepthPoint(i, Kinect.JointType.SpineBase)) * PointsLength(this.List_preBoneDictionary[i][Kinect.JointType.SpineMid], this.List_preBoneDictionary[i][Kinect.JointType.SpineBase]));

                //det = ax * by - ay * bx = - or +  どっち向きかの判断
                if ((JointToDepthPoint(i, Kinect.JointType.SpineMid).X - JointToDepthPoint(i,Kinect.JointType.SpineBase).X) * (this.List_preBoneDictionary[i][Kinect.JointType.SpineMid].X - this.List_preBoneDictionary[i][Kinect.JointType.SpineBase].X) -
                    (JointToDepthPoint(i, Kinect.JointType.SpineMid).Y - JointToDepthPoint(i,Kinect.JointType.SpineBase).Y) * (this.List_preBoneDictionary[i][Kinect.JointType.SpineMid].Y - this.List_preBoneDictionary[i][Kinect.JointType.SpineBase].Y) < 0)
                {
                    boneRad *= -1;
                }

                float[,] rotMatArr = { { (float)Math.Cos(boneRad), (float)-Math.Sin(boneRad),0 },
                                       { (float)Math.Sin(boneRad), (float)Math.Cos(boneRad),0  },
                                       {0,0,1 } };
                Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);

                //  順番 <----  　　完全に一致　<- 平行移動　<- 元の場所へもどす　<- 回転＋拡大縮小　<- 原点へ移動
                preMat = transMat * backOriMat * rotMat * scaleMat * goOriMat;

            }




            //行列変換
            //scaleMat = eyeMat;

            //rotMat = eyeMat;
            //rotMat.Set<float>(0, 0, (float)Math.Cos(ctlRate));
            //rotMat.Set<float>(0, 0, (float)-Math.Sin(ctlRate));
            //rotMat.Set<float>(1, 0, (float)Math.Cos(ctlRate));
            //rotMat.Set<float>(1, 1, (float)Math.Cos(ctlRate));

            //transMat = eyeMat;

            //transGo = eyeMat;
            //transGo.Set<float>(0,2,JointToDepthPoint(this.bodyContList[0][0].bodyNum),)

            for (int i = 0; i < this.contour_Center.Count; i++)
            {
                Cv2.Circle(m_buffer, contour_Center[i], 3, new Scalar(255, 255, 0), 2);
            }


            //取得した輪郭点について1つずつ処理
            this.List_Contours.Clear();

            for (int i = 0; i < bodyContList.Count; i++)
            {
                Cv2.Line(m_buffer, JointToDepthPoint(this.bodyContList[i][0].bodyNum, Windows.Kinect.JointType.SpineBase), this.contour_Center[this.bodyContList[i][0].contourNum], new Scalar(255, 200, 100));

                this.CvPoints.Clear();

                preMat = GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.ElbowLeft, rotShoulder);
                for (int j = 0; j < this.bodyContList[i].Count; j++)  //順番
                {
                    //ひじと手を動かす
                    if (bodyContList[i][j].nearJt1 == Windows.Kinect.JointType.ElbowLeft)
                    {
                        bodyContList[i][j].center = ContourRotate(bodyContList[i][j], rotShoulder);
                        //Cv2.Circle(m_buffer, ContourRotate(bodyContList[i][j], ctlRate), 5, new Scalar(255, 255, 0), 2);

                    }
                    if (bodyContList[i][j].nearJt1 == Windows.Kinect.JointType.WristLeft)
                    {
                        bodyContList[i][j].center = ContourRotate(bodyContList[i][j], ctlRate, preMat);
                        //Cv2.Circle(m_buffer, ContourRotate(bodyContList[i][j], ctlRate), 5, new Scalar(255, 255, 0), 2);
                    }


                    //各ボーンの動きを速める
                    if (List_preBoneDictionary[i] != null)
                    {
                        bodyContList[i][j].center += bodyContList[i][j].center - List_preBoneDictionary[i][bodyContList[i][j].nearJt1];


                    }








                    this.CvPoints.Add(bodyContList[i][j].center);
                }

                this.List_Contours.Add(new List<Point>(CvPoints));
            }


            var _contour = List_Contours.ToArray();
            Cv2.DrawContours(m_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);


            Cv2.CvtColor(m_buffer, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            Cv2.DrawContours(m_buffer, contour, -1, color, -1, OpenCvSharp.LineType.Link8);


            dst += m_buffer;
            this.List_Contours_Buffer = this.List_Contours;
            this.List_preBoneDictionary = this.List_BoneDictionary;
            preBodyData = BodyDataOnDepthImage;

            //Cv2.CvtColor(dstMat, dst, OpenCvSharp.ColorConversion.BgraToBgr);

        }


        //全部で10本の骨として考える　handではなくwrist, footではなくankleを使用 spineBaseがkey  /Headがkey
        public Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> _BoneMap = new Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType>()
        {
            { Windows.Kinect.JointType.AnkleLeft, Windows.Kinect.JointType.KneeLeft },
            { Windows.Kinect.JointType.KneeLeft,  Windows.Kinect.JointType.SpineBase},

            { Windows.Kinect.JointType.AnkleRight,Windows.Kinect.JointType.KneeRight },
            { Windows.Kinect.JointType.KneeRight, Windows.Kinect.JointType.SpineBase },

            { Windows.Kinect.JointType.WristLeft, Windows.Kinect.JointType.ElbowLeft },
            { Windows.Kinect.JointType.ElbowLeft, Windows.Kinect.JointType.ShoulderLeft },

            { Windows.Kinect.JointType.WristRight, Windows.Kinect.JointType.ElbowRight },
            { Windows.Kinect.JointType.ElbowRight, Windows.Kinect.JointType.ShoulderRight },
            { Windows.Kinect.JointType.SpineBase, Windows.Kinect.JointType.SpineMid },
            { Windows.Kinect.JointType.Head, Windows.Kinect.JointType.Neck },
        };

        //全部で10本の骨として考える　handではなくwrist, footではなくankleを使用 spineBaseがkey  /Headがkey
        public Dictionary<int, Windows.Kinect.JointType> _BoneKeyTable = new Dictionary<int, Windows.Kinect.JointType>()
        {
            { 0,Windows.Kinect.JointType.AnkleLeft },
            { 1,Windows.Kinect.JointType.KneeLeft},

            { 2,Windows.Kinect.JointType.AnkleRight },
            { 3,Windows.Kinect.JointType.KneeRight  },

            { 4,Windows.Kinect.JointType.WristLeft   },
            { 5,Windows.Kinect.JointType.ElbowLeft    },

            { 6,Windows.Kinect.JointType.WristRight },
            { 7,Windows.Kinect.JointType.ElbowRight },
            { 8,Windows.Kinect.JointType.SpineBase},
            { 9,Windows.Kinect.JointType.Head},
        };

        //posを計測するボーンのリスト
        public Dictionary<int, Windows.Kinect.JointType> _TrackigBoneList = new Dictionary<int, Windows.Kinect.JointType>()
        {
            { 0,Windows.Kinect.JointType.AnkleLeft },
            { 1,Windows.Kinect.JointType.KneeLeft},

            { 2,Windows.Kinect.JointType.AnkleRight },
            { 3,Windows.Kinect.JointType.KneeRight  },

            { 4,Windows.Kinect.JointType.WristLeft   },
            { 5,Windows.Kinect.JointType.ElbowLeft    },

            { 6,Windows.Kinect.JointType.WristRight },
            { 7,Windows.Kinect.JointType.ElbowRight },
            { 8,Windows.Kinect.JointType.SpineBase},
            { 9,Windows.Kinect.JointType.Head},
            { 10,Windows.Kinect.JointType.ShoulderLeft},
            { 11,Windows.Kinect.JointType.ShoulderRight},
            { 12,Windows.Kinect.JointType.SpineMid},
            { 13,Windows.Kinect.JointType.Neck},
        };


        private float VecLength(Vec2f vec)
        {
            return (float)Math.Sqrt(Math.Pow(vec.Item0, 2) + (Math.Pow(vec.Item1, 2)));
        }

        private float PointsLength(Point pt1, Point pt2)
        {
            return (float)Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + (Math.Pow(pt1.Y - pt2.Y, 2)));
        }
        private float PointsLength(Point pt1)
        {
            return (float)Math.Sqrt(Math.Pow(pt1.X, 2) + (Math.Pow(pt1.Y, 2)));
        }

        private bool InCircleCheck(Point checkPt, Vec2f bone, Vec2f boneCtl, float r)
        {
            bool bl = false;
            if (Math.Sqrt(Math.Pow((checkPt.X - (bone.Item0 + boneCtl.Item0) / 2), 2 + Math.Pow(checkPt.Y - (bone.Item1 + boneCtl.Item1) / 2, 2))) < r)
            {
                bl = true;
            }

            return bl;
        }

        private float LineToPointLength(Vec2f bone, Vec2f bisector, Point pt)
        {

            return (bisector.Item0 * (pt.Y - bone.Item1) - bisector.Item1 * (pt.X - bone.Item0)) / VecLength(bisector);
        }

        private Point JointToDepthPoint(int bodyNum, Windows.Kinect.JointType jt)
        {
            return new Point(BodyDataOnDepthImage[bodyNum].JointDepth[jt].position.X, BodyDataOnDepthImage[bodyNum].JointDepth[jt].position.Y);
        }

        private Point ContourRotate(BodyContor bc, double rad)
        {

            float[,] contMatArr = { {bc.center.X },
                                    {bc.center.Y },
                                    {1 } };
            Mat contMat = new Mat(3, 1, MatType.CV_32FC1, contMatArr);

            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bc.bodyNum,_BoneMap[ bc.nearJt1] ).X },
                                     {0,1, -JointToDepthPoint(bc.bodyNum, _BoneMap[ bc.nearJt1]).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1] ).X },
                                       {0,1, JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1]  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);

            //  あと　　←　　先　:　元　　（行列の順番）
            contMat = backOriMat * rotMat * goOriMat * contMat;



            return new Point(contMat.At<float>(0, 0), contMat.At<float>(0, 1));
        }

        private Point ContourRotate(BodyContor bc, double rad, Mat preMat3x3)
        {

            float[,] contMatArr = { {bc.center.X },
                                    {bc.center.Y },
                                    {1 } };
            Mat contMat = new Mat(3, 1, MatType.CV_32FC1, contMatArr);

            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bc.bodyNum,_BoneMap[ bc.nearJt1] ).X },
                                     {0,1, -JointToDepthPoint(bc.bodyNum, _BoneMap[ bc.nearJt1]).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1] ).X },
                                       {0,1, JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1]  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);

            //  あと　　←　　先　:　元　　（行列の順番）
            contMat = preMat3x3 * backOriMat * rotMat * goOriMat * contMat;

            return new Point(contMat.At<float>(0, 0), contMat.At<float>(0, 1));
        }

        private Mat GetRotateMat(BodyContor bc, double rad, Mat preMat3x3)
        {

            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bc.bodyNum,_BoneMap[ bc.nearJt1] ).X },
                                     {0,1, -JointToDepthPoint(bc.bodyNum, _BoneMap[ bc.nearJt1]).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1] ).X },
                                       {0,1, JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1]  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);


            return preMat3x3 * backOriMat * rotMat * goOriMat;
        }

        private Mat GetRotateMat(BodyContor bc, double rad)
        {
            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bc.bodyNum,_BoneMap[ bc.nearJt1] ).X },
                                     {0,1, -JointToDepthPoint(bc.bodyNum, _BoneMap[ bc.nearJt1]).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1] ).X },
                                       {0,1, JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1]  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);


            return backOriMat * rotMat * goOriMat;
        }
        private Mat GetRotateMat(int bodyNum, Kinect.JointType jt, double rad)
        {
            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bodyNum,_BoneMap[ jt] ).X },
                                     {0,1, -JointToDepthPoint(bodyNum, _BoneMap[ jt]).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bodyNum,  _BoneMap[ jt] ).X },
                                       {0,1, JointToDepthPoint(bodyNum,  _BoneMap[ jt]  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);


            return backOriMat * rotMat * goOriMat;
        }

        //ボーンの前情報との比較から変換行列を算出
        private Mat GetBoneMoveMat(int bodyNum, Kinect.JointType nearJt, Kinect.JointType farJt, DepthBody[] preBody)
        {
            
            float[,] transMatArr = {{ 1,0, JointToDepthPoint(bodyNum, nearJt).X - preBody[bodyNum].JointDepth[nearJt].position.X },
                                    { 0,1, JointToDepthPoint(bodyNum, nearJt).Y - preBody[bodyNum].JointDepth[nearJt].position.Y },
                                    { 0,0,1 }};
            Mat transMat = new Mat(3, 3, MatType.CV_32FC1, transMatArr);

            float scaleRate = PointsLength(JointToDepthPoint(bodyNum, nearJt), JointToDepthPoint(bodyNum, farJt)) /
                              PointsLength(new Point(preBody[bodyNum].JointDepth[nearJt].position.X - preBody[bodyNum].JointDepth[farJt].position.X, preBody[bodyNum].JointDepth[nearJt].position.Y - preBody[bodyNum].JointDepth[farJt].position.Y) );
            float[,] scaleMatArr = { { scaleRate, 0, 0   },
                                     { 0,scaleRate, 0 },
                                     { 0, 0, 1 } };
            Mat scaleMat = new Mat(3, 3, MatType.CV_32FC1, scaleMatArr);


            float[,] goOriMatArr = { {1,0, - preBody[bodyNum].JointDepth[nearJt].position.X },
                                     {0,1, - preBody[bodyNum].JointDepth[nearJt].position.Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1, 0, preBody[bodyNum].JointDepth[nearJt].position.X  },
                                       {0, 1, preBody[bodyNum].JointDepth[nearJt].position.Y  },
                                       {0, 0, 1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);

            //内積から角度を計算
            double boneRad = Math.Acos((double)((JointToDepthPoint(bodyNum, farJt).X - JointToDepthPoint(bodyNum,nearJt).X) * (preBody[bodyNum].JointDepth[farJt].position.X - preBody[bodyNum].JointDepth[nearJt].position.X) +
                                                (JointToDepthPoint(bodyNum, farJt).Y - JointToDepthPoint(bodyNum,nearJt).Y) * (preBody[bodyNum].JointDepth[farJt].position.Y - preBody[bodyNum].JointDepth[nearJt].position.Y)) /
                                                 PointsLength(JointToDepthPoint(bodyNum, farJt), JointToDepthPoint(bodyNum, nearJt)) * PointsLength(new Point(preBody[bodyNum].JointDepth[farJt].position.X - preBody[bodyNum].JointDepth[nearJt].position.X, preBody[bodyNum].JointDepth[farJt].position.Y - preBody[bodyNum].JointDepth[nearJt].position.Y)));

            //det = ax * by - ay * bx = - or +  どっち向きかの判断
            if ((JointToDepthPoint(bodyNum, farJt).X - JointToDepthPoint(bodyNum, nearJt).X) * (preBody[bodyNum].JointDepth[farJt].position.Y - preBody[bodyNum].JointDepth[nearJt].position.Y) -
                (JointToDepthPoint(bodyNum, farJt).Y - JointToDepthPoint(bodyNum, nearJt).Y) * (preBody[bodyNum].JointDepth[farJt].position.X - preBody[bodyNum].JointDepth[nearJt].position.X) < 0)
            {
                boneRad *= -1;   //ちょっとここはやってみてから判断笑
            }

            float[,] rotMatArr = { { (float)Math.Cos(boneRad), (float)-Math.Sin(boneRad),0 },
                                   { (float)Math.Sin(boneRad), (float)Math.Cos(boneRad),0  },
                                   {0, 0, 1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);

            //  順番 <----  　　完全に一致!　<- 平行移動　<- 元の場所へもどす　<- 回転＋拡大縮小　<- 原点へ移動
            return transMat * backOriMat * rotMat * scaleMat * goOriMat;
            
        }




/*
private Windows.Kinect.JointType FindNearJoint1(Point pt , int bodyNum)
{
    for(     

    p1madenokyori = pt
         this.List_Contours[j][k] - JointToDepthPoint(i, _BoneMap[keyList[m]]) + this.List_Contours[j][k] - JointToDepthPoint(i, keyList[m]);
                        if ()



            return jt;
}
*/


        public class BodyContor   //すべての輪郭点のクラス
        {

            public OpenCvSharp.CPlusPlus.Point center { get; set; }
            public int contourNum { get; set; }
            public int bodyNum { get; set; }
            public Windows.Kinect.JointType nearJt1 { get; set; }
            //Windows.Kinect.JointType nearJt2 { get; set; }

            /*
            public BodyContor(OpenCvSharp.CPlusPlus.Point pt, int contNum, int bdNum, Windows.Kinect.JointType jt1, Windows.Kinect.JointType jt2)
            {
                this.center = pt;
                this.contourNum = contNum;
                this.bodyNum = bdNum;
                this.nearJt1 = jt1;
                this.nearJt2 = jt2;
            }
            */
            public BodyContor(OpenCvSharp.CPlusPlus.Point pt, int contNum, int bdNum, Windows.Kinect.JointType jt1)
            {
                this.center = pt;
                this.contourNum = contNum;
                this.bodyNum = bdNum;
                this.nearJt1 = jt1;
            }
        }

        public class BoneMat   //すべての輪郭点のクラス
        {

            public OpenCvSharp.CPlusPlus.Point keyBonePos { get; set; }
            public OpenCvSharp.CPlusPlus.Point valueBonePos { get; set; }
            public Mat transMat { get; set; }
            public Mat rotMat { get; set; }
            public Mat scaleMat { get; set; }
            public Windows.Kinect.JointType joint { get; set; }

            public BoneMat(OpenCvSharp.CPlusPlus.Point keyPos, OpenCvSharp.CPlusPlus.Point valPos, Windows.Kinect.JointType jt)
            {
                this.keyBonePos = keyPos;
                this.valueBonePos = valPos;
                this.joint = jt;
            }

            public Mat totalMat()
            {
                return new Mat();
            }
        }


        public override string ToString()
        {
            return "HandElbow";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.HandElbow;
        }

        public bool IsFirstFrame { get; private set; }
    }




}
/*
if (LineToPointLength(new Vec2f(BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.X,
                                                            BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.Y),
                                                            elbowBisector[k],contour[i][j])  < sharpness ){

                                circlePoint.Add(contour[i][j]);
                            }

    */

/*
inCircle = InCircleCheck(contour[i][j], new Vec2f(BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.X,
                                                  BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.Y),
                                         new Vec2f(BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.X + elbowBisector[k].Item0,
                                                   BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.Y + elbowBisector[k].Item1),
                                         20.0f);
if(inCircle)
{
    if (nearestContour == null)
    {
        nearestContour = contour[i][j];
        nearestNum = j;
    }
    //もっと近い点が見つかったら
    if(VecLength(new Vec2f(nearestContour.Value.X - BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.X,
                           nearestContour.Value.Y - BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.Y)) >
       VecLength(new Vec2f(contour[i][j].X - BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.X,
                           contour[i][j].Y - BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.Y))) {

        nearestContour = contour[i][j];
        nearestNum = j;
    }
    */

/* 一番近い点　これは危険
//for (int k=0;k< elbowToHand.Count; k++) {
                    //    if (nearElbow[0] == null) nearElbow.Add(contour[i][j]);
                    //    if (VecLength(new Vec2f(nearElbow[0].X - BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.X,
                    //                            nearElbow[0].X - BodyDataOnDepthImage[TrackedBodyNum[k]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.Y)　>



                    //        nearElbow.Add(contour[i][j]);
                    //}
*/
/*

                //Cv2.Circle(m_buffer, new Point(BodyDataOnDepthImage[TrackedBodyNum[i]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.X + elbowBisector[i].Item0,
                //                               BodyDataOnDepthImage[TrackedBodyNum[i]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.Y + elbowBisector[i].Item1), 5, new Scalar(255, 255, 255), 2);
                //Cv2.Circle(m_buffer, new Point(BodyDataOnDepthImage[TrackedBodyNum[i]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.X,
                //                               BodyDataOnDepthImage[TrackedBodyNum[i]].JointDepth[Windows.Kinect.JointType.ElbowLeft].position.Y), 5, new Scalar(0, 255, 255), 2);

    */
