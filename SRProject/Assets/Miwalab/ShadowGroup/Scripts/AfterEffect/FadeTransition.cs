using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;
using Miwalab.ShadowGroup.ImageProcesser;

namespace Miwalab.ShadowGroup.AfterEffect
{
    public class FadeTransition : ATransition
    {
        public bool IsTransitioned { set; get; }
        AShadowImageProcesser m_to;
        ASensorImporter Sensor { set; get; }

        int Fadein_frame = 100;
        int Fadeout_frame = 100;
        private bool IsWhite = true;


        public FadeTransition(List<AAfterEffect> transitionList,ASensorImporter sensor, AShadowImageProcesser to)
        {

            (UIHost.GetUI("Frame_of_FadeIn") as ParameterSlider).ValueChanged += Frame_of_FadeIn_ValueChanged;
            (UIHost.GetUI("Frame_of_FadeOut") as ParameterSlider).ValueChanged += Frame_of_FadeOut_ValueChanged;
            (UIHost.GetUI("White_Fade") as ParameterCheckbox).ValueChanged += White_Fade_ValueChanged;

            (UIHost.GetUI("Frame_of_FadeIn") as ParameterSlider).ValueUpdate();
            (UIHost.GetUI("Frame_of_FadeOut") as ParameterSlider).ValueUpdate();
            (UIHost.GetUI("White_Fade") as ParameterCheckbox).ValueUpdate();


            this.Parent = transitionList;

            if (IsWhite)
            {
                m_fadein = new Fade.WhiteFadeIn(Fadein_frame);
                m_fadeout = new Fade.WhiteFadeOut(Fadeout_frame);
            }
            else
            {
                m_fadein = new Fade.BlackFadeIn(Fadein_frame);
                m_fadeout = new Fade.BlackFadeOut(Fadeout_frame);
            }
            

            Sensor = sensor;
            m_to = to;
            IsTransitioned = false;
        }

        private void White_Fade_ValueChanged(object sender, EventArgs e)
        {
            this.IsWhite = (e as ParameterCheckbox.ChangedValue).Value;
        }

        private void Frame_of_FadeOut_ValueChanged(object sender, EventArgs e)
        {
            this.Fadein_frame = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Frame_of_FadeIn_ValueChanged(object sender, EventArgs e)
        {
            this.Fadeout_frame = (int)(e as ParameterSlider.ChangedValue).Value;

        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            if (!m_fadeout.IsFinished)
            {
                m_fadeout.ImageProcess(ref src, ref dst);
            }
            else if (!m_fadein.IsFinished)
            {
                if (IsTransitioned == false)
                {
                    Sensor.RemoveAllImageProcesser();
                    Sensor.AddImageProcesser(m_to);
                    IsTransitioned = true;
                }
                m_fadein.ImageProcess(ref src, ref dst);
            }
            else
            {
                OnFinished();
            }
        }
        
    }
}
