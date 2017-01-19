using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class DoubleAfterImage : AShadowImageProcesser
    {
        private List<List<bool>> m_Field = new List<List<bool>>();
        private bool IsInit = false;
        private int _w;
        private int _h;

        private Mat bufimage = new Mat();
        //永続確定
        private Mat outerColorBuffer2;
        private Mat innerColorBuffer2;
        private Mat innerGrayBuffer2;
        private Mat outerGrayBuffer2;

        private Mat m_element;
        private int __w;
        private int __h;

        Scalar insidecolor;
        Scalar outsidecolor;
        
        double innertime;
        double outertime;
        double parameter;

        bool inner = true;
        bool outer = true;

        private bool invert;

        public bool IsFirstFrame { get; private set; }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            if (!IsInit)
            {
                this.Initialize(src.Width, src.Height);
                this.IsInit = true;
            }
            this.Update(ref src, ref dst);
        }

        #region CAmethods





        #endregion

        public DoubleAfterImage():base()
        {
            (ShadowMediaUIHost.GetUI("Zanzou_ins_R") as ParameterSlider).ValueChanged += Zanzou_ins_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_G") as ParameterSlider).ValueChanged += Zanzou_ins_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_B") as ParameterSlider).ValueChanged += Zanzou_ins_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Zanzou_out_R") as ParameterSlider).ValueChanged += Zanzou_out_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Zanzou_out_G") as ParameterSlider).ValueChanged += Zanzou_out_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Zanzou_out_B") as ParameterSlider).ValueChanged += Zanzou_out_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Zanzou_in_tm") as ParameterSlider).ValueChanged += Zanzou_in_tm_ValueChanged;
            (ShadowMediaUIHost.GetUI("Zanzou_ou_tm") as ParameterSlider).ValueChanged += Zanzou_ou_tm_ValueChanged;
            (ShadowMediaUIHost.GetUI("Zanzou_param") as ParameterSlider).ValueChanged += Zanzou_param_ValueChanged;
            (ShadowMediaUIHost.GetUI("Zanzou_Invert") as ParameterCheckbox).ValueChanged += Zanzou_Invert_ValueChanged;
            (ShadowMediaUIHost.GetUI("Zanzou_Inner") as ParameterCheckbox).ValueChanged += Zanzou_Inner_ValueChanged;
            (ShadowMediaUIHost.GetUI("Zanzou_Outer") as ParameterCheckbox).ValueChanged += Zanzou_Outer_ValueChanged;

            (ShadowMediaUIHost.GetUI("Zanzou_CC_Blue") as ParameterButton).Clicked += Zanzou_CC_Blue_Clicked;
            (ShadowMediaUIHost.GetUI("Zanzou_CC_Orange") as ParameterButton).Clicked += Zanzou_CC_Orange_Clicked;
            (ShadowMediaUIHost.GetUI("Zanzou_CC_Yellow") as ParameterButton).Clicked += Zanzou_CC_Yellow_Clicked;
            (ShadowMediaUIHost.GetUI("Zanzou_CC_Pink") as ParameterButton).Clicked += Zanzou_CC_Pink_Clicked;
            (ShadowMediaUIHost.GetUI("Zanzou_CC_Green") as ParameterButton).Clicked += Zanzou_CC_Green_Clicked;

            (ShadowMediaUIHost.GetUI("Zanzou_ins_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Zanzou_ins_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Zanzou_ins_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Zanzou_out_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Zanzou_out_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Zanzou_out_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Zanzou_in_tm") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Zanzou_ou_tm") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Zanzou_param") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Zanzou_Invert") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Zanzou_Inner") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Zanzou_Outer") as ParameterCheckbox).ValueUpdate();

        }

        private void Zanzou_CC_Green_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Zanzou_ins_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_B") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_out_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Zanzou_out_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_out_B") as ParameterSlider).m_slider.value = 0;
        }

        private void Zanzou_CC_Pink_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Zanzou_ins_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_B") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_out_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_out_G") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Zanzou_out_B") as ParameterSlider).m_slider.value = 255;
        }
      

        private void Zanzou_CC_Yellow_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Zanzou_ins_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_B") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_out_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_out_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_out_B") as ParameterSlider).m_slider.value = 0;
        }

        private void Zanzou_CC_Orange_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Zanzou_ins_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_B") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_out_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_out_G") as ParameterSlider).m_slider.value = 125;
            (ShadowMediaUIHost.GetUI("Zanzou_out_B") as ParameterSlider).m_slider.value = 0;
        }

        private void Zanzou_CC_Blue_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Zanzou_ins_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_ins_B") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_out_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Zanzou_out_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Zanzou_out_B") as ParameterSlider).m_slider.value = 255;
        }

        private void Zanzou_Invert_ValueChanged(object sender, EventArgs e)
        {
            this.invert = (e as ParameterCheckbox.ChangedValue).Value;
        }

        private void Zanzou_Inner_ValueChanged(object sender, EventArgs e)
        {
            this.inner = (e as ParameterCheckbox.ChangedValue).Value;
        }

        private void Zanzou_Outer_ValueChanged(object sender, EventArgs e)
        {
            this.outer = (e as ParameterCheckbox.ChangedValue).Value;
        }

        private void Zanzou_param_ValueChanged(object sender, EventArgs e)
        {
            this.parameter = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Zanzou_ou_tm_ValueChanged(object sender, EventArgs e)
        {
            this.outertime = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Zanzou_in_tm_ValueChanged(object sender, EventArgs e)
        {
            this.innertime = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Zanzou_out_B_ValueChanged(object sender, EventArgs e)
        {
            this.outsidecolor.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Zanzou_out_G_ValueChanged(object sender, EventArgs e)
        {
            this.outsidecolor.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Zanzou_out_R_ValueChanged(object sender, EventArgs e)
        {
            this.outsidecolor.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Zanzou_ins_B_ValueChanged(object sender, EventArgs e)
        {
            this.insidecolor.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Zanzou_ins_G_ValueChanged(object sender, EventArgs e)
        {
            this.insidecolor.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Zanzou_ins_R_ValueChanged(object sender, EventArgs e)
        {
            this.insidecolor.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Update(ref Mat src, ref Mat dst)
        {
            try
            {
                Cv2.CvtColor(src, bufimage, OpenCvSharp.ColorConversion.BgrToGray);

                #region 邪魔者はちるべし
                //unsafe
                //{
                //    byte* pPixel = bufimage.DataPointer;
                //    Random r = new Random();

                //    for (int y = 0; y < _h; y++)
                //    {
                //        for (int x = 0; x < _w; x++)
                //        {
                //            //黒くない点は生きてる
                //            if (*pPixel > 100)
                //            {
                //                m_Field[y][x] = true;
                //                if (r.Next(0, 100) < 60)
                //                {           //ofRandom(0,100) < 80だった
                //                    //m_Field[y][x] = true;
                //                }
                //            }
                //            else
                //            {
                //                m_Field[y][x] = false;
                //            }
                //            *pPixel = m_Field[y][x] ? (byte)255 : (byte)0;
                //            pPixel++;
                //        }
                //    }
                //}
                #endregion
                //    //////

                Cv2.Dilate(bufimage, bufimage, m_element);
                Cv2.Dilate(bufimage, bufimage, m_element);

                Cv2.Erode(bufimage, bufimage, m_element);
                Cv2.Erode(bufimage, bufimage, m_element);



                //*********************************************************************************************
                //ここからメイン
                //*********************************************************************************************

                ////***********************************************************

                ////cvFindContoursを用いた輪郭抽出*****************************
                Mat tmp_bufImage_next;
                Mat tmp_bufImage_next3;

                //TODO:移動可
                tmp_bufImage_next = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
                tmp_bufImage_next3 = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));

                bufimage.CopyTo(tmp_bufImage_next);

                Point[][] contours;
                HierarchyIndex[] hierarchy;

                /// Find contours
                Cv2.FindContours(tmp_bufImage_next, out contours, out hierarchy, OpenCvSharp.ContourRetrieval.Tree, OpenCvSharp.ContourChain.ApproxNone);

                /// Draw contours
                for (int i = 0; i < contours.Length; i++)
                {
                    Scalar color = new Scalar(255);
                    //Cv2.DrawContours(tmp_bufImage_next3, contours, i, color, 2, OpenCvSharp.LineType.Link8, hierarchy, 0);
                    Cv2.FillPoly(tmp_bufImage_next3, contours, color);
                }


                //cvClearSeq(contours);	//これはいらないみたい
                ////***********************************************************

                ////残像処理***************************************************


                innerGrayBuffer2 -= innertime;        //param.slider[0];
                outerGrayBuffer2 -= outertime;   //param.slider[1];

                outerGrayBuffer2 += tmp_bufImage_next3;

                innerGrayBuffer2 += tmp_bufImage_next3.Clone() - parameter;


                for (int i = 0; i < 3; i++)
                {       //(int)param.slider[2]
                    Cv2.Erode(innerGrayBuffer2, innerGrayBuffer2, m_element);
                }

                for (int i = 0; i < 1; i++)
                {       //(int)param.slider[3]
                    Cv2.Erode(outerGrayBuffer2, outerGrayBuffer2, m_element);
                }


                Mat tmpColorBuffer2 = new Mat(new Size(bufimage.Width, bufimage.Height), MatType.CV_8UC3,new Scalar(255,255,255));
                outerColorBuffer2.SetTo(outsidecolor, null);
                Cv2.CvtColor(outerGrayBuffer2, tmpColorBuffer2, OpenCvSharp.ColorConversion.GrayToBgr);
                Cv2.Multiply(outerColorBuffer2, tmpColorBuffer2, outerColorBuffer2, 1.0 / 255.0);
                innerColorBuffer2.SetTo(insidecolor, null);
                Cv2.CvtColor(innerGrayBuffer2, tmpColorBuffer2, OpenCvSharp.ColorConversion.GrayToBgr);
                Cv2.Multiply(innerColorBuffer2, tmpColorBuffer2, innerColorBuffer2, 1.0 / 255.0);

                if(inner == true && outer == true)
                {
                    outerColorBuffer2 -= innerColorBuffer2;

                    outerColorBuffer2.GaussianBlur(new Size(3, 3), 3).CopyTo(dst);
                }

                else if (inner == true && outer == false)
                {

                    innerColorBuffer2.GaussianBlur(new Size(3, 3), 3).CopyTo(dst);
                }

                else if (inner == false && outer == true)
                {

                    outerColorBuffer2.GaussianBlur(new Size(3, 3), 3).CopyTo(dst);
                }
                else
                {

                }


                ////***********************************************************
                //bufimage_pre = bufimage;
                //bufimage_pre -= 20.0;

                if (invert)
                {
                    dst = ~dst;
                }


                //Cv2.CvtColor(bufimage, dst, OpenCvSharp.ColorConversion.GrayToBgr);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.TargetSite);
            }


        }

        private void Initialize(int w, int h)
        {
            m_Field.Clear();
            _w = w;
            _h = h;
            this.innerGrayBuffer2 = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            this.outerGrayBuffer2 = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            this.outerColorBuffer2 = new Mat(new Size(_w, _h), MatType.CV_8UC3, new Scalar(255, 255, 255));
            this.innerColorBuffer2 = new Mat(new Size(_w, _h), MatType.CV_8UC3, new Scalar(255, 255, 255));


            this.m_element = new Mat(3, 3, MatType.CV_8UC1, new Scalar(1));
            this.m_element.Set<byte>(0, 0, 0);
            this.m_element.Set<byte>(2, 0, 0);
            this.m_element.Set<byte>(0, 2, 0);
            this.m_element.Set<byte>(2, 2, 0);

            this.bufimage = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));

            //LifeGame_setup(_w / 5, _h / 5);
            for (int i = 0; i < h; i++)
            {
                List<bool> vTmp = new List<bool>();
                for (int j = 0; j < w; j++)
                {
                    vTmp.Add(false);
                }
                this.m_Field.Add(vTmp);
            }
            //this.IsFirstFrame = true;
        }

        public override string ToString()
        {
            return "Zanzou";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.DoubleAfterImage;
        }
    }
}
