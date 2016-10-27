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

        int sharpness = 100;
        double ctlRate = 0;
        int blurRate;
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
        //List<Windows.Kinect.JointType> nearList = new List<Windows.Kinect.JointType>();
        Windows.Kinect.JointType nearBone;

        List<List<BodyContor>> bodyContList = new List<List<BodyContor>>();
        double rotShoulder;
        double rotBody;

        //Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> _BoneMap;
        //Dictionary<int, Windows.Kinect.JointType> _BoneKeyTable;
        Mat preMat = Mat.Eye(3, 3, MatType.CV_32FC1);
        Mat bodyRotMat = Mat.Eye(3, 3, MatType.CV_32FC1);
        Mat headRotMat = Mat.Eye(3, 3, MatType.CV_32FC1);
        //Mat scaleMat;
        //Mat rotMat;
        //Mat transMat;  //scale -> rot-> trans?
        //Mat transGo;
        //Mat transBack;
        //int[,,] scaleM;
        List<List<BoneMat>> List_BoneMat = new List<List<BoneMat>>();
        //Dictionary<Kinect.JointType, Point> BoneDictionary = new Dictionary<Kinect.JointType, Point>();
        //List<Dictionary<Kinect.JointType, Point>> List_BoneDictionary = new List<Dictionary<Kinect.JointType, Point>>();
        //List<Dictionary<Kinect.JointType, Point>> List_preBoneDictionary = new List<Dictionary<Kinect.JointType, Point>>();

        Dictionary<Kinect.JointType, double> BoneRads = new Dictionary<Kinect.JointType, double>();
        List<Dictionary<Kinect.JointType, double>> List_BoneRads = new List<Dictionary<Kinect.JointType, double>>();

        Dictionary<Kinect.JointType, Point> MoveMatDictionary = new Dictionary<Kinect.JointType, Point>();
        List<Dictionary<Kinect.JointType, Point>> List_moveMat = new List<Dictionary<Kinect.JointType, Point>>();

        //List<Vec2f> baseMoveX = new List<Vec2f>();
        //Dictionary<int, double > baseAttractRad = new Dictionary<int, double>();
        //double[][] baseAttractRad;

        List< List< double> > baseAttractRad = new List< List<double>>();
        //int bodyListLength = 100;
        float baseMoveX;

        DepthBody[] preBodyData;
        List<DepthBody[]> preBodyList = new List<DepthBody[]>();
        float bodyThick;
        int targetContNum;
        int frameCount = 0;
        double speedRate;
        double radAverage;

        double count = 0;
        bool useExa;
        bool useBec;
        bool useAtt;
        bool flag = false;
        //float[] preBasePos  ;
        //List<float> preBasePos = new List<float>();
        Dictionary<int, float> preBasePosDic = new Dictionary<int, float>();
        public HandElbow() : base()
        {
            (ShadowMediaUIHost.GetUI("HandElbow_con_R") as ParameterSlider).ValueChanged += HandElbow_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_con_G") as ParameterSlider).ValueChanged += HandElbow_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_con_B") as ParameterSlider).ValueChanged += HandElbow_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_rot_S") as ParameterSlider).ValueChanged += HandElbow_rot_S_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_rot_B") as ParameterSlider).ValueChanged += HandElbow_rot_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_bodyThick") as ParameterSlider).ValueChanged += HandElbow_bodyThick_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_Rate") as ParameterSlider).ValueChanged += HandElbow_Rate_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_CtlRate") as ParameterSlider).ValueChanged += HandElbow_CtlRate_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_speedRate") as ParameterSlider).ValueChanged += HandElbow_speedRate_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_UseFade") as ParameterCheckbox).ValueChanged += HandElbow_UseFade_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_UseExa") as ParameterCheckbox).ValueChanged += HandElbow_UseExa_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_UseBec") as ParameterCheckbox).ValueChanged += HandElbow_UseBec_ValueChanged;
            (ShadowMediaUIHost.GetUI("HandElbow_UseAtt") as ParameterCheckbox).ValueChanged += HandElbow_UseAtt_ValueChanged;
            /*
            (ShadowMediaUIHost.GetUI("HandElbow_CC_Blue") as ParameterButton).Clicked += HandElbow_CC_Blue_Clicked;
            (ShadowMediaUIHost.GetUI("HandElbow_CC_Orange") as ParameterButton).Clicked += HandElbow_CC_Orange_Clicked;
            (ShadowMediaUIHost.GetUI("HandElbow_CC_Yellow") as ParameterButton).Clicked += HandElbow_CC_Yellow_Clicked;
            (ShadowMediaUIHost.GetUI("HandElbow_CC_Pink") as ParameterButton).Clicked += HandElbow_CC_Pink_Clicked;
            (ShadowMediaUIHost.GetUI("HandElbow_CC_Green") as ParameterButton).Clicked += HandElbow_CC_Green_Clicked;
            */

            (ShadowMediaUIHost.GetUI("HandElbow_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_rot_S") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_rot_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_bodyThick") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_Rate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_CtlRate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_speedRate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_UseFade") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_UseExa") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_UseBec") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("HandElbow_UseAtt") as ParameterCheckbox).ValueUpdate();
        }
        /*
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
        */
        private void HandElbow_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void HandElbow_UseExa_ValueChanged(object sender, EventArgs e)
        {
            this.useExa = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }
        private void HandElbow_UseBec_ValueChanged(object sender, EventArgs e)
        {
            this.useBec = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }
        private void HandElbow_UseAtt_ValueChanged(object sender, EventArgs e)
        {
            this.useAtt = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void HandElbow_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.blurRate = (int)(e as ParameterSlider.ChangedValue).Value;

        }

        private void HandElbow_CtlRate_ValueChanged(object sender, EventArgs e)
        {
            this.ctlRate = (double)(e as ParameterSlider.ChangedValue).Value;
        }
        private void HandElbow_speedRate_ValueChanged(object sender, EventArgs e)
        {
            this.speedRate = (double)(e as ParameterSlider.ChangedValue).Value;
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

        private void HandElbow_rot_B_ValueChanged(object sender, EventArgs e)
        {
            this.rotBody = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void HandElbow_bodyThick_ValueChanged(object sender, EventArgs e)
        {
            this.bodyThick = (float)(e as ParameterSlider.ChangedValue).Value;

        }

        Mat m_buffer;
        bool m_UseFade;
        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();

            this.contour_Center.Clear();
            this.bodyContList.Clear();
            this.List_BoneRads.Clear();
            //this.baseMoveX.Clear();
            //this.baseAttractRad.Clear();

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
            if (!flag)
            {
                //for (int i = 0; i< bodyListLength; ++i)
                //{
                //    this.preBodyList.Add(BodyDataOnDepthImage);
                //}

                for (int i = 0; i < BodyData.Length; i++)
                {
                    //変位量のリストが体とエラーになるからゼロを入れておく
                    List<double> ListBuf = new List<double>();
                    for (int j =0; j <10; j++)
                    {
                        ListBuf.Add(0);
                    }
                    this.baseAttractRad.Add(new List<double>(ListBuf)); 

                    if (BodyData[i].IsTracked)
                    {
                        if (CheckBodyInSCreen(i, src))
                        {
                            this.preBasePosDic.Add(i, BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].position.x);
                        }
                    }
                }
                flag = true;
            }



            //if (count > 2 * Math.PI) count = 0;
            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            Cv2.MedianBlur(grayimage, grayimage, 11);
            Cv2.Dilate(grayimage,grayimage,new Mat(),null,1); //←適宜

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
                this.bodyConts.Clear();
                this.BoneRads.Clear();
                if (BodyData[i].IsTracked)
                {
                    if (CheckBodyInSCreen(i, src))
                    {
                        //Boneの角度情報リスト作成   spinebase基準 全部で9箇所
                        #region

                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ElbowLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.WristLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ShoulderLeft].state != Kinect.TrackingState.NotTracked)
                            this.BoneRads.Add(Kinect.JointType.ElbowLeft, GetBoneRad(i, Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft));
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ShoulderLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ElbowLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                            this.BoneRads.Add(Kinect.JointType.ShoulderLeft, GetBoneRad(i, Kinect.JointType.ElbowLeft, Kinect.JointType.SpineMid));
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ElbowRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.WristRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ShoulderRight].state != Kinect.TrackingState.NotTracked)
                            this.BoneRads.Add(Kinect.JointType.ElbowRight, GetBoneRad(i, Kinect.JointType.WristRight, Kinect.JointType.ElbowRight));
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ShoulderRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ElbowRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                            this.BoneRads.Add(Kinect.JointType.ShoulderRight, GetBoneRad(i, Kinect.JointType.ElbowRight, Kinect.JointType.SpineMid));
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.Neck].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.Head].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                            this.BoneRads.Add(Kinect.JointType.Neck, GetBoneRad(i, Kinect.JointType.Head, Kinect.JointType.SpineMid));

                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.KneeLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.AnkleLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                            this.BoneRads.Add(Kinect.JointType.KneeLeft, GetBoneRad(i, Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft));
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.KneeLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                            this.BoneRads.Add(Kinect.JointType.HipLeft, GetBoneRad(i, Kinect.JointType.KneeLeft, Kinect.JointType.SpineBase)); //便宜上HIPを使う　spineBaseだと名前が被る
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.KneeRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.AnkleRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                            this.BoneRads.Add(Kinect.JointType.KneeRight, GetBoneRad(i, Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight));
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.KneeRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                            this.BoneRads.Add(Kinect.JointType.HipRight, GetBoneRad(i, Kinect.JointType.KneeRight, Kinect.JointType.SpineBase)); //便宜上HIPを使う　spineBaseだと名前が被る

                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked)
                            this.BoneRads.Add(Kinect.JointType.SpineBase, GetBoneRad(i, Kinect.JointType.SpineBase));//軸の回転角度

                        //this.BoneRads.Add(Kinect.JointType.ElbowLeft, GetBoneRad(i, Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft));
                        //this.BoneRads.Add(Kinect.JointType.ShoulderLeft, GetBoneRad(i, Kinect.JointType.ElbowLeft, Kinect.JointType.SpineMid));
                        //this.BoneRads.Add(Kinect.JointType.ElbowRight, GetBoneRad(i, Kinect.JointType.WristRight, Kinect.JointType.ElbowRight));
                        //this.BoneRads.Add(Kinect.JointType.ShoulderRight, GetBoneRad(i, Kinect.JointType.ElbowRight, Kinect.JointType.SpineMid));
                        //this.BoneRads.Add(Kinect.JointType.Neck, GetBoneRad(i, Kinect.JointType.Head, Kinect.JointType.SpineMid));

                        //this.BoneRads.Add(Kinect.JointType.KneeLeft, GetBoneRad(i, Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft));
                        //this.BoneRads.Add(Kinect.JointType.HipLeft, GetBoneRad(i, Kinect.JointType.KneeLeft, Kinect.JointType.SpineBase)); //便宜上HIPを使う　spineBaseだと名前が被る
                        //this.BoneRads.Add(Kinect.JointType.KneeRight, GetBoneRad(i, Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight));
                        //this.BoneRads.Add(Kinect.JointType.HipRight, GetBoneRad(i, Kinect.JointType.KneeRight, Kinect.JointType.SpineBase)); //便宜上HIPを使う　spineBaseだと名前が被る

                        //this.BoneRads.Add(Kinect.JointType.SpineBase, GetBoneRad(i, Kinect.JointType.SpineBase));//軸の回転角度
                        this.List_BoneRads.Add(new Dictionary<Kinect.JointType, double>(BoneRads));
                        #endregion

                        //SpineBaseの移動量測定
                        if (this.preBasePosDic.ContainsKey(i))
                        {
                            //Debug.Log("bodyNum :  " + i);
                            //Debug.Log("basePre :  " + this.preBasePosDic[i]);
                            //Debug.Log("baseNow :  " + (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].position.X));
                            //Debug.Log("baseMove :  " + (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].position.X - this.preBasePosDic[i]));
                            //this.baseMoveX.Add(new Vec2f(i, BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].position.X - preBodyData[i].JointDepth[Kinect.JointType.SpineBase].position.X));
                            baseMoveX = BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].position.x - this.preBasePosDic[i];
                            if (baseMoveX > 2) baseMoveX = 2;
                            //this.baseAttractRad.Add(i, baseMoveX / 2 * Math.PI / 3);
                            this.baseAttractRad[i].Insert(0, baseMoveX / 2 * Math.PI / 3);
                            if (this.baseAttractRad.Count > 20) baseAttractRad.RemoveAt(this.baseAttractRad.Count -1); 
                        }


                        //トラッキングされているbodyにつき最も近い輪郭点群を探し番号を取得
                        contNumBuf = 0;
                        for (int j = 0; j < this.contour_Center.Count; ++j)
                        {
                            if (PointsLength(JointToDepthPoint(i, Kinect.JointType.SpineBase), this.contour_Center[contNumBuf]) >
                                PointsLength(JointToDepthPoint(i, Kinect.JointType.SpineBase), this.contour_Center[j]))
                            {
                                contNumBuf = j;
                            }
                        }

                        //見つけた輪郭点群の ある点が最も近いボーンを探す
                        for (int k = 0; k < this.List_Contours[contNumBuf].Count; k++)
                        {

                            toBoneMin = 5000;
                            for (int m = 0; m < _BoneKeyTable.Count; m++)
                            {
                                toBoneLength = PointsLength(JointToDepthPoint(i, this._BoneMap[this._BoneKeyTable[m]]) - this.List_Contours[contNumBuf][k] + JointToDepthPoint(i, this._BoneKeyTable[m]) - this.List_Contours[contNumBuf][k]);

                                if (this._BoneKeyTable[m] == Kinect.JointType.SpineBase) toBoneLength *= bodyThick;
                                if (toBoneMin > toBoneLength)
                                {
                                    toBoneMin = toBoneLength;
                                    nearBone = _BoneKeyTable[m];
                                }

                                //左右の肩の部分をSpineBaseに再編
                                if (nearBone == Kinect.JointType.Head)
                                {
                                    toBoneLength = PointsLength(JointToDepthPoint(i,Kinect.JointType.SpineShoulder) - this.List_Contours[contNumBuf][k] + JointToDepthPoint(i, Kinect.JointType.ShoulderLeft) - this.List_Contours[contNumBuf][k]);
                                    if (toBoneMin > toBoneLength) nearBone = Kinect.JointType.SpineBase;
                                    toBoneLength = PointsLength(JointToDepthPoint(i, Kinect.JointType.SpineShoulder) - this.List_Contours[contNumBuf][k] + JointToDepthPoint(i, Kinect.JointType.ShoulderRight) - this.List_Contours[contNumBuf][k]);
                                    if (toBoneMin > toBoneLength) nearBone = Kinect.JointType.SpineBase;
                                }


                            }
                            //Cv2.Circle(m_buffer, this.List_Contours[contNumBuf][k], 5, new Scalar(0, 255, 255), 2);
                            this.bodyConts.Add(new BodyContor(this.List_Contours[contNumBuf][k], contNumBuf, i, nearBone));
                        }

                        this.bodyContList.Add(new List<BodyContor>(bodyConts));
                        //Cv2.Line(m_buffer, JointToDepthPoint(i, Windows.Kinect.JointType.SpineBase), this.contour_Center[contNumBuf], new Scalar(255, 200, 100));


                    }
                }
            }


            for (int i = 0; i < this.contour_Center.Count; i++)
            {
                //Cv2.Circle(m_buffer, contour_Center[i], 3, new Scalar(255, 255, 0), 2);
            }


            //取得した輪郭点について1つずつ処理
            this.List_Contours.Clear();

            for (int i = 0; i < bodyContList.Count; i++)
            {
                //Cv2.Line(m_buffer, JointToDepthPoint(this.bodyContList[i][0].bodyNum, Windows.Kinect.JointType.SpineBase), this.contour_Center[this.bodyContList[i][0].contourNum], new Scalar(255, 200, 100));
                this.CvPoints.Clear();

                //入れ替える対象のナンバー
                targetContNum = i + 1;
                if (targetContNum == List_BoneRads.Count) targetContNum = 0;
                //何もないときはこれが回る↓
                bodyRotMat = GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.SpineBase, rotBody);
                //動きが傾きになって表れるもーど↓
                if (useAtt) {
                    //移動量から求めた回転量の5フレーム分の平均を求める
                    radAverage = 0;
                    for (int j = 0; j < 5; j++)
                    {
                        radAverage += baseAttractRad[i][j] / 5;
                    }
                    bodyRotMat = GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.SpineBase, radAverage);
                }
                preMat = GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.ShoulderLeft, rotShoulder, bodyRotMat);   //新しい関数を作るところから

                for (int j = 0; j < this.bodyContList[i].Count; j++)  //順番
                {

                    switch (bodyContList[i][j].nearJt1)
                    {
                        case Kinect.JointType.AnkleLeft:

                            break;

                        case Kinect.JointType.KneeLeft:

                            break;

                        case Kinect.JointType.AnkleRight:

                            break;

                        case Kinect.JointType.KneeRight:

                            break;

                        case Kinect.JointType.WristLeft:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, preMat);

                            if (List_BoneRads[i].ContainsKey(Kinect.JointType.ElbowLeft) == true && List_BoneRads[targetContNum].ContainsKey(Kinect.JointType.ElbowLeft) == true)
                            {
                                //Debug.Log("elbow.x : " + BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ElbowLeft].position.X);

                                if (!double.IsNaN(List_BoneRads[targetContNum][Kinect.JointType.ElbowLeft] - List_BoneRads[i][Kinect.JointType.ElbowLeft]))
                                {

                                    //Debug.Log("problemRad : " + (List_BoneRads[targetContNum][Kinect.JointType.ElbowLeft] - List_BoneRads[i][Kinect.JointType.ElbowLeft]));
                                    //こいつが止めている↓
                                    bodyContList[i][j].center = ContourRotate(bodyContList[i][j], List_BoneRads[targetContNum][Kinect.JointType.ElbowLeft] - List_BoneRads[i][Kinect.JointType.ElbowLeft], preMat);
                                }
                            }

                            if (useExa == true)  //遊び
                            {
                                bodyContList[i][j].center = ContourRotate(bodyContList[i][j], count , preMat);
                            }
                            break;

                        case Kinect.JointType.ElbowLeft:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], rotShoulder, bodyRotMat);
                            //Cv2.Circle(m_buffer, ContourRotate(bodyContList[i][j], ctlRate), 5, new Scalar(255, 255, 0), 2);
                            break;

                        case Kinect.JointType.WristRight:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, bodyRotMat);

                            break;

                        case Kinect.JointType.ElbowRight:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, bodyRotMat);

                            break;

                        case Kinect.JointType.SpineBase:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, bodyRotMat);

                            break;

                        case Kinect.JointType.Head:
                            if (useBec)
                            {
                                headRotMat = GetRotateMat(this.bodyContList[i][0].bodyNum,Kinect.JointType.SpineShoulder, Math.PI / 6 * Math.Sin(count + i * Math.PI / 8), bodyRotMat);
                                bodyContList[i][j].center = ContourRotate(bodyContList[i][j],0, headRotMat);
                            }
                            else
                            {
                                bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, bodyRotMat);

                            }



                            break;

                        default:

                            break;
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

            this.preBasePosDic.Clear();
            for (int i = 0; i < BodyData.Length; i++)
            {
                if (BodyData[i].IsTracked)
                {
                    if (CheckBodyInSCreen(i, src))
                    {
                        this.preBasePosDic.Add(i, BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].position.x);

                    }
                }
            }

            //preBodyData = BodyDataOnDepthImage;
            //10個溜まったら10個目を消す
            //if (this.preBodyList.Count > bodyListLength) this.preBodyList.RemoveAt(bodyListLength-1);
            //this.preBodyList.Insert(0, new DepthBody[](BodyDataOnDepthImage));
            count += (0.01 * speedRate);
            if (count > 2 * Math.PI) count = 0;

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

            { Windows.Kinect.JointType.SpineMid, Windows.Kinect.JointType.SpineBase },
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
            return new Point(BodyDataOnDepthImage[bodyNum].JointDepth[jt].position.x, BodyDataOnDepthImage[bodyNum].JointDepth[jt].position.y);
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
        private Mat GetRotateMat(int bodyNum, Kinect.JointType pivotJt, double rad)
        {
            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bodyNum,pivotJt ).X },
                                     {0,1, -JointToDepthPoint(bodyNum, pivotJt).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bodyNum, pivotJt ).X },
                                       {0,1, JointToDepthPoint(bodyNum,  pivotJt  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);


            return backOriMat * rotMat * goOriMat;
        }
        private Mat GetRotateMat(int bodyNum, Kinect.JointType pivotJt, double rad, Mat preMat3x3)
        {
            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bodyNum,pivotJt ).X },
                                     {0,1, -JointToDepthPoint(bodyNum, pivotJt).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bodyNum,  pivotJt ).X },
                                       {0,1, JointToDepthPoint(bodyNum,  pivotJt  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);


            return preMat3x3 * backOriMat * rotMat * goOriMat;
        }

        //ボーンの前情報との比較から変換行列を算出
        private Mat GetBoneMoveMat(int bodyNum, Kinect.JointType nearJt, Kinect.JointType farJt, DepthBody[] preBody)
        {

            float[,] transMatArr = {{ 1,0, JointToDepthPoint(bodyNum, nearJt).X - preBody[bodyNum].JointDepth[nearJt].position.x },
                                    { 0,1, JointToDepthPoint(bodyNum, nearJt).Y - preBody[bodyNum].JointDepth[nearJt].position.y },
                                    { 0,0,1 }};
            Mat transMat = new Mat(3, 3, MatType.CV_32FC1, transMatArr);

            float scaleRate = PointsLength(JointToDepthPoint(bodyNum, nearJt), JointToDepthPoint(bodyNum, farJt)) /
                              PointsLength(new Point(preBody[bodyNum].JointDepth[nearJt].position.x - preBody[bodyNum].JointDepth[farJt].position.x, preBody[bodyNum].JointDepth[nearJt].position.y - preBody[bodyNum].JointDepth[farJt].position.y));
            float[,] scaleMatArr = { { scaleRate, 0, 0   },
                                     { 0,scaleRate, 0 },
                                     { 0, 0, 1 } };
            Mat scaleMat = new Mat(3, 3, MatType.CV_32FC1, scaleMatArr);


            float[,] goOriMatArr = { {1,0, - preBody[bodyNum].JointDepth[nearJt].position.x },
                                     {0,1, - preBody[bodyNum].JointDepth[nearJt].position.y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1, 0, preBody[bodyNum].JointDepth[nearJt].position.x  },
                                       {0, 1, preBody[bodyNum].JointDepth[nearJt].position.y  },
                                       {0, 0, 1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);

            //内積から角度を計算
            double boneRad = Math.Acos((double)((JointToDepthPoint(bodyNum, farJt).X - JointToDepthPoint(bodyNum, nearJt).X) * (preBody[bodyNum].JointDepth[farJt].position.x - preBody[bodyNum].JointDepth[nearJt].position.x) +
                                                (JointToDepthPoint(bodyNum, farJt).Y - JointToDepthPoint(bodyNum, nearJt).Y) * (preBody[bodyNum].JointDepth[farJt].position.y - preBody[bodyNum].JointDepth[nearJt].position.y)) /
                                                 PointsLength(JointToDepthPoint(bodyNum, farJt), JointToDepthPoint(bodyNum, nearJt)) * PointsLength(new Point(preBody[bodyNum].JointDepth[farJt].position.x - preBody[bodyNum].JointDepth[nearJt].position.x, preBody[bodyNum].JointDepth[farJt].position.y - preBody[bodyNum].JointDepth[nearJt].position.y)));

            //det = ax * by - ay * bx = - or +  どっち向きかの判断
            if ((JointToDepthPoint(bodyNum, farJt).X - JointToDepthPoint(bodyNum, nearJt).X) * (preBody[bodyNum].JointDepth[farJt].position.y - preBody[bodyNum].JointDepth[nearJt].position.y) -
                (JointToDepthPoint(bodyNum, farJt).Y - JointToDepthPoint(bodyNum, nearJt).Y) * (preBody[bodyNum].JointDepth[farJt].position.x - preBody[bodyNum].JointDepth[nearJt].position.x) < 0)
            {
                boneRad *= -1;   //ちょっとここはやってみてから判断笑
            }

            float[,] rotMatArr = { { (float)Math.Cos(boneRad), (float)-Math.Sin(boneRad),0 },
                                   { (float)Math.Sin(boneRad), (float) Math.Cos(boneRad),0  },
                                   {0, 0, 1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);

            //  順番 <----  　　完全に一致!　<- 平行移動　<- 元の場所へもどす　<- 回転＋拡大縮小　<- 原点へ移動
            return transMat * backOriMat * rotMat * scaleMat * goOriMat;

        }
        /*
        public double GetBoneRad(int bodyNum, Kinect.JointType farJt, Kinect.JointType pivotJt, Kinect.JointType nearJt)
        {
            //内積から角度を計算
           
            double boneRad = Math.Acos(((JointToDepthPoint(bodyNum, farJt).X - JointToDepthPoint(bodyNum, pivotJt).X) * (JointToDepthPoint(bodyNum, pivotJt).X - JointToDepthPoint(bodyNum, nearJt).X) +
                                        (JointToDepthPoint(bodyNum, farJt).Y - JointToDepthPoint(bodyNum, pivotJt).Y) * (JointToDepthPoint(bodyNum, pivotJt).Y - JointToDepthPoint(bodyNum, nearJt).Y)) /
                                          PointsLength(JointToDepthPoint(bodyNum, farJt), JointToDepthPoint(bodyNum, pivotJt)) / PointsLength(JointToDepthPoint(bodyNum, pivotJt), JointToDepthPoint(bodyNum, nearJt)));

            //det = ax * by - ay * bx = - or +  どっち向きかの判断
            if ((JointToDepthPoint(bodyNum, farJt).X - JointToDepthPoint(bodyNum, pivotJt).X) * (JointToDepthPoint(bodyNum, pivotJt).Y - JointToDepthPoint(bodyNum, nearJt).Y) -
                (JointToDepthPoint(bodyNum, farJt).Y - JointToDepthPoint(bodyNum, pivotJt).Y) * (JointToDepthPoint(bodyNum, pivotJt).X - JointToDepthPoint(bodyNum, nearJt).X) < 0)
            {
                boneRad *= -1;   // これで左曲がりがマイナス　右曲がりがプラス
            }
            return boneRad;
        }
        */
        public double GetBoneRad(int bodyNum, Kinect.JointType farJtTop, Kinect.JointType nearJtTop)
        {
            //内積から角度を計算
            double boneRad = Math.Acos(((JointToDepthPoint(bodyNum, farJtTop).X - JointToDepthPoint(bodyNum, _BoneMap[farJtTop]).X) * (JointToDepthPoint(bodyNum, nearJtTop).X - JointToDepthPoint(bodyNum, _BoneMap[nearJtTop]).X) +
                                        (JointToDepthPoint(bodyNum, farJtTop).Y - JointToDepthPoint(bodyNum, _BoneMap[farJtTop]).Y) * (JointToDepthPoint(bodyNum, nearJtTop).Y - JointToDepthPoint(bodyNum, _BoneMap[nearJtTop]).Y)) /
                                          PointsLength(JointToDepthPoint(bodyNum, farJtTop), JointToDepthPoint(bodyNum, _BoneMap[farJtTop])) / PointsLength(JointToDepthPoint(bodyNum, nearJtTop), JointToDepthPoint(bodyNum, _BoneMap[nearJtTop])));

            //det = ax * by - ay * bx = - or +  どっち向きかの判断
            if ((JointToDepthPoint(bodyNum, farJtTop).X - JointToDepthPoint(bodyNum, _BoneMap[farJtTop]).X) * (JointToDepthPoint(bodyNum, nearJtTop).Y - JointToDepthPoint(bodyNum, _BoneMap[nearJtTop]).Y) -
                (JointToDepthPoint(bodyNum, farJtTop).Y - JointToDepthPoint(bodyNum, _BoneMap[farJtTop]).Y) * (JointToDepthPoint(bodyNum, nearJtTop).X - JointToDepthPoint(bodyNum, _BoneMap[nearJtTop]).X) < 0)
            {
                boneRad *= -1;   // これで左曲がりがマイナス　右曲がりがプラス
            }
            return boneRad;
        }

        public double GetBoneRad(int bodyNum, Kinect.JointType baseJtTop)
        {
            //内積から角度を計算  (0,-1 ) との内積を求める

            double baseRad = Math.Acos((-(JointToDepthPoint(bodyNum, baseJtTop).Y - JointToDepthPoint(bodyNum, _BoneMap[baseJtTop]).Y)) /
                                          PointsLength(JointToDepthPoint(bodyNum, baseJtTop), JointToDepthPoint(bodyNum, _BoneMap[baseJtTop])));

            //det = ax * by - ay * bx = - or +  どっち向きかの判断
            //det = 0 * by - (-1)* bx = bx
            if (JointToDepthPoint(bodyNum, baseJtTop).X - JointToDepthPoint(bodyNum, _BoneMap[baseJtTop]).X < 0)
            {
                baseRad *= -1;   // これで左曲がりがマイナス　右曲がりがプラス
            }
            return baseRad;
        }

        bool CheckBodyInSCreen(int bodyNum, Mat srcMat)
        {
            bool bl = true;
            //for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)

            for (int i = 0; i < this._TrackigBoneList.Count; ++i)
            {

                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x > srcMat.Width) bl = false;
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.X > srcMat.Width) Debug.Log("OverNum : "  + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.X); 
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x < 0) bl = false;
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.X < 0 ) Debug.Log("OverNum : " + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.X);
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y > srcMat.Height) bl = false;
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.Y > srcMat.Height) Debug.Log("OverNum : " + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.Y);
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y < 0) bl = false;
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.Y < 0) Debug.Log("OverNum : " + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.Y);
                if (double.IsInfinity(this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x))
                {
                    //    Debug.Log("lostBonex : " + _TrackigBoneList[i]);
                    bl = false;
                }
                if (double.IsInfinity(this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y))
                {
                    //    Debug.Log("lostBoney : " + _TrackigBoneList[i]);
                    bl = false;
                }
                /*
                画面内にあるものも消してしまいそう↓
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].state == Kinect.TrackingState.NotTracked)
                {
                    Debug.Log("UntrackingBone : " + _TrackigBoneList[i]);
                }
                */
            }

            return bl;
        }

        //bool CheckBoneAvailable(int BodyNum,Kinect.JointType jt1, Kinect.JointType jt2, Kinect.JointType jt3)
        //{

        //}


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
