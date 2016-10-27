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
    class Ahead : AShadowImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 180;
        double accRate;
        double preRotRate;
        double preMoveRate;
        int blurRate;
        float attractionRate;
        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours_Buffer = new List<List<Point>>();
        Scalar color;
        Scalar colorBack;

        List<Point> contour_Center = new List<Point>();

        bool inCircle;
        int contNumBuf = 0;

        List<BodyContor> bodyConts = new List<BodyContor>();
        float toBoneLength;
        float toBoneMin;
        Windows.Kinect.JointType nearBone;

        List<List<BodyContor>> bodyContList = new List<List<BodyContor>>();


        Mat preMat = Mat.Eye(3, 3, MatType.CV_32FC1);
        Mat bodyRotMat = Mat.Eye(3, 3, MatType.CV_32FC1);
        Mat headRotMat = Mat.Eye(3, 3, MatType.CV_32FC1);
        List<List<BoneMat>> List_BoneMat = new List<List<BoneMat>>();

        Dictionary<Kinect.JointType, Mat> SwapBodyMats = new Dictionary<Kinect.JointType, Mat>();

        Dictionary<Kinect.JointType, double> BoneRads = new Dictionary<Kinect.JointType, double>();
        List<Dictionary<Kinect.JointType, double>> List_PreBoneRads = new List<Dictionary<Kinect.JointType, double>>();
        List<Dictionary<Kinect.JointType, double>> List_BoneRads = new List<Dictionary<Kinect.JointType, double>>();
        List<List<Dictionary<Kinect.JointType, double>>> Tree_BoneRads = new List<List<Dictionary<Kinect.JointType, double>>>();

        Dictionary<Kinect.JointType, double> BoneVels = new Dictionary<Kinect.JointType, double>();
        List<Dictionary<Kinect.JointType, double>> List_PreBoneVels = new List<Dictionary<Kinect.JointType, double>>();
        List<Dictionary<Kinect.JointType, double>> List_BoneVels = new List<Dictionary<Kinect.JointType, double>>();
        List<List<Dictionary<Kinect.JointType, double>>> Tree_BoneVels = new List<List<Dictionary<Kinect.JointType, double>>>();

        Dictionary<Kinect.JointType, double> BoneAccs = new Dictionary<Kinect.JointType, double>();
        List<List<Dictionary<Kinect.JointType, double>>> Tree_BoneAccs = new List<List<Dictionary<Kinect.JointType, double>>>();
        List<Dictionary<Kinect.JointType, double>> List_BoneAccs = new List<Dictionary<Kinect.JointType, double>>();
        List<Dictionary<Kinect.JointType, double>> List_PreBoneAccs = new List<Dictionary<Kinect.JointType, double>>();

        Dictionary<Kinect.JointType, Point> MoveMatDictionary = new Dictionary<Kinect.JointType, Point>();
        List<Dictionary<Kinect.JointType, Point>> List_moveMat = new List<Dictionary<Kinect.JointType, Point>>();

        //List<List<double>> baseAttractRad = new List<List<double>>();
        //float baseMoveX;

        //DepthBody[] preBodyData;
        //List<DepthBody[]> preBodyList = new List<DepthBody[]>();  //更新されてしまってうまくいかない
        float bodyThick = 0.8f;
        int targetContNum;
        int targetAccContNum;
        int targetAttContNum;
        int frameCount = 0;
        double speedRate;
        double radAverage;
        double baseMoveAve;


        double count = 0;
        bool useRot;
        bool usePre;

        bool devideCont;
        bool reverceCheck;
        bool brightCheck;
        bool addImg;
        bool useAve;
        bool flag = false;
        // Dictionary<int, float> preBasePosDic = new Dictionary<int, float>();

        List<float> nowBasePos = new List<float>();
        List<List<float>> Tree_BasePos = new List<List<float>>();
        List<float> nowBaseVels = new List<float>();
        List<List<float>> Tree_BaseVel = new List<List<float>>();


        public Ahead() : base()
        {
            (ShadowMediaUIHost.GetUI("Ahead_con_R") as ParameterSlider).ValueChanged += Ahead_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_con_G") as ParameterSlider).ValueChanged += Ahead_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_con_B") as ParameterSlider).ValueChanged += Ahead_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_bgd_R") as ParameterSlider).ValueChanged += Ahead_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_bgd_G") as ParameterSlider).ValueChanged += Ahead_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_bgd_B") as ParameterSlider).ValueChanged += Ahead_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_preRotRate") as ParameterSlider).ValueChanged += Ahead_preRotRate_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_preMoveRate") as ParameterSlider).ValueChanged += Ahead_preMoveRate_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_UseFade") as ParameterCheckbox).ValueChanged += Ahead_UseFade_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_UseDiv") as ParameterCheckbox).ValueChanged += Ahead_UseDiv_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_UseRot") as ParameterCheckbox).ValueChanged += Ahead_UseRot_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_UsePre") as ParameterCheckbox).ValueChanged += Ahead_UsePre_ValueChanged;
            (ShadowMediaUIHost.GetUI("Ahead_AddImg") as ParameterCheckbox).ValueChanged += Ahead_AddImg_ValueChanged;

            (ShadowMediaUIHost.GetUI("Ahead_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_preRotRate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_preMoveRate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_UseFade") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_UseDiv") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_UseRot") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_UsePre") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Ahead_AddImg") as ParameterCheckbox).ValueUpdate();
        }

        private void Ahead_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void Ahead_UseDiv_ValueChanged(object sender, EventArgs e)
        {
            this.devideCont = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }
      
       
        private void Ahead_UseRot_ValueChanged(object sender, EventArgs e)
        {
            this.useRot = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }
        private void Ahead_UsePre_ValueChanged(object sender, EventArgs e)
        {
            this.usePre = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }
       
        private void Ahead_AddImg_ValueChanged(object sender, EventArgs e)
        {
            this.addImg = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void Ahead_preRotRate_ValueChanged(object sender, EventArgs e)
        {
            this.preRotRate = (double)(e as ParameterSlider.ChangedValue).Value;

        }
        private void Ahead_preMoveRate_ValueChanged(object sender, EventArgs e)
        {
            this.preMoveRate = (double)(e as ParameterSlider.ChangedValue).Value;

        }


        private void Ahead_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Ahead_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Ahead_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }


        private void Ahead_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Ahead_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Ahead_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }
        


        Mat m_buffer;
        bool m_UseFade;
        Mat m_cont_buffer;
        Mat m_first_buffer;
        Mat m_second_buffer;

        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();
            this.contour_Center.Clear();
            this.bodyContList.Clear();
            this.List_BoneRads.Clear();
            this.List_BoneVels.Clear();
            this.List_BoneAccs.Clear();

            if (m_buffer == null)
            {
                m_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, new Scalar(0,0,0));
                m_first_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, new Scalar(0, 0, 0));
                m_second_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, new Scalar(0, 0, 0));
            }
            else
            {
                if (this.m_UseFade)
                {
                    m_buffer *= 0.9;
                    m_first_buffer *= 0;
                    m_second_buffer *= 0;
                }
                else
                {
                    m_buffer *= 0;
                    m_first_buffer *= 0;
                    m_second_buffer *= 0;
                }
            }
            if (m_cont_buffer == null)
            {
                m_cont_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, new Scalar(0, 0, 0));
            }
            else
            {
                m_cont_buffer *= 0;
            }


            if (!flag)
            {

                for (int i = 0; i < BodyData.Length; i++)
                {

                    if (BodyData[i].IsTracked)
                    {
                        if (CheckBodyInSCreen(i, src))
                        {

                        }
                    }
                }

                this.flag = true;
            }



            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);

            //輪郭を分けるか分けないか
            if (!this.devideCont)
            {
                //輪郭はくっつくけどある程度滑らか
                Cv2.MedianBlur(grayimage, grayimage, 9);
                //Cv2.Dilate(grayimage, grayimage, new Mat(), null, 1); //←適宜

            }
            else
            {
                //輪郭がくっつかないようにする　ノイズは乗る
                Cv2.Erode(grayimage, grayimage, new Mat(), null, 1);
                Cv2.MedianBlur(grayimage, grayimage, 3);
                Cv2.Dilate(grayimage, grayimage, new Mat(), null, 1);
            }

            //dstMat = new Mat(dst.Height, dst.Width, MatType.CV_8UC4,new Scalar(0,0,0));
            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, new Scalar(0, 0, 0));


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
            var _contour = List_Contours.ToArray();
           
            Cv2.DrawContours(m_first_buffer, _contour, -1, colorBack, -1, OpenCvSharp.LineType.Link8);

            



            //輪郭点の分類を開始
            this.contNumBuf = 0;
            this.nowBasePos.Clear();


            for (int i = 0; i < BodyData.Length; i++)
            {
                this.bodyConts.Clear();
                this.BoneRads.Clear();
                if (BodyData[i].IsTracked)
                {
                    if (this.CheckBodyInSCreen(i, src))
                    {
                        //Boneの角度情報リスト作成   spinebase基準 全部で9箇所
                        #region boneRad
                        //骨格がUntrackingではなく、かつふたつのボーンのなす角がNaNではないときに角度取得
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ElbowLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.WristLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ShoulderLeft].state != Kinect.TrackingState.NotTracked)
                        {
                            if (!double.IsNaN(GetBoneRad(i, Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft))) this.BoneRads.Add(Kinect.JointType.ElbowLeft, GetBoneRad(i, Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft));
                        }
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ShoulderLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ElbowLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                        {
                            if (!double.IsNaN(GetBoneRad(i, Kinect.JointType.ElbowLeft, Kinect.JointType.SpineMid))) this.BoneRads.Add(Kinect.JointType.ShoulderLeft, GetBoneRad(i, Kinect.JointType.ElbowLeft, Kinect.JointType.SpineMid));
                        }
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ElbowRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.WristRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ShoulderRight].state != Kinect.TrackingState.NotTracked)
                        {
                            if (!double.IsNaN(GetBoneRad(i, Kinect.JointType.WristRight, Kinect.JointType.ElbowRight))) this.BoneRads.Add(Kinect.JointType.ElbowRight, GetBoneRad(i, Kinect.JointType.WristRight, Kinect.JointType.ElbowRight));
                        }
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ShoulderRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.ElbowRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                        {
                            if (!double.IsNaN(GetBoneRad(i, Kinect.JointType.ElbowRight, Kinect.JointType.SpineMid))) this.BoneRads.Add(Kinect.JointType.ShoulderRight, GetBoneRad(i, Kinect.JointType.ElbowRight, Kinect.JointType.SpineMid));
                        }
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.Neck].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.Head].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                        {
                            if (!double.IsNaN(GetBoneRad(i, Kinect.JointType.Head, Kinect.JointType.SpineMid))) this.BoneRads.Add(Kinect.JointType.Neck, GetBoneRad(i, Kinect.JointType.Head, Kinect.JointType.SpineMid));
                        }
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.KneeLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.AnkleLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                        {
                            if (!double.IsNaN(GetBoneRad(i, Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft))) this.BoneRads.Add(Kinect.JointType.KneeLeft, GetBoneRad(i, Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft));
                        }
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.KneeLeft].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                        {
                            if (!double.IsNaN(GetBoneRad(i, Kinect.JointType.KneeLeft, Kinect.JointType.SpineBase))) this.BoneRads.Add(Kinect.JointType.HipLeft, GetBoneRad(i, Kinect.JointType.KneeLeft, Kinect.JointType.SpineBase)); //便宜上HIPを使う　spineBaseだと名前が被る
                        }
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.KneeRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.AnkleRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                        {
                            if (!double.IsNaN(GetBoneRad(i, Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight))) this.BoneRads.Add(Kinect.JointType.KneeRight, GetBoneRad(i, Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight));
                        }
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.KneeRight].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked)
                        {
                            if (!double.IsNaN(GetBoneRad(i, Kinect.JointType.KneeRight, Kinect.JointType.SpineBase))) this.BoneRads.Add(Kinect.JointType.HipRight, GetBoneRad(i, Kinect.JointType.KneeRight, Kinect.JointType.SpineBase)); //便宜上HIPを使う　spineBaseだと名前が被る
                        }
                        if (BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].state != Kinect.TrackingState.NotTracked && BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineMid].state != Kinect.TrackingState.NotTracked)
                        {
                            //Debug.Log("baserad" + GetBoneRad(i, Kinect.JointType.SpineBase));
                            //Debug.Log("bool " + double.IsNaN(GetBoneRad(i, Kinect.JointType.SpineBase)));
                            //if (!double.IsNaN(GetBoneRad(i, Kinect.JointType.SpineBase))) this.BoneRads.Add(Kinect.JointType.SpineBase, GetBoneRad(i, Kinect.JointType.SpineBase));//軸の回転角度

                            if (!double.IsNaN(GetBoneRad(i, Kinect.JointType.SpineBase))) this.BoneRads.Add(Kinect.JointType.SpineBase, GetBoneRad(i, Kinect.JointType.SpineBase));//軸の回転角度
                        }
                        this.List_BoneRads.Add(new Dictionary<Kinect.JointType, double>(this.BoneRads));
                        #endregion



                        //spineBase位置新しくツリーで格納
                        this.nowBasePos.Add(BodyDataOnDepthImage[i].JointDepth[Kinect.JointType.SpineBase].position.x);



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
                                    toBoneLength = PointsLength(JointToDepthPoint(i, Kinect.JointType.SpineShoulder) - this.List_Contours[contNumBuf][k] + JointToDepthPoint(i, Kinect.JointType.ShoulderLeft) - this.List_Contours[contNumBuf][k]);
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
            //角度情報ツリーを更新
            if (this.Tree_BoneRads.Count == 0) this.Tree_BoneRads.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneRads));
            this.Tree_BoneRads.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneRads));
            if (this.Tree_BoneRads.Count > 5) this.Tree_BoneRads.RemoveAt(this.Tree_BoneRads.Count - 1);

            //SpineBase位置情報ツリーを更新
            if (this.Tree_BasePos.Count == 0) this.Tree_BasePos.Insert(0, new List<float>(this.nowBasePos));
            this.Tree_BasePos.Insert(0, new List<float>(this.nowBasePos));
            if (this.Tree_BasePos.Count > 15) this.Tree_BasePos.RemoveAt(this.Tree_BasePos.Count - 1);


            //Boneの角速度情報リスト作成 Treeを使ったバージョン
            #region boneVel　treeUse
            for (int i = 0; i < this.Tree_BoneRads[0].Count; i++)
            {
                this.BoneVels.Clear();
                if (this.Tree_BoneRads[1].Count > i)
                {
                    if (this.Tree_BoneRads[0][i].ContainsKey(Kinect.JointType.ElbowLeft) && this.Tree_BoneRads[1][i].ContainsKey(Kinect.JointType.ElbowLeft))
                    {
                        this.BoneVels.Add(Kinect.JointType.ElbowLeft, this.Tree_BoneRads[0][i][Kinect.JointType.ElbowLeft] - this.Tree_BoneRads[1][i][Kinect.JointType.ElbowLeft]);
                    }
                    if (this.Tree_BoneRads[0][i].ContainsKey(Kinect.JointType.ShoulderLeft) && this.Tree_BoneRads[1][i].ContainsKey(Kinect.JointType.ShoulderLeft))
                    {
                        this.BoneVels.Add(Kinect.JointType.ShoulderLeft, this.Tree_BoneRads[0][i][Kinect.JointType.ShoulderLeft] - this.Tree_BoneRads[1][i][Kinect.JointType.ShoulderLeft]);
                    }
                    if (this.Tree_BoneRads[0][i].ContainsKey(Kinect.JointType.ElbowRight) && this.Tree_BoneRads[1][i].ContainsKey(Kinect.JointType.ElbowRight))
                    {
                        this.BoneVels.Add(Kinect.JointType.ElbowRight, this.Tree_BoneRads[0][i][Kinect.JointType.ElbowRight] - this.Tree_BoneRads[1][i][Kinect.JointType.ElbowRight]);
                    }
                    if (this.Tree_BoneRads[0][i].ContainsKey(Kinect.JointType.ShoulderRight) && this.Tree_BoneRads[1][i].ContainsKey(Kinect.JointType.ShoulderRight))
                    {
                        this.BoneVels.Add(Kinect.JointType.ShoulderRight, this.Tree_BoneRads[0][i][Kinect.JointType.ShoulderRight] - this.Tree_BoneRads[1][i][Kinect.JointType.ShoulderRight]);
                    }
                    if (this.Tree_BoneRads[0][i].ContainsKey(Kinect.JointType.Neck) && this.Tree_BoneRads[1][i].ContainsKey(Kinect.JointType.Neck))
                    {
                        this.BoneVels.Add(Kinect.JointType.Neck, this.Tree_BoneRads[0][i][Kinect.JointType.Neck] - this.Tree_BoneRads[1][i][Kinect.JointType.Neck]);
                    }
                    if (this.Tree_BoneRads[0][i].ContainsKey(Kinect.JointType.KneeLeft) && this.Tree_BoneRads[1][i].ContainsKey(Kinect.JointType.KneeLeft))
                    {
                        this.BoneVels.Add(Kinect.JointType.KneeLeft, this.Tree_BoneRads[0][i][Kinect.JointType.KneeLeft] - this.Tree_BoneRads[1][i][Kinect.JointType.KneeLeft]);
                    }
                    if (this.Tree_BoneRads[0][i].ContainsKey(Kinect.JointType.HipLeft) && this.Tree_BoneRads[1][i].ContainsKey(Kinect.JointType.HipLeft))
                    {
                        this.BoneVels.Add(Kinect.JointType.HipLeft, this.Tree_BoneRads[0][i][Kinect.JointType.HipLeft] - this.Tree_BoneRads[1][i][Kinect.JointType.HipLeft]);
                    }
                    if (this.Tree_BoneRads[0][i].ContainsKey(Kinect.JointType.KneeRight) && this.Tree_BoneRads[1][i].ContainsKey(Kinect.JointType.KneeRight))
                    {
                        this.BoneVels.Add(Kinect.JointType.KneeRight, this.Tree_BoneRads[0][i][Kinect.JointType.KneeRight] - this.Tree_BoneRads[1][i][Kinect.JointType.KneeRight]);
                    }
                    if (this.Tree_BoneRads[0][i].ContainsKey(Kinect.JointType.HipRight) && this.Tree_BoneRads[1][i].ContainsKey(Kinect.JointType.HipRight))
                    {
                        this.BoneVels.Add(Kinect.JointType.HipRight, this.Tree_BoneRads[0][i][Kinect.JointType.HipRight] - this.Tree_BoneRads[1][i][Kinect.JointType.HipRight]);
                    }
                    if (this.Tree_BoneRads[0][i].ContainsKey(Kinect.JointType.SpineBase) && this.Tree_BoneRads[1][i].ContainsKey(Kinect.JointType.SpineBase))
                    {
                        this.BoneVels.Add(Kinect.JointType.SpineBase, this.Tree_BoneRads[0][i][Kinect.JointType.SpineBase] - this.Tree_BoneRads[1][i][Kinect.JointType.SpineBase]);
                    }

                    this.List_BoneVels.Add(new Dictionary<Kinect.JointType, double>(this.BoneVels));
                }
            }
            //角速度情報ツリーを更新
            if (this.Tree_BoneVels.Count == 0)
            {
                this.Tree_BoneVels.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneVels));
                this.Tree_BoneVels.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneVels));
                this.Tree_BoneVels.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneVels));
                this.Tree_BoneVels.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneVels));
                this.Tree_BoneVels.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneVels));
                this.Tree_BoneVels.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneVels));
            }
            this.Tree_BoneVels.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneVels));
            if (this.Tree_BoneVels.Count > 10) this.Tree_BoneVels.RemoveAt(this.Tree_BoneVels.Count - 1);

            #endregion



            //Boneの角加速度情報リスト作成
            #region boneAcc TreeUse
            for (int i = 0; i < this.Tree_BoneVels[0].Count; i++)
            {
                this.BoneAccs.Clear();
                if (this.Tree_BoneVels[1].Count > i)
                {
                    if (this.Tree_BoneVels[0][i].ContainsKey(Kinect.JointType.ElbowLeft) && this.Tree_BoneVels[1][i].ContainsKey(Kinect.JointType.ElbowLeft))
                    {

                        this.BoneAccs.Add(Kinect.JointType.ElbowLeft, this.Tree_BoneVels[0][i][Kinect.JointType.ElbowLeft] - this.Tree_BoneVels[1][i][Kinect.JointType.ElbowLeft]);
                    }
                    if (this.Tree_BoneVels[0][i].ContainsKey(Kinect.JointType.ShoulderLeft) && this.Tree_BoneVels[1][i].ContainsKey(Kinect.JointType.ShoulderLeft))
                    {
                        this.BoneAccs.Add(Kinect.JointType.ShoulderLeft, this.Tree_BoneVels[0][i][Kinect.JointType.ShoulderLeft] - this.Tree_BoneVels[1][i][Kinect.JointType.ShoulderLeft]);
                    }
                    if (this.Tree_BoneVels[0][i].ContainsKey(Kinect.JointType.ElbowRight) && this.Tree_BoneVels[1][i].ContainsKey(Kinect.JointType.ElbowRight))
                    {
                        this.BoneAccs.Add(Kinect.JointType.ElbowRight, this.Tree_BoneVels[0][i][Kinect.JointType.ElbowRight] - this.Tree_BoneVels[1][i][Kinect.JointType.ElbowRight]);
                    }
                    if (this.Tree_BoneVels[0][i].ContainsKey(Kinect.JointType.ShoulderRight) && this.Tree_BoneVels[1][i].ContainsKey(Kinect.JointType.ShoulderRight))
                    {
                        this.BoneAccs.Add(Kinect.JointType.ShoulderRight, this.Tree_BoneVels[0][i][Kinect.JointType.ShoulderRight] - this.Tree_BoneVels[1][i][Kinect.JointType.ShoulderRight]);
                    }
                    if (this.Tree_BoneVels[0][i].ContainsKey(Kinect.JointType.Neck) && this.Tree_BoneVels[1][i].ContainsKey(Kinect.JointType.Neck))
                    {
                        this.BoneAccs.Add(Kinect.JointType.Neck, this.Tree_BoneVels[0][i][Kinect.JointType.Neck] - this.Tree_BoneVels[1][i][Kinect.JointType.Neck]);
                    }
                    if (this.Tree_BoneVels[0][i].ContainsKey(Kinect.JointType.KneeLeft) && this.Tree_BoneVels[1][i].ContainsKey(Kinect.JointType.KneeLeft))
                    {
                        this.BoneAccs.Add(Kinect.JointType.KneeLeft, this.Tree_BoneVels[0][i][Kinect.JointType.KneeLeft] - this.Tree_BoneVels[1][i][Kinect.JointType.KneeLeft]);
                    }
                    if (this.Tree_BoneVels[0][i].ContainsKey(Kinect.JointType.HipLeft) && this.Tree_BoneVels[1][i].ContainsKey(Kinect.JointType.HipLeft))
                    {
                        this.BoneAccs.Add(Kinect.JointType.HipLeft, this.Tree_BoneVels[0][i][Kinect.JointType.HipLeft] - this.Tree_BoneVels[1][i][Kinect.JointType.HipLeft]);
                    }
                    if (this.Tree_BoneVels[0][i].ContainsKey(Kinect.JointType.KneeRight) && this.Tree_BoneVels[1][i].ContainsKey(Kinect.JointType.KneeRight))
                    {
                        this.BoneAccs.Add(Kinect.JointType.KneeRight, this.Tree_BoneVels[0][i][Kinect.JointType.KneeRight] - this.Tree_BoneVels[1][i][Kinect.JointType.KneeRight]);
                    }
                    if (this.Tree_BoneVels[0][i].ContainsKey(Kinect.JointType.HipRight) && this.Tree_BoneVels[1][i].ContainsKey(Kinect.JointType.HipRight))
                    {
                        this.BoneAccs.Add(Kinect.JointType.HipRight, this.Tree_BoneVels[0][i][Kinect.JointType.HipRight] - this.Tree_BoneVels[1][i][Kinect.JointType.HipRight]);
                    }
                    if (this.Tree_BoneVels[0][i].ContainsKey(Kinect.JointType.SpineBase) && this.Tree_BoneVels[1][i].ContainsKey(Kinect.JointType.SpineBase))
                    {
                        this.BoneAccs.Add(Kinect.JointType.SpineBase, this.Tree_BoneVels[0][i][Kinect.JointType.SpineBase] - this.Tree_BoneVels[1][i][Kinect.JointType.SpineBase]);
                    }

                    this.List_BoneAccs.Add(new Dictionary<Kinect.JointType, double>(this.BoneAccs));
                }
            }
            //角加速度情報ツリーを更新
            if (this.Tree_BoneAccs.Count == 0)
            {
                this.Tree_BoneAccs.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneAccs));
                this.Tree_BoneAccs.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneAccs));
                this.Tree_BoneAccs.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneAccs));
                this.Tree_BoneAccs.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneAccs));
                this.Tree_BoneAccs.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneAccs));
            }
            this.Tree_BoneAccs.Insert(0, new List<Dictionary<Kinect.JointType, double>>(this.List_BoneAccs));
            if (this.Tree_BoneAccs.Count > 10) this.Tree_BoneAccs.RemoveAt(this.Tree_BoneAccs.Count - 1);

            #endregion

            //SpineBase速度ツリーを作成
            this.nowBaseVels.Clear();
            for (int i = 0; i < this.Tree_BasePos[0].Count; ++i)
            {
                if (this.Tree_BasePos[1].Count > i)
                {
                    this.nowBaseVels.Add(this.Tree_BasePos[0][i] - this.Tree_BasePos[1][i]);
                }
                else
                {
                    this.nowBaseVels.Add(0);
                }
            }
            //SpineBase速度ツリーを作成
            if (this.Tree_BaseVel.Count == 0)
            {
                this.Tree_BaseVel.Insert(0, new List<float>(this.nowBaseVels));
                this.Tree_BaseVel.Insert(0, new List<float>(this.nowBaseVels));
                this.Tree_BaseVel.Insert(0, new List<float>(this.nowBaseVels));
                this.Tree_BaseVel.Insert(0, new List<float>(this.nowBaseVels));
                this.Tree_BaseVel.Insert(0, new List<float>(this.nowBaseVels));
            }
            this.Tree_BaseVel.Insert(0, new List<float>(this.nowBaseVels));
            if (this.Tree_BaseVel.Count > 10) this.Tree_BaseVel.RemoveAt(this.Tree_BaseVel.Count - 1);


            for (int i = 0; i < this.contour_Center.Count; i++)
            {
                //Cv2.Circle(m_buffer, contour_Center[i], 3, new Scalar(255, 255, 0), 2);
            }


            //取得した輪郭について1つずつ処理
            this.List_Contours.Clear();

            for (int i = 0; i < bodyContList.Count; i++)
            {
                //Cv2.Line(m_buffer, JointToDepthPoint(this.bodyContList[i][0].bodyNum, Windows.Kinect.JointType.SpineBase), this.contour_Center[this.bodyContList[i][0].contourNum], new Scalar(255, 200, 100));
                this.CvPoints.Clear();

                //入れ替える対象のナンバー
                targetContNum = i + 1;
                if (targetContNum == List_BoneRads.Count) targetContNum = 0;

                //加速度の入れ替え対象ナンバー
                targetAccContNum = i + 1;
                if (targetAccContNum >= List_BoneAccs.Count) targetAccContNum = 0;

                //傾きの入れ替え対象ナンバー
                targetAttContNum = i + 1;
                if (targetAttContNum >= Tree_BaseVel[0].Count) targetAttContNum = 0;

                this.SwapBodyMats.Clear();



                //回転の動きを大きくする影　　　　　　
                #region make rot mat
                if (this.useRot)
                {

                    //SpinBase
                    if (this.List_BoneRads[i].ContainsKey(Kinect.JointType.SpineBase) == true)
                    {
                        this.SwapBodyMats.Add(Kinect.JointType.SpineBase, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.SpineBase, this.List_BoneRads[i][Kinect.JointType.SpineBase] * -2));
                        //Debug.Log("countNum : " + this.List_BoneRads[i].Count);

                    }
                    else
                    {
                        this.SwapBodyMats.Add(Kinect.JointType.SpineBase, Mat.Eye(3, 3, MatType.CV_32FC1));
                    }
                    //LeftShoulder
                    if (this.List_BoneRads[i].ContainsKey(Kinect.JointType.ShoulderLeft) == true && List_BoneRads[i].ContainsKey(Kinect.JointType.SpineBase) == true)
                    {
                        if (-this.List_BoneRads[i][Kinect.JointType.SpineBase] > 0)
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ShoulderLeft, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.ShoulderLeft, -this.List_BoneRads[i][Kinect.JointType.SpineBase], SwapBodyMats[Kinect.JointType.SpineBase]));

                        }
                        else
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ShoulderLeft, SwapBodyMats[Kinect.JointType.SpineBase]);
                        }

                    }
                    else
                    {
                        this.SwapBodyMats.Add(Kinect.JointType.ShoulderLeft, this.SwapBodyMats[Kinect.JointType.SpineBase]);
                    }
                    //LeftElbow
                    this.SwapBodyMats.Add(Kinect.JointType.ElbowLeft, this.SwapBodyMats[Kinect.JointType.ShoulderLeft]);
                    //RightShoulder
                    if (this.List_BoneRads[i].ContainsKey(Kinect.JointType.ShoulderRight) == true && this.List_BoneRads[i].ContainsKey(Kinect.JointType.SpineBase) == true)
                    {
                        if (-this.List_BoneRads[i][Kinect.JointType.SpineBase] <= 0)
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ShoulderRight, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.ShoulderRight, -this.List_BoneRads[i][Kinect.JointType.SpineBase], SwapBodyMats[Kinect.JointType.SpineBase]));
                        }
                        else
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ShoulderRight, this.SwapBodyMats[Kinect.JointType.SpineBase]);
                        }
                    }
                    else
                    {
                        this.SwapBodyMats.Add(Kinect.JointType.ShoulderRight, this.SwapBodyMats[Kinect.JointType.SpineBase]);
                    }
                    //RightElbow
                    this.SwapBodyMats.Add(Kinect.JointType.ElbowRight, this.SwapBodyMats[Kinect.JointType.ShoulderRight]);
                    //LeftHip
                    if (this.List_BoneRads[i].ContainsKey(Kinect.JointType.HipLeft) == true && this.List_BoneRads[i].ContainsKey(Kinect.JointType.SpineBase) == true)
                    {
                        if (-this.List_BoneRads[i][Kinect.JointType.SpineBase] > 0) //状況を見て符号は変える
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.HipLeft, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.HipLeft, -2 * this.List_BoneRads[i][Kinect.JointType.SpineBase]));
                        }
                        else
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.HipLeft, Mat.Eye(3, 3, MatType.CV_32FC1));
                        }
                    }
                    else
                    {
                        this.SwapBodyMats.Add(Kinect.JointType.HipLeft, Mat.Eye(3, 3, MatType.CV_32FC1));
                    }
                    //RightHip
                    if (this.List_BoneRads[i].ContainsKey(Kinect.JointType.HipRight) == true && this.List_BoneRads[i].ContainsKey(Kinect.JointType.SpineBase) == true)
                    {
                        if (-this.List_BoneRads[i][Kinect.JointType.SpineBase] <= 0) //状況を見て符号は変える
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.HipRight, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.HipRight, -2 * this.List_BoneRads[i][Kinect.JointType.SpineBase]));
                        }
                        else
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.HipRight, Mat.Eye(3, 3, MatType.CV_32FC1));
                        }
                    }
                    else
                    {
                        this.SwapBodyMats.Add(Kinect.JointType.HipRight, Mat.Eye(3, 3, MatType.CV_32FC1));
                    }
                }
                #endregion

                //微分を用いて傾きが先行する影　
                #region make Pre　Rot mat
                if (this.usePre)
                {
                    Debug.Log("this.List_BoneVels.Count ; " + this.List_BoneVels.Count);
                    if (this.List_BoneVels.Count > i)
                    {
                        //SpinBase
                        if (this.List_BoneVels[i].ContainsKey(Kinect.JointType.SpineBase) == true)
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.SpineBase, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.SpineBase, this.List_BoneVels[i][Kinect.JointType.SpineBase] * this.preRotRate));
                        }
                        else
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.SpineBase, Mat.Eye(3, 3, MatType.CV_32FC1));
                        }
                        //ShoulderLeft
                        if (this.List_BoneVels[i].ContainsKey(Kinect.JointType.ShoulderLeft) == true)
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ShoulderLeft, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.ShoulderLeft, -this.List_BoneVels[i][Kinect.JointType.ShoulderLeft] * this.preRotRate, SwapBodyMats[Kinect.JointType.SpineBase]));
                        }
                        else
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ShoulderLeft, this.SwapBodyMats[Kinect.JointType.SpineBase]);
                        }
                        //ElbowLeft
                        if (this.List_BoneVels[i].ContainsKey(Kinect.JointType.ElbowLeft) == true)
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ElbowLeft, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.ElbowLeft, -this.List_BoneVels[i][Kinect.JointType.ElbowLeft] * this.preRotRate, SwapBodyMats[Kinect.JointType.ShoulderLeft]));
                        }
                        else
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ElbowLeft, this.SwapBodyMats[Kinect.JointType.ShoulderLeft]);
                        }
                        //ShoulderRight
                        if (this.List_BoneVels[i].ContainsKey(Kinect.JointType.ShoulderRight) == true)
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ShoulderRight, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.ShoulderRight, -this.List_BoneVels[i][Kinect.JointType.ShoulderRight], SwapBodyMats[Kinect.JointType.SpineBase]));
                        }
                        else
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ShoulderRight, this.SwapBodyMats[Kinect.JointType.SpineBase]);
                        }
                        //ElbowRight
                        if (this.List_BoneVels[i].ContainsKey(Kinect.JointType.ElbowRight) == true)
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ElbowRight, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.ElbowRight, -this.List_BoneVels[i][Kinect.JointType.ElbowRight], SwapBodyMats[Kinect.JointType.ShoulderRight]));
                        }
                        else
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.ElbowRight, this.SwapBodyMats[Kinect.JointType.ShoulderRight]);
                        }
                        //HipLeft
                        if (this.List_BoneVels[i].ContainsKey(Kinect.JointType.HipLeft) == true && this.List_BoneRads[i].ContainsKey(Kinect.JointType.SpineBase) == true)
                        {
                            if (-this.List_BoneRads[i][Kinect.JointType.SpineBase] > 0) //状況を見て符号は変える
                            {
                                this.SwapBodyMats.Add(Kinect.JointType.HipLeft, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.HipLeft, -this.List_BoneVels[i][Kinect.JointType.HipLeft] * this.preRotRate));
                            }
                            else
                            {
                                this.SwapBodyMats.Add(Kinect.JointType.HipLeft, Mat.Eye(3, 3, MatType.CV_32FC1));
                            }
                        }
                        else
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.HipLeft, Mat.Eye(3, 3, MatType.CV_32FC1));
                        }
                        //HipRight
                        if (this.List_BoneVels[i].ContainsKey(Kinect.JointType.HipRight) == true && this.List_BoneRads[i].ContainsKey(Kinect.JointType.SpineBase) == true)
                        {
                            if (-this.List_BoneRads[i][Kinect.JointType.SpineBase] <= 0) //状況を見て符号は変える
                            {
                                this.SwapBodyMats.Add(Kinect.JointType.HipRight, GetRotateMat(this.bodyContList[i][0].bodyNum, Kinect.JointType.HipRight, -this.List_BoneRads[i][Kinect.JointType.HipRight] * this.preRotRate));
                            }
                            else
                            {
                                this.SwapBodyMats.Add(Kinect.JointType.HipRight, Mat.Eye(3, 3, MatType.CV_32FC1));
                            }
                        }
                        else
                        {
                            this.SwapBodyMats.Add(Kinect.JointType.HipRight, Mat.Eye(3, 3, MatType.CV_32FC1));
                        }
                    }

                }
                #endregion


                //何もしないとき、空のmatを作成
          
                #region make empty mat
                if (SwapBodyMats.Count == 0)
                {
                    //SpinBase
                    SwapBodyMats.Add(Kinect.JointType.SpineBase, Mat.Eye(3, 3, MatType.CV_32FC1));
                    //LeftShoulder
                    SwapBodyMats.Add(Kinect.JointType.ShoulderLeft, Mat.Eye(3, 3, MatType.CV_32FC1));
                    //LeftElbow
                    SwapBodyMats.Add(Kinect.JointType.ElbowLeft, Mat.Eye(3, 3, MatType.CV_32FC1));
                    //RightShoulder
                    SwapBodyMats.Add(Kinect.JointType.ShoulderRight, Mat.Eye(3, 3, MatType.CV_32FC1));
                    //RightElbow
                    SwapBodyMats.Add(Kinect.JointType.ElbowRight, Mat.Eye(3, 3, MatType.CV_32FC1));
                    //LeftHip
                    SwapBodyMats.Add(Kinect.JointType.HipLeft, Mat.Eye(3, 3, MatType.CV_32FC1));
                    //RightHip
                    SwapBodyMats.Add(Kinect.JointType.HipRight, Mat.Eye(3, 3, MatType.CV_32FC1));
                }
                #endregion


                //輪郭"点"について1つずつ処理
                for (int j = 0; j < this.bodyContList[i].Count; j++)  //順番
                {
                    if (this.Tree_BaseVel[0].Count > i)
                    {
                        this.baseMoveAve = GetBaseMoveAve(this.Tree_BaseVel,i , 5);
                    }
                    else
                    {
                        this.baseMoveAve = 0;
                    }

                    #region  move Contour
                    switch (bodyContList[i][j].nearJt1)
                    {
                        case Kinect.JointType.AnkleLeft:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, SwapBodyMats[Kinect.JointType.HipLeft]);
                            bodyContList[i][j].center += new Point(this.baseMoveAve * this.preMoveRate * 
                                                                  (BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.AnkleLeft].position.y - bodyContList[i][j].center.Y) /
                                                                  (BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.AnkleLeft].position.y - BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.SpineBase].position.y), 0);

                            break;

                        case Kinect.JointType.KneeLeft:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, SwapBodyMats[Kinect.JointType.HipLeft]);
                            bodyContList[i][j].center += new Point(this.baseMoveAve * this.preMoveRate *
                                                                  (BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.AnkleLeft].position.y - bodyContList[i][j].center.Y) /
                                                                  (BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.AnkleLeft].position.y - BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.SpineBase].position.y), 0);

                            break;

                        case Kinect.JointType.AnkleRight:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, SwapBodyMats[Kinect.JointType.HipRight]);
                            bodyContList[i][j].center += new Point(this.baseMoveAve * this.preMoveRate *
                                                                 (BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.AnkleRight].position.y - bodyContList[i][j].center.Y) /
                                                                 (BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.AnkleRight].position.y - BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.SpineBase].position.y), 0);

                            break;

                        case Kinect.JointType.KneeRight:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, SwapBodyMats[Kinect.JointType.HipRight]);
                            bodyContList[i][j].center += new Point(this.baseMoveAve * this.preMoveRate *
                                                                  (BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.AnkleRight].position.y - bodyContList[i][j].center.Y) /
                                                                  (BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.AnkleRight].position.y - BodyDataOnDepthImage[bodyContList[i][0].bodyNum].JointDepth[Kinect.JointType.SpineBase].position.y), 0);


                            break;

                        case Kinect.JointType.WristLeft:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, SwapBodyMats[Kinect.JointType.ElbowLeft]);
                            bodyContList[i][j].center += new Point(this.baseMoveAve * this.preMoveRate, 0);

                            break;

                        case Kinect.JointType.ElbowLeft:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, SwapBodyMats[Kinect.JointType.ShoulderLeft]);
                            bodyContList[i][j].center += new Point(this.baseMoveAve * this.preMoveRate, 0);

                            break;

                        case Kinect.JointType.WristRight:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, SwapBodyMats[Kinect.JointType.ElbowRight]);
                            bodyContList[i][j].center += new Point(this.baseMoveAve * this.preMoveRate, 0);

                            break;

                        case Kinect.JointType.ElbowRight:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, SwapBodyMats[Kinect.JointType.ShoulderRight]);
                            bodyContList[i][j].center += new Point(this.baseMoveAve * this.preMoveRate, 0);
                            break;

                        case Kinect.JointType.SpineBase:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, SwapBodyMats[Kinect.JointType.SpineBase]);
                            bodyContList[i][j].center += new Point(this.baseMoveAve * this.preMoveRate, 0);

                            break;

                        case Kinect.JointType.Head:
                            bodyContList[i][j].center = ContourRotate(bodyContList[i][j], 0, SwapBodyMats[Kinect.JointType.SpineBase]);
                            bodyContList[i][j].center += new Point(this.baseMoveAve * this.preMoveRate, 0);
                            break;

                        default:

                            break;
                            #endregion

                    }
                    


                    this.CvPoints.Add(bodyContList[i][j].center);
                }

                this.List_Contours.Add(new List<Point>(CvPoints));
            }


            //var _contour = List_Contours.ToArray();
            _contour = List_Contours.ToArray();
            Cv2.DrawContours(m_cont_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);


            Cv2.CvtColor(m_cont_buffer, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            Cv2.DrawContours(m_buffer, contour, -1, color, -1, OpenCvSharp.LineType.Link8);



            this.List_Contours_Buffer = this.List_Contours;

            this.count += (0.01);
            if (this.count > 2 * Math.PI) this.count = 0;

            //チェック用
            if (this.count == 0)
            {
                Debug.Log(this.List_BoneAccs.Count);

            }

            //Cv2.CvtColor(dstMat, dst, OpenCvSharp.ColorConversion.BgraToBgr);
            //dst += m_buffer;

            m_second_buffer = m_buffer.Clone();
            if (this.addImg)
            {
                m_first_buffer += m_second_buffer;
                dst += m_first_buffer;
            }
            //加算合成ver.
            else
            {
                dst += m_buffer;
            }
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
        private Mat GetRotateMat(int bodyNum, Kinect.JointType pivotJt, double rad, Point trans)
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

            float[,] transMatArr = { {1,0,trans.X },
                                     {0,1,trans.Y  },
                                     {0,0,1 } };
            Mat transMat = new Mat(3, 3, MatType.CV_32FC1, transMatArr);


            return transMat * backOriMat * rotMat * goOriMat;
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

        double GetBoneAccAve(List<List<Dictionary<Kinect.JointType, double>>> accList, int bodyNum, Kinect.JointType jt, int AveNum)
        {
            double Average = 0;
            int count = 0;
            for (int a = 0; a < AveNum; a++)
            {
                if (accList[a].Count > bodyNum)
                {
                    if (accList[a][bodyNum].ContainsKey(jt))
                    {
                        Average += accList[a][bodyNum][jt];
                    }
                    else
                    {
                        count++;
                    }
                }
                else
                {
                    count++;
                }

            }
            if (count == AveNum) count--;
            return Average / (AveNum - count);
        }


        float GetBaseMoveAve(List<List<float>> baseTree, int bodyNum, int AveNum)
        {
            float Average = 0;
            int count = 0;
            for (int a = 0; a < AveNum; a++)
            {
                if (baseTree[a].Count > bodyNum)
                {
                    Average += baseTree[a][bodyNum];
                }
                else
                {
                    count++;
                }
            }
            if (count == AveNum) count--;
            return Average / (AveNum - count);
        }

        double GetBoneRadAve(List<Dictionary<Kinect.JointType, double>> boneRadList, Kinect.JointType jt)
        {
            double Average = 0;
            int count = 0;
            if (boneRadList.Count != 0)
            {
                for (int a = 0; a < boneRadList.Count; a++)
                {
                    if (boneRadList[a].ContainsKey(jt))
                    {
                        Average += boneRadList[a][jt];
                    }
                    else
                    {
                        count++;
                    }
                }
                if (count == boneRadList.Count) count--;
                return Average / (boneRadList.Count - count);

            }
            else
            {
                return 0;
            }

        }

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

        public double GetBoneRad(int bodyNum, Kinect.JointType spineBase)
        {
            //内積から角度を計算  vec(0,-1 ) との内積を求める

            double baseRad = Math.Acos(-1 * (JointToDepthPoint(bodyNum, _BoneMap[spineBase]).Y - JointToDepthPoint(bodyNum, spineBase).Y) /
                                       PointsLength(JointToDepthPoint(bodyNum, spineBase), JointToDepthPoint(bodyNum, _BoneMap[spineBase])));

            //det = ax * by - ay * bx = - or +  どっち向きかの判断
            //det = 0 * by - (-1)* bx = bx
            if (JointToDepthPoint(bodyNum, spineBase).X - JointToDepthPoint(bodyNum, _BoneMap[spineBase]).X < 0)
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
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x > srcMat.Width) Debug.Log("OverNum : "  + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x); 
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x < 0) bl = false;
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x < 0 ) Debug.Log("OverNum : " + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x);
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y > srcMat.Height) bl = false;
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y > srcMat.Height) Debug.Log("OverNum : " + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y);
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y < 0) bl = false;
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y < 0) Debug.Log("OverNum : " + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y);
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

                //画面内にあるものも消してしまいそう↓
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].state == Kinect.TrackingState.NotTracked)
                //{
                //    Debug.Log("UntrackingBone : " + _TrackigBoneList[i]);
                //}

            }

            return bl;
        }


        public class BodyContor   //すべての輪郭点のクラス
        {

            public OpenCvSharp.CPlusPlus.Point center { get; set; }
            public int contourNum { get; set; }
            public int bodyNum { get; set; }
            public Windows.Kinect.JointType nearJt1 { get; set; }
            //Windows.Kinect.JointType nearJt2 { get; set; }


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
            return "Ahead";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Ahead;
        }

        public bool IsFirstFrame { get; private set; }
    }
}

