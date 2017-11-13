using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;
using UnityEngine;
using System.IO;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class TanakaTest : AShadowImageProcesser
    {
        private float fps;

        private int ListMax = 1000;
        private int DelayCounter;
        private int NextDelayCounter;
        //private int OldDelayCounter;  //処理用
        private int pOldDelayCounter; //ピッチ計算用
        private int pitch;
        private int pitchnum = 10;
        private int NextRandTime = 200;
        private int RandCounter = 0;
        private int RandMaxNDC = 500;
        private int RandMaxNRT = 300;
        private int RandMinNRT = 50;

        private bool DT_random;
        private bool jikken;
        private bool DT_rand2;

        private int framecount;
        private float nexttime;

        private float DelayTime;

        private string FileName;

        List<Mat> list;
        
        public TanakaTest()
            :base()
        {
            nexttime = Time.time + 1;
            this.list = new List<Mat>();

            FileName = DateTime.Now.ToString("yy_MM_dd_HH_mm_ss");
            //DataSave("Time,fps,DelayCounter,DelayTime");

            (ShadowMediaUIHost.GetUI("TanakaTest_DelayTime") as ParameterSlider).ValueChanged += TanakaTest_DelayTime;
            (ShadowMediaUIHost.GetUI("TanakaTest_DelayTime") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("DelayTime_Random") as ParameterCheckbox).ValueChanged += DelayTime_Random;
            (ShadowMediaUIHost.GetUI("Jikken") as ParameterCheckbox).ValueChanged += TanakaTest_Jikken;

            (ShadowMediaUIHost.GetUI("RandMax_NextDC") as ParameterSlider).ValueChanged += TanakaTest_RandMaxNDC;
            (ShadowMediaUIHost.GetUI("RandMin_NextRandTime") as ParameterSlider).ValueChanged += TanakaTest_RandMinNRT;
            (ShadowMediaUIHost.GetUI("RandMax_NextRandTime") as ParameterSlider).ValueChanged += TanakaTest_RandMaxNRT;
            (ShadowMediaUIHost.GetUI("pitchnum") as ParameterSlider).ValueChanged += TanakaTest_pitchnum;

        }

        //UI関連
        #region
        private void TanakaTest_Jikken(object sender, EventArgs e)
        {
            this.jikken = (e as ParameterCheckbox.ChangedValue).Value;

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

        private void TanakaTest_pitchnum(object sender, EventArgs e)
        {
            pitchnum = (int)(e as ParameterSlider.ChangedValue).Value;

        }

        private void DelayTime_Random(object sender, EventArgs e)
        {
            this.DT_random = (e as ParameterCheckbox.ChangedValue).Value;
        }

        private void TanakaTest_DelayTime(object sender, EventArgs e)
        {
            DelayCounter = (int)(e as ParameterSlider.ChangedValue).Value;
        }
        #endregion

        //継承抽象メンバーImageProcess(ref Mat src, ref Mat dst)
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            this.Update(ref src, ref dst);
        }

        private void Update(ref Mat src, ref Mat dst)
        {
            framecount++;
            if (Time.time >= nexttime)
            {
                fps = framecount;
                framecount = 0;
                nexttime += 1;
            }

            Mat item = new Mat();
            src.CopyTo(item); //srcをitemにコピー

            //リスト
            this.list.Insert(0, item); //リストの先頭にitemを追加

            if (list.Count > ListMax) //リストがListMaxを超えたら古いもの(末尾)から削除
            {
                this.list.RemoveAt(ListMax - 1);
            }

            //実験専用
            if (jikken)
            {
                RandCounter++;
                if (RandCounter >= 100)
                {
                    DelayCounter=DelayCounter+10;
                    RandCounter = 0;
                }
            }

            if (DT_random)
            {
                RandCounter++;

                if (RandCounter % 10 == 0)
                {
                    if (NextDelayCounter - 10 <= DelayCounter && DelayCounter <= NextDelayCounter + 10)
                    {
                        DelayCounter = NextDelayCounter;
                    }
                    else if (NextDelayCounter < DelayCounter)
                    {
                        DelayCounter = DelayCounter - pitchnum;
                    }
                    else if (NextDelayCounter > DelayCounter)
                    {
                        DelayCounter = DelayCounter + pitchnum;
                    }
                }

                if (RandCounter / NextRandTime > 0) //一定時間たったら新しい遅れ時間を用意
                {
                    pOldDelayCounter = DelayCounter;
                    System.Random randNDC = new System.Random(); //NextRandCounter用乱数
                    NextDelayCounter = randNDC.Next(0, RandMaxNDC);
                    System.Random randNRT = new System.Random(); //NextRandTime用乱数
                    NextRandTime = randNRT.Next(RandMinNRT, RandMaxNRT);
                    RandCounter = 0;
                    //pitch = Math.Abs(NextDelayCounter - pOldDelayCounter) / pitchnum; //刻み．間に10枚くらい挟む
                }
            }

            if (DelayCounter > 0)
            {
                Mat newitem = new Mat();
                newitem = list[DelayCounter - 1];
                newitem.CopyTo(dst);
            }

            DelayTime = DelayCounter / fps;
            Debug.Log("DC"+DelayCounter);
            Debug.Log("NDC" + NextDelayCounter);
            Debug.Log("pitchnum"+pitchnum);
            //DataSave(Time.time + "," + fps + "," + DelayCounter + "," + DelayTime);
        }

        public override string ToString()
        {
            return "TanakaTest";
        }

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

        //継承抽象メンバーgetImageProcesserType()
        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.TanakaTest;
        }

        public bool IsFirstFrame { get; private set; }
    }
}
