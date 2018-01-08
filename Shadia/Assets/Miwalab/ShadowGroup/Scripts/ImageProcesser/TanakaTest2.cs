using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;
using UnityEngine;
using System.IO;
using Windows.Kinect;


namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class TanakaTest2 : AShadowImageProcesser
    {
        #region //値宣言
        //ループ
        private int i,j,k;

        //リスト
        List<Mat> ListMat;
        List<float> ListTime;
        private int ListMax = 1000;

        //FPS
        private float fps;
        private float NextTimeFPS;
        private int framecount = 0;

        //表示
        private bool AddNow;
        private bool Gradually;
        private bool Jikken;
        private bool jikken_Invert;
        private bool Stop;
        Mat stopitem = new Mat();
        private bool flip;
        private bool ColorInvert;

        //Delay処理
        private float DelayTime;
        private int DelayCounter;
        private float TrueDT;

        //DT_Rand
        private bool DT_Random;
        private float PreTimeRand;
        private float RandTime;
        private float TargetDelayTime;
        private int RandMaxTDT;

        //DT_Interactive
        private bool DT_Interactive;
        private bool Int_Invert;
        private float PreTimeInt;
        private float IntTime;
        private int ListTimeCounter = 0;

        //SolveNDT
        private float Gradient;
        private int Threshold;
        private float PitchDT;

        //差分画像
        Mat diffimage = new Mat();
        private int CountWhite;
        private int SumCW = 0;
        private int AveCW = 0;
        private int OldAveCW;

        //ChandeDT
        private int Counter = 0;
        private int pitchTP;
        private int pitchTM;

        //kinect関連
        List<Body> trackedBodyData; //body型を収納するリスト
        private int Humannum;
        private string SpineData;
        private bool boolGetSpine;

        //AddDelay2
        private bool AddDelay2;
        private float DelayTime2;
        private int DelayCounter2;

        //Save 
        private bool LogFPS;
        private bool LogAveCW;
        private bool LogTDT;
        private bool LogDC;
        private bool Save;
        private int intAddNow;
        private int intAddDelay2;
        private string FileName;

        Mat newitem = new Mat();
        #endregion

        public TanakaTest2()
                : base()
        {
            //フレームレート計算の初期設定
            NextTimeFPS = Time.time + 1;
            this.ListMat = new List<Mat>();
            this.ListTime = new List<float>();
            //目標フレームレートの設定
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;

            #region //UI関連
            //(ShadowMediaUIHost.GetUI("TT2_DelayCounter") as ParameterSlider).ValueChanged += TT2_DelayCounter;
            (ShadowMediaUIHost.GetUI("TT2_DelayTime[s/100]") as ParameterSlider).ValueChanged += TT2_DelayTime;
            (ShadowMediaUIHost.GetUI("TT2_ColorInvert") as ParameterCheckbox).ValueChanged += TT2_ColorInvert;
            (ShadowMediaUIHost.GetUI("TT2_Flip") as ParameterCheckbox).ValueChanged += TT2_Flip;
            (ShadowMediaUIHost.GetUI("TT2_AddNow") as ParameterCheckbox).ValueChanged += TT2_AddNow;
            (ShadowMediaUIHost.GetUI("TT2_AddDelay2") as ParameterCheckbox).ValueChanged += TT2_AddDelay2;
            (ShadowMediaUIHost.GetUI("TT2_DelayTime2[s/100]") as ParameterSlider).ValueChanged += TT2_DelayTime2;
            (ShadowMediaUIHost.GetUI("TT2_Stop") as ParameterCheckbox).ValueChanged += TT2_Stop;
            (ShadowMediaUIHost.GetUI("TT2_Gradually") as ParameterCheckbox).ValueChanged += TT2_Gradually;
            (ShadowMediaUIHost.GetUI("TT2_TargetDelayTime[s/100]") as ParameterSlider).ValueChanged += TT2_TargetDelayTime;
            (ShadowMediaUIHost.GetUI("TT2_Jikken") as ParameterCheckbox).ValueChanged += TT2_Jikken;
            (ShadowMediaUIHost.GetUI("TT2_Jikken_Invert") as ParameterCheckbox).ValueChanged += TT2_Jikken_Invert;
            (ShadowMediaUIHost.GetUI("TT2_PitchTimePlus") as ParameterSlider).ValueChanged += TT2_PitchTP;
            (ShadowMediaUIHost.GetUI("TT2_PitchTimeMinus") as ParameterSlider).ValueChanged += TT2_PitchTM;
            (ShadowMediaUIHost.GetUI("TT2_DT_Random") as ParameterCheckbox).ValueChanged += TT2_DT_Random;
            (ShadowMediaUIHost.GetUI("TT2_MaxTargetDT[s/100]") as ParameterSlider).ValueChanged += TT2_MaxTargetDT;
            (ShadowMediaUIHost.GetUI("TT2_RandTime[s/100]") as ParameterSlider).ValueChanged += TT2_RandTime;
            (ShadowMediaUIHost.GetUI("TT2_DT_Interactive") as ParameterCheckbox).ValueChanged += TT2_DT_Interactive;
            (ShadowMediaUIHost.GetUI("TT2_DT_Int_Invert") as ParameterCheckbox).ValueChanged += TT2_DT_Int_Invert;
            (ShadowMediaUIHost.GetUI("TT2_Int_Threshold") as ParameterSlider).ValueChanged += TT2_Int_Threshold;
            (ShadowMediaUIHost.GetUI("TT2_Int_pitchDT") as ParameterSlider).ValueChanged += TT2_Int_pitchDT;
            (ShadowMediaUIHost.GetUI("TT2_IntTime[s/100]") as ParameterSlider).ValueChanged += TT2_IntTime;
            (ShadowMediaUIHost.GetUI("TT2_LogFPS") as ParameterCheckbox).ValueChanged += TT2_LogFPS;
            (ShadowMediaUIHost.GetUI("TT2_LogAveCW") as ParameterCheckbox).ValueChanged += TT2_LogAveCW;
            (ShadowMediaUIHost.GetUI("TT2_LogTDT") as ParameterCheckbox).ValueChanged += TT2_LogTDT;
            (ShadowMediaUIHost.GetUI("TT2_LogDC") as ParameterCheckbox).ValueChanged += TT2_LogDC;
            (ShadowMediaUIHost.GetUI("TT2_GetSpine") as ParameterCheckbox).ValueChanged += TT2_GetSpine;
            (ShadowMediaUIHost.GetUI("TT2_DataSave") as ParameterCheckbox).ValueChanged += TT2_DataSave;
            (ShadowMediaUIHost.GetUI("TT2_0.3") as ParameterButton).Clicked += TT2_030;
            (ShadowMediaUIHost.GetUI("TT2_0.5") as ParameterButton).Clicked += TT2_050;
            (ShadowMediaUIHost.GetUI("TT2_1.0") as ParameterButton).Clicked += TT2_100;
            (ShadowMediaUIHost.GetUI("TT2_3.0") as ParameterButton).Clicked += TT2_300;
            (ShadowMediaUIHost.GetUI("TT2_5.0") as ParameterButton).Clicked += TT2_500;
            (ShadowMediaUIHost.GetUI("TT2_10.0") as ParameterButton).Clicked += TT2_1000;
            (ShadowMediaUIHost.GetUI("TT2_20.0") as ParameterButton).Clicked += TT2_2000;
            (ShadowMediaUIHost.GetUI("TT2_30.0") as ParameterButton).Clicked += TT2_3000;
            #endregion
        }

        #region //UI関連
        private void TT2_DelayTime(object sender, EventArgs e)
        {
            DelayTime = (int)(e as ParameterSlider.ChangedValue).Value;
            DelayTime = DelayTime / 100f;
        }
        private void TT2_ColorInvert(object sender, EventArgs e)
        {
            this.ColorInvert = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_Flip(object sender, EventArgs e)
        {
            this.flip = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_AddNow(object sender, EventArgs e)
        {
            this.AddNow = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_AddDelay2(object sender, EventArgs e)
        {
            this.AddDelay2 = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_DelayTime2(object sender, EventArgs e)
        {
            DelayTime2 = (int)(e as ParameterSlider.ChangedValue).Value;
            DelayTime2 = DelayTime2 / 100f;
        }
        private void TT2_Gradually(object sender, EventArgs e)
        {
            this.Gradually = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_Stop(object sender, EventArgs e)
        {
            this.Stop = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_TargetDelayTime(object sender, EventArgs e)
        {
            TargetDelayTime = (int)(e as ParameterSlider.ChangedValue).Value;
            TargetDelayTime = TargetDelayTime / 100f;
        }
        private void TT2_Jikken_Invert(object sender, EventArgs e)
        {
            this.jikken_Invert = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_Jikken(object sender, EventArgs e)
        {
            this.Jikken = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_PitchTM(object sender, EventArgs e)
        {
            pitchTM = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TT2_PitchTP(object sender, EventArgs e)
        {
            pitchTP = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TT2_DT_Random(object sender, EventArgs e)
        {
            this.DT_Random = (e as ParameterCheckbox.ChangedValue).Value;
            PreTimeRand = Time.time;
            if (DT_Interactive) Debug.Log("!Warning! DT_interactive_is_Checking" + PreTimeRand);
        }
        private void TT2_MaxTargetDT(object sender, EventArgs e)
        {
            RandMaxTDT = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TT2_RandTime(object sender, EventArgs e)
        {
            RandTime = (int)(e as ParameterSlider.ChangedValue).Value;
            RandTime = RandTime / 100f;
        }
        private void TT2_DT_Interactive(object sender, EventArgs e)
        {
            this.DT_Interactive = (e as ParameterCheckbox.ChangedValue).Value;
            PreTimeInt = Time.time;
            if (DT_Random) Debug.Log("!Warning! DT_random_is_Checking" + PreTimeInt);
        }
        private void TT2_DT_Int_Invert(object sender, EventArgs e)
        {
            this.Int_Invert = (e as ParameterCheckbox.ChangedValue).Value;

        }
        private void TT2_Int_pitchDT(object sender, EventArgs e)
        {
            PitchDT = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TT2_Int_Threshold(object sender, EventArgs e)
        {
            Threshold = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TT2_IntTime(object sender, EventArgs e)
        {
            IntTime = (int)(e as ParameterSlider.ChangedValue).Value;
            IntTime = IntTime / 100f;
        }
        private void TT2_LogFPS(object sender, EventArgs e)
        {
            this.LogFPS = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_LogTDT(object sender, EventArgs e)
        {
            this.LogTDT = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_LogAveCW(object sender, EventArgs e)
        {
            this.LogAveCW = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_LogDC(object sender, EventArgs e)
        {
            this.LogDC = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TT2_GetSpine(object sender, EventArgs e)
        {
            this.boolGetSpine = (e as ParameterCheckbox.ChangedValue).Value;
            if (!boolGetSpine) SpineData = ("");
        }
        private void TT2_DataSave(object sender, EventArgs e)
        {
            this.Save = (e as ParameterCheckbox.ChangedValue).Value;
            if (Save)
            {
                FileName = DateTime.Now.ToString("yy_MM_dd_HH_mm_ss");
                DataSave("1Time,2FPS,3AddNow,4AddDelay2,5DelayTime2,6pitchTP,7pitchTM,8pitchF,9AveCW,10Threshold,11NextDelayCounter,12TargetDelayTime,13DelayCounter,14DelayTime(DC/fps),15DelayTime,16HumanNum,17SpineX,18SpineZ,19HumanNum,20SpineX,21SpineZ,22HumanNum,23SpineX,24SpineZ");
            }
        }
        private void TT2_3000(object sender, EventArgs e)
        {
            DelayTime = 30.0f;
        }
        private void TT2_2000(object sender, EventArgs e)
        {
            DelayTime = 20.0f;
        }
        private void TT2_1000(object sender, EventArgs e)
        {
            DelayTime = 10.0f;
        }
        private void TT2_500(object sender, EventArgs e)
        {
            DelayTime = 5.0f;
        }
        private void TT2_300(object sender, EventArgs e)
        {
            DelayTime = 3.0f;
        }
        private void TT2_100(object sender, EventArgs e)
        {
            DelayTime = 1.0f;
        }
        private void TT2_050(object sender, EventArgs e)
        {
            DelayTime = 0.50f;
        }
        private void TT2_030(object sender, EventArgs e)
        {
            DelayTime = 0.30f;
        }
        #endregion

        // Update is called once per frame
        private void Update(ref Mat src, ref Mat dst)
        {
            //フレームレート
            FrameRate();
                        
            //リスト
            Mat item = new Mat();
            src.CopyTo(item); //srcをitemにコピー
            this.ListMat.Insert(0, item); //リストの先頭にitemを追加
            if (ListMat.Count > ListMax) this.ListMat.RemoveAt(ListMax - 1); //リストがListMaxを超えたら古いもの(末尾)から削除
            
            this.ListTime.Insert(0, Time.time); //リストの先頭にTime.timeを追加
            if (ListTime.Count > ListMax) this.ListTime.RemoveAt(ListMax - 1); //リストがListMaxを超えたら古いもの(末尾)から削除
            
            //実験
            if (Jikken) JikkenDelayTime(); 

            //ランダム
            if (DT_Random) RandomDelayTime(RandTime);

            //インタラクティブ
            if (DT_Interactive) InteractiveDelayTime(IntTime);

            if (Stop)
            {
                stopitem.CopyTo(dst);
            }
            if (!Stop)
            {
                //徐々に
                if (Gradually) ChangeDT();
                //表示
                if (DelayTime <= 0)
                {
                    DelayTime = 0;
                    DelayCounter = 0;
                    dst.CopyTo(newitem);
                }

                for (i = 0; ListTime[0] - ListTime[i] <= DelayTime; i++)
                {
                    DelayCounter = i;
                }
                
                if (DelayCounter > 0)
                {
                    
                    if (AddNow) Cv2.Add(ListMat[0], ListMat[DelayCounter - 1], newitem);
                    if (!AddNow) newitem = ListMat[i - 1];
                    //newitem.CopyTo(dst);
                }
                if (AddDelay2) funcAddDelay(newitem, newitem);
                if (flip) Cv2.Flip(newitem, newitem, FlipMode.Y);
                if (ColorInvert) newitem = ~newitem;
                newitem.CopyTo(dst);
                dst.CopyTo(stopitem);
            }
            TrueDT = DelayCounter / fps;
            if(boolGetSpine) GetSpine();
            LogAndSave();

        }

        #region 関数
        //腰位置とる
        public void GetSpine()
        {
            if (BodyData != null)
            {
                SpineData = ("");
                trackedBodyData = new List<Body>();
                for(j = 0; j < BodyData.Length; j++)
                {
                    if (BodyData[j].Joints[JointType.SpineBase].Position.Z != 0)
                    {
                        trackedBodyData.Add(BodyData[j]);
                        SpineData =SpineData + ("," + j + "," + BodyData[j].Joints[JointType.SpineBase].Position.X + "," + BodyData[j].Joints[JointType.SpineBase].Position.Z);
                    }
                }
                Humannum = trackedBodyData.Count;
                if (Humannum == 0) return;
            }
        }
        //DT変える
        public void ChangeDT()
        {
            Counter++;
            if (TargetDelayTime - 0.1f <= DelayTime && DelayTime <= TargetDelayTime + 0.1f)
            {
                DelayTime = TargetDelayTime;
            }
            if (Counter % pitchTM == 0)
            {
                if (TargetDelayTime < DelayTime)
                {
                    DelayTime = DelayTime - 0.04f;
                }
            }
            if (Counter % pitchTP == 0)
            {
                if (TargetDelayTime > DelayTime)
                {
                    DelayTime = DelayTime + 0.04f;
                }
            }
        }
        //実験
        public void JikkenDelayTime()
        {
            Counter++;
            if (!jikken_Invert)
            {
                if (Counter >= pitchTP)
                {
                    DelayTime = DelayTime + 0.04f;
                    Counter = 0;
                }
            }
            if (jikken_Invert)
            {
                if (Counter >= pitchTM)
                {
                    DelayTime = DelayTime - 0.04f;
                    Counter = 0;
                }
            }
        }
        //ランダム
        public void RandomDelayTime(float x)
        {
            ChangeDT();
            if (ListTime[0] - PreTimeRand > x) //一定時間たったら新しい遅れ時間を用意
            {
                System.Random randTDT = new System.Random(); //NextRandCounter用乱数
                TargetDelayTime = randTDT.Next(0, RandMaxTDT) / 100f;
                //System.Random randNRT = new System.Random(); //NextRandTime用乱数
                //NextRandTime = randNRT.Next(RandMinNRT, RandMaxNRT);
                PreTimeRand = ListTime[0];             
            }
        }
        //インタラクティブ
        public void InteractiveDelayTime(float x)
        {
            SumCW = SumCW + CountWhiteDiff(0, 1);
            ListTimeCounter++;
            ChangeDT();

            if (ListTime[0] - PreTimeInt > x) //一定時間たったら新しい遅れ時間を用意
            {
                OldAveCW = AveCW;
                AveCW = SumCW / ListTimeCounter;
                SumCW = 0;
                SolveTDT(AveCW);
                ListTimeCounter = 0;
                PreTimeInt = ListTime[0];
            }
        }
        //AveCWからTDTを求めるやつ.interactive
        public void SolveTDT(int x)
        {
            Gradient = PitchDT / 1000f;
            if (!Int_Invert) TargetDelayTime = (float)((Gradient * x) - (Gradient * Threshold)); //増えると増える(早いと遅く)
            if (Int_Invert) TargetDelayTime = (float)Math.Round(-(Gradient * x) + (Gradient * Threshold)); //増えると減る(早いと等速)
            if (TargetDelayTime <= 0) TargetDelayTime = 0;
        }
        //list[x] とlist[x + 1]の差分
        public int CountWhiteDiff(int x, int y)
        {
            //Mat diffimage = new Mat();
            Cv2.Absdiff(ListMat[x], ListMat[y], diffimage);
            CountWhite = Cv2.CountNonZero(Cv2.Split(diffimage)[0]); //差分画像
            return CountWhite;
        }
        //二種類の遅れを重ねる
        public Mat funcAddDelay(Mat src2, Mat dst2)
        {
            for (k = 0; ListTime[0] - ListTime[k] <= DelayTime2; k++)
            {
                DelayCounter2 = k;
            }
            Cv2.Add(src2, ListMat[DelayCounter2 - 1], dst2);
            return dst2;
        }
        //フレームレート
        public void FrameRate()
        {
            framecount++;
            if (Time.time >= NextTimeFPS)
            {
                fps = framecount;
                framecount = 0;
                NextTimeFPS += 1;
            }
        }
        //UI表示&保存用
        public void LogAndSave()
        {
            if (LogFPS) Debug.Log("FPS:" + fps);
            if (LogAveCW) Debug.Log("AveCW:" + AveCW);
            if (LogTDT) Debug.Log("TDT:" + TargetDelayTime);
            if (LogDC) Debug.Log("DC:" + DelayCounter);
            if (Save)
            {
                if (AddNow) intAddNow = 1;
                if (!AddNow) intAddNow = 0;
                if (AddDelay2) intAddDelay2 = 1;
                if (!AddDelay2) intAddDelay2 = 0;
                DataSave(ListTime[0] + "," + fps + "," +intAddNow + ","+intAddDelay2+","+DelayTime2+"," + pitchTP + "," + pitchTM + ",0," + AveCW + "," + Threshold + ",0," + TargetDelayTime + "," + DelayCounter + "," + TrueDT +","+ DelayTime+SpineData);
            }
        }
        //csv保存用
        public void DataSave(string txt)
        {
            StreamWriter sw;
            FileInfo fi;
            fi = new FileInfo(Application.dataPath + "/tanaka/csvData/" + FileName + ".csv");
            sw = fi.AppendText();
            sw.WriteLine(txt);
            sw.Flush();
            sw.Close();
        }
        #endregion

        public override string ToString()
        {
            return "TanakaTest2";
        }
        //継承抽象メンバーImageProcess(ref Mat src, ref Mat dst)
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            this.Update(ref src, ref dst);
        }
        //継承抽象メンバーgetImageProcesserType()
        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.TanakaTest;
        }
        public bool IsFirstFrame { get; private set; }
    }
}
