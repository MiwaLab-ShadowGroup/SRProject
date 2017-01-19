using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class SecondDelay : AShadowImageProcesser
    {
        //遅らせる時間 秒
        private float DelaySec = 3;
        private float time = 0;

        List<Mat> matList;
        List<float> timeList;

        bool flag = true;
        float maxtime = 185;
        int targetFrame;
        int deleteFrameNum;
        float predictFPS = 60;


        int layerNum = 0;

        public SecondDelay()
            : base()
        {


            this.matList = new List<Mat>();
            this.timeList = new List<float>();


            (ShadowMediaUIHost.GetUI("SecondDelay_DelaySec") as ParameterSlider).ValueChanged += SecondDelay_DelaySec_ValueChanged;
            (ShadowMediaUIHost.GetUI("SecondDelay_LayerNum") as ParameterSlider).ValueChanged += SecondDelay_LayerNum_ValueChanged;
            //(ShadowMediaUIHost.GetUI("SecondDelay_LayerNum") as ParameterCheckbox).ValueChanged += SecondDelay_LayerNum_ValueChanged;


            (ShadowMediaUIHost.GetUI("SecondDelay_DelaySec") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("SecondDelay_LayerNum") as ParameterSlider).ValueUpdate();
            //(ShadowMediaUIHost.GetUI("SecondDelay_LayerNum") as ParameterCheckbox).ValueUpdate();

        }

        private void SecondDelay_DelaySec_ValueChanged(object sender, EventArgs e)
        {
            this.DelaySec = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void SecondDelay_LayerNum_ValueChanged(object sender, EventArgs e)
        {
            this.layerNum = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        //private void SecondDelay_LayerNum_ValueChanged(object sender, EventArgs e)
        //{
        //    this.layerNum = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        //}


        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }



        private void Update(ref Mat src, ref Mat dst)
        {
            Mat item = new Mat();
            src.CopyTo(item);
            this.time += Time.deltaTime;

            //Listを埋める　maxtime,predictFPSからバッファーフレームを計算   180秒録画する
            if (this.flag == true)
            {
                for (int i = 0; i < this.maxtime * this.predictFPS; i++)
                {
                    this.matList.Add(item);
                    this.timeList.Add(- i / this.predictFPS);
                }


                this.flag = false;
            }


            //時間と画像のリストを作る
            this.matList.Insert(0, item);
            this.timeList.Insert(0, this.time);



            //取り出すフレームの確認
            for (int i = 0; this.timeList[i] >= (this.time - this.DelaySec); ++i)
            {
                this.targetFrame = i;
            }


            //描画
            this.matList[this.targetFrame].CopyTo(dst);

            //何人も出すときの描画
            for (int i = 0; i < this.layerNum; ++i)
            {
                dst += this.matList[(int)(i * this.targetFrame / this.layerNum)];
            }





            //時間を超過したフレームの個数をカウント
            this.deleteFrameNum = 0;
            for (int i = this.timeList.Count - 1; this.timeList[i] < (this.time - this.maxtime); --i)
            {
                this.deleteFrameNum++;
            }


            //カウントしたフレームを削除
            for (int i = 0; i < this.deleteFrameNum; ++i)
            {
                this.matList.RemoveAt(this.matList.Count - 1);
                this.timeList.RemoveAt(this.timeList.Count - 1);
            }




        }
        public override string ToString()
        {
            return "SecondDelay";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.SecondDelay;
        }

        public bool IsFirstFrame { get; private set; }

    }
}

