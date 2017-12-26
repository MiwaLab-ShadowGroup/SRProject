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
    public class TanakaTest : AShadowImageProcesser
    {
        #region//値宣言
        private float fps;

        private bool AddNow;
        private bool DT_random;
        private bool jikken;
        private bool jikken_invert;
        private bool DT_interactive;

        private bool Save;
        private bool LogAveCW;
        private bool LogDC;
        private bool LogNDC;
        private bool LogFPS;
        private int intAddNow;

        private int ListMax = 1000;
        private int DelayCounter;
        private int NextDelayCounter;
        private int PitchTP;
        private int PitchTM;
        private int PitchF;
        private int NextRandTime = 200;
        private int RandCounter = 0;
        private int RandMaxNDC = 500;
        private int RandMaxNRT = 300;
        private int RandMinNRT = 50;

        private int framecount;
        private float nexttimeFPS;
        private float DelayTime;

        private string FileName;

        private int CountWhite;
        private int CountWhiteNow;
        private int sumCW = 0;
        private int AveCW;
        private int CWcount;
        private int CWcountMax = 90;
        private float WhitePercent;
        private float OldAveCW = 0;

        private int Thresh;
        private float pitchDC;
        private float a;
        private bool Int_Invert;
        List<Mat> list;

        //腰位置
        List<Body> trackedBodyData; //body型を収納するリスト
        private int Humannum;
        private int j;
        private string SpineData;

        Mat diffimage = new Mat();
        #endregion

        public TanakaTest()
            :base()
        {           
            nexttimeFPS = Time.time + 1;
            this.list = new List<Mat>();
            //目標フレームレート
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            #region//UI
            (ShadowMediaUIHost.GetUI("DelayTime_Interactive") as ParameterCheckbox).ValueChanged += TanakaTest_Interactive;
            (ShadowMediaUIHost.GetUI("TanakaTest_DelayTime") as ParameterSlider).ValueChanged += TanakaTest_DelayTime;
            (ShadowMediaUIHost.GetUI("TanakaTest_DelayTime") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("DelayTime_Random") as ParameterCheckbox).ValueChanged += DelayTime_Random;
            (ShadowMediaUIHost.GetUI("Jikken") as ParameterCheckbox).ValueChanged += TanakaTest_Jikken;
            (ShadowMediaUIHost.GetUI("Jikken_Invert") as ParameterCheckbox).ValueChanged += TanakaTest_Jikken_Invert;
            (ShadowMediaUIHost.GetUI("RandMax_NextDC") as ParameterSlider).ValueChanged += TanakaTest_RandMaxNDC;
            (ShadowMediaUIHost.GetUI("RandMin_NextRandTime") as ParameterSlider).ValueChanged += TanakaTest_RandMinNRT;
            (ShadowMediaUIHost.GetUI("RandMax_NextRandTime") as ParameterSlider).ValueChanged += TanakaTest_RandMaxNRT;
            (ShadowMediaUIHost.GetUI("PitchFrame") as ParameterSlider).ValueChanged += TanakaTest_PitchF;
            (ShadowMediaUIHost.GetUI("PitchTimePlus") as ParameterSlider).ValueChanged += TanakaTest_PitchTP;
            (ShadowMediaUIHost.GetUI("PitchTimeMinus") as ParameterSlider).ValueChanged += TanakaTest_PitchTM;
            (ShadowMediaUIHost.GetUI("Int_Thresh") as ParameterSlider).ValueChanged += TanakaTest_Thresh;
            (ShadowMediaUIHost.GetUI("Int_pitchDC") as ParameterSlider).ValueChanged += TanakaTest_pitchDC;
            (ShadowMediaUIHost.GetUI("Int_Invert") as ParameterCheckbox).ValueChanged += TanakaTest_Int_Invert;
            (ShadowMediaUIHost.GetUI("DataSave") as ParameterCheckbox).ValueChanged += TanakaTest_DetaSave;
            (ShadowMediaUIHost.GetUI("LogAveCW") as ParameterCheckbox).ValueChanged += TanakaTest_LogAveCw;
            (ShadowMediaUIHost.GetUI("LogDC") as ParameterCheckbox).ValueChanged += TanakaTest_LogCW;
            (ShadowMediaUIHost.GetUI("LogNDC") as ParameterCheckbox).ValueChanged += TanakaTest_LogNDC;
            (ShadowMediaUIHost.GetUI("LogFPS") as ParameterCheckbox).ValueChanged += TanakaTest_LogFPS;
            (ShadowMediaUIHost.GetUI("TanakaTest_AddNow") as ParameterCheckbox).ValueChanged += TanakaTest_AddNow;
            #endregion
        }

        #region//UI
        private void TanakaTest_AddNow(object sender, EventArgs e)
        {
            this.AddNow = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TanakaTest_LogCW(object sender, EventArgs e)
        {
            this.LogDC = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TanakaTest_LogAveCw(object sender, EventArgs e)
        {
            this.LogAveCW = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TanakaTest_LogNDC(object sender, EventArgs e)
        {
            this.LogNDC = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TanakaTest_LogFPS(object sender, EventArgs e)
        {
            this.LogFPS = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TanakaTest_DetaSave(object sender, EventArgs e)
        {
            this.Save = (e as ParameterCheckbox.ChangedValue).Value;
            if (Save)
            {
                FileName = DateTime.Now.ToString("yy_MM_dd_HH_mm_ss");
                DataSave("1Time,2FPS,3AddNow,4pitchTP,5pitchTM,6pitchF,7AveCW,8Threshold,9NextDelayCounter,10TargetDelayTime,11DelayCounter,12DelayTime(DC/fps),13DelayTime,14HumanNum,15SpineX,16SpineZ,17HumanNum,18SpineX,19SpineZ,20HumanNum,21SpineX,22SpineZ");
            }
        }
        private void TanakaTest_Int_Invert(object sender, EventArgs e)
        {
            this.Int_Invert = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TanakaTest_pitchDC(object sender, EventArgs e)
        {
            pitchDC = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TanakaTest_Thresh(object sender, EventArgs e)
        {
            Thresh = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TanakaTest_Interactive(object sender, EventArgs e)
        {
            this.DT_interactive = (e as ParameterCheckbox.ChangedValue).Value;
            if (DT_random) Debug.Log("!Warning! DT_random_is_Checking"+Time.time);
        }
        private void TanakaTest_PitchTP(object sender, EventArgs e)
        {
            PitchTP = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TanakaTest_PitchTM(object sender, EventArgs e)
        {
            PitchTM = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TanakaTest_Jikken(object sender, EventArgs e)
        {
            this.jikken = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TanakaTest_Jikken_Invert(object sender, EventArgs e)
        {
            this.jikken_invert = (e as ParameterCheckbox.ChangedValue).Value;
        }
        private void TanakaTest_RandMaxNDC(object sender, EventArgs e)
        {
            RandMaxNDC = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TanakaTest_RandMinNRT(object sender, EventArgs e)
        {
            RandMinNRT = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TanakaTest_RandMaxNRT(object sender, EventArgs e)
        {
            RandMaxNRT = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void TanakaTest_PitchF(object sender, EventArgs e)
        {
            PitchF = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        private void DelayTime_Random(object sender, EventArgs e)
        {
            this.DT_random = (e as ParameterCheckbox.ChangedValue).Value;
            if (DT_interactive) Debug.Log("!Warning! DT_interactive_is_Checking" + Time.time);
        }
        private void TanakaTest_DelayTime(object sender, EventArgs e)
        {
            DelayCounter = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        #endregion

        private void Update(ref Mat src, ref Mat dst)
        {
            //フレームレート
            FrameRate();

            Mat item = new Mat();
            src.CopyTo(item); //srcをitemにコピー

            //リスト
            this.list.Insert(0, item); //リストの先頭にitemを追加

            if (list.Count > ListMax) //リストがListMaxを超えたら古いもの(末尾)から削除
            {
                this.list.RemoveAt(ListMax - 1);
            }        

            //実験で使うかも？
            if (jikken) Jikken();

            //ランダム処理
            if (DT_random) RandomDelayCounter();

            //身体に合わせるやつ
            if (DT_interactive) InteractiveDelayCounter();

            //表示
            if (DelayCounter <= 0)
            {
                DelayCounter = 0;
            }
            if (DelayCounter > 0)
            {
                Mat newitem = new Mat();
                if(AddNow) Cv2.Add(list[0], list[DelayCounter - 1], newitem);
                if(!AddNow) newitem = list[DelayCounter - 1];
                newitem.CopyTo(dst);
            }

            DelayTime = DelayCounter / fps;

            GetSpine();
            //データ保存，表示系
            #region
            if (LogAveCW) Debug.Log("aveCW:" + AveCW);
            if (LogDC) Debug.Log("DC:"+ DelayCounter);
            if (LogNDC) Debug.Log("NDC:"+ NextDelayCounter);
            if (LogFPS) Debug.Log("FPS:"+ fps);

            if (Save)
            {
                if (AddNow) intAddNow = 1;
                if (!AddNow) intAddNow = 0;
                DataSave(Time.time + "," + fps + "," + intAddNow + "," + PitchTP + "," + PitchTM + "," + PitchF + "," + AveCW + "," + Thresh + "," + NextDelayCounter + ",0," + DelayCounter + "," + DelayTime +",0"+ SpineData);
            }
            #endregion
        }

        //腰位置とる
        public void GetSpine()
        {
            if (BodyData != null)
            {
                SpineData = ("");
                trackedBodyData = new List<Body>();
                for (j = 0; j < BodyData.Length; j++)
                {
                    if (BodyData[j].Joints[JointType.SpineBase].Position.Z != 0)
                    {
                        trackedBodyData.Add(BodyData[j]);
                        SpineData = SpineData + ("," + j + "," + BodyData[j].Joints[JointType.SpineBase].Position.X + "," + BodyData[j].Joints[JointType.SpineBase].Position.Z);
                    }
                }
                Humannum = trackedBodyData.Count;
                if (Humannum == 0) return;
            }
        }
        //フレームレート
        public void FrameRate()
        {
            framecount++;
            if (Time.time >= nexttimeFPS)
            {
                fps = framecount;
                framecount = 0;
                nexttimeFPS += 1;
            }
        }
        //インタラクティブなやつ
        public void InteractiveDelayCounter()
        {
            CWcount++;
            CountWhiteNow = Cv2.CountNonZero(Cv2.Split(list[0])[0]); //現在画像の白の数
            sumCW = sumCW + CountWhiteDiff(0, 1);

            if (CWcount >= CWcountMax)
            {
                OldAveCW = AveCW;
                AveCW = sumCW / CWcountMax;
                sumCW = 0;
                CWcount = 0;

                SolveNDC(AveCW);
            }

            ChangeDC(CWcount);
        }
        //ランダム処理
        public void RandomDelayCounter()
        {
            RandCounter++;

            ChangeDC(RandCounter);

            if (RandCounter / NextRandTime > 0) //一定時間たったら新しい遅れ時間を用意
            {
                //pOldDelayCounter = DelayCounter;
                System.Random randNDC = new System.Random(); //NextRandCounter用乱数
                NextDelayCounter = randNDC.Next(0, RandMaxNDC);
                System.Random randNRT = new System.Random(); //NextRandTime用乱数
                NextRandTime = randNRT.Next(RandMinNRT, RandMaxNRT);
                RandCounter = 0;
                //pitch = Math.Abs(NextDelayCounter - pOldDelayCounter) / Pitchnum; //刻み．間に10枚くらい挟む
            }
        }
        //実験        
        public void Jikken()
        {
            RandCounter++;
            if (!jikken_invert)
            {
                if (RandCounter >= PitchTP)
                {
                    DelayCounter = DelayCounter + PitchF;
                    RandCounter = 0;
                }
            }
            if (jikken_invert)
            {
                if (RandCounter >= PitchTM)
                {
                    DelayCounter = DelayCounter - PitchF;
                    RandCounter = 0;
                }
            }
        }
        //DC変えるやつ．random，interactive共通
        public void ChangeDC(int x)
        {
            if (NextDelayCounter - 5 <= DelayCounter && DelayCounter <= NextDelayCounter + 5)
            {
                DelayCounter = NextDelayCounter;
            }

            if (x % PitchTM == 0)
            {
                if (NextDelayCounter < DelayCounter)
                {
                    DelayCounter = DelayCounter - (PitchF);
                }
            }

            if (x % PitchTP == 0)
            {
                if (NextDelayCounter > DelayCounter)
                {
                    DelayCounter = DelayCounter + PitchF;
                }
            }
        }
        //AveCWからNDCを求めるやつ.interactive
        public void SolveNDC(int x)
        {
            a = pitchDC / 1000f;
            if(!Int_Invert) NextDelayCounter =(int) Math.Round((a * x) - (a * Thresh)); //増えると増える(早いと遅く)
            if(Int_Invert) NextDelayCounter = (int)Math.Round(-(a * x) + (a * Thresh)); //増えると減る(早いと等速)
        }
        //list[x] とlist[x + 1]の差分
        public int CountWhiteDiff(int x, int y)
        {
            //Mat diffimage = new Mat();
            Cv2.Absdiff(list[x], list[y], diffimage);
            CountWhite = Cv2.CountNonZero(Cv2.Split(diffimage)[0]); //差分画像
            return CountWhite;
        }
        //csv保存
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

        public override string ToString()
        {
            return "TanakaTest";
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
