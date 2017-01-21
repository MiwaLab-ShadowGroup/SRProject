
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class BodyChase : AShadowImageProcesser
    {
        //遅らせる時間 秒
        private float DelaySec = 3;
        private float preDelaySec = 0;

        private float time = 0;

        List<Mat> matList;
        List<float> timeList;

        List<float> chaseList;

        bool flag = true;
        float maxtime = 185;
        int targetFrame;
        int deleteFrameNum;
        float predictFPS = 60;
        float dampRate = 1.0f;
        float dampRateMin = 0f;

        float frameSpeed = 2;

        int layerNum = 0;
        int preLayerNum = 0;

        bool layerColor = false;
        bool layerFade = false;

        Mat m_buffer;

        public BodyChase()
            : base()
        {


            this.matList = new List<Mat>();
            this.timeList = new List<float>();
            this.chaseList = new List<float>();


            (ShadowMediaUIHost.GetUI("BodyChase_DelaySec") as ParameterSlider).ValueChanged += BodyChase_DelaySec_ValueChanged;
            (ShadowMediaUIHost.GetUI("BodyChase_LayerNum") as ParameterSlider).ValueChanged += BodyChase_LayerNum_ValueChanged;
            (ShadowMediaUIHost.GetUI("BodyChase_LayerClr") as ParameterCheckbox).ValueChanged += BodyChase_LayerClr_ValueChanged;
            (ShadowMediaUIHost.GetUI("BodyChase_DampRateMin") as ParameterSlider).ValueChanged += BodyChase_DampRateMin_ValueChanged;
            (ShadowMediaUIHost.GetUI("BodyChase_FrameSpd") as ParameterSlider).ValueChanged += BodyChase_FrameSpd_ValueChanged;
            (ShadowMediaUIHost.GetUI("BodyChase_LayerFade") as ParameterCheckbox).ValueChanged += BodyChase_LayerFade_ValueChanged;


            (ShadowMediaUIHost.GetUI("BodyChase_DelaySec") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("BodyChase_LayerNum") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("BodyChase_LayerClr") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("BodyChase_DampRateMin") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("BodyChase_FrameSpd") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("BodyChase_LayerFade") as ParameterCheckbox).ValueUpdate();

        }

        private void BodyChase_DelaySec_ValueChanged(object sender, EventArgs e)
        {
            this.DelaySec = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BodyChase_LayerNum_ValueChanged(object sender, EventArgs e)
        {
            this.layerNum = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BodyChase_LayerClr_ValueChanged(object sender, EventArgs e)
        {
            this.layerColor = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void BodyChase_DampRateMin_ValueChanged(object sender, EventArgs e)
        {
            this.dampRateMin = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BodyChase_FrameSpd_ValueChanged(object sender, EventArgs e)
        {
            this.frameSpeed = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BodyChase_LayerFade_ValueChanged(object sender, EventArgs e)
        {
            this.layerFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }


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
                    this.timeList.Add(-i / this.predictFPS);
                }

                for (int i = 0; i < 15; ++i )
                {
                    this.chaseList.Add(0);
                }

                m_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, new Scalar(0, 0, 0));
                this.flag = false;
            }

            //時間と画像のリストを作る
            this.matList.Insert(0, item);
            this.timeList.Insert(0, this.time);

            //取り出すフレームの確認　ここが重いかと思ったがそうでもない
            for (int i = 0; this.timeList[i] >= (this.time - this.DelaySec); ++i)
            {
                this.targetFrame = i;
            }

            //チェイサーのフレームの更新および間隔の変更
            if (this.preLayerNum == this.layerNum)
            {

                for (int i = 0; i < this.layerNum; ++i)
                {
                    this.chaseList[i] -= this.frameSpeed;
                    if (this.chaseList[i] < 0) this.chaseList[i] = this.targetFrame;
                    if (this.chaseList[i] > this.targetFrame ) this.chaseList[i] = 0;
                }

            }
            else
            {
                for (int i = 0; i < this.layerNum; ++i)
                {
                    this.chaseList[i] = (i+1) *  this.targetFrame / this.layerNum ;

                }
                this.preLayerNum = this.layerNum;
            }

            //DelaySecが変わっていたらレイヤーの間隔を等間隔に再調整
            if (this.preDelaySec != this.DelaySec )
            {
                for (int i = 0; i < this.layerNum; ++i)
                {
                    this.chaseList[i] = (i + 1) * this.targetFrame / this.layerNum;

                }
                this.preDelaySec = this.DelaySec;
            }

            


            if (!this.layerColor)
            {
                
                //描画
                this.matList[0].CopyTo(dst);

                //何人も出すときの描画
                for (int i = 0; i < this.layerNum; ++i)
                {
                    if (!this.layerFade)
                    {
                        dst += this.matList[(int)this.chaseList[i]];
                    }
                    else
                    {
                        dst += this.matList[(int)this.chaseList[i]] * ((this.targetFrame -this.chaseList[i]) / this.targetFrame)  ;
                    }

                    //Debug.Log("this.chaseList[ " +i + " ]" + this.chaseList[i]);

                }
                
            }
            //　色を薄くして分身している感を出す
            else
            {
                //描画
                this.matList[0].CopyTo(m_buffer);

                this.dampRate = 1.0f / (this.layerNum + 1.0f);

                if (this.dampRate < this.dampRateMin) this.dampRate = this.dampRateMin;

                m_buffer *= this.dampRate;

                //何人も出すときの描画
                for (int i = 0; i < this.layerNum; ++i)
                {
                    m_buffer += this.matList[(int)this.chaseList[i]] * this.dampRate;
                    //m_buffer += this.matList[(int)(i * this.targetFrame / this.layerNum)] * this.dampRate;
                    //Cv2.AddWeighted(dst, 1f, this.matList[(int)(i * this.targetFrame / this.layerNum)], 1 / (this.layerNum + 1), 0, dst);
                }

                m_buffer.CopyTo(dst);
            }


           // this.preLayerNum = this.layerNum;





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
            return "BodyChase";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.BodyChase;
        }

        public bool IsFirstFrame { get; private set; }

    }
}

