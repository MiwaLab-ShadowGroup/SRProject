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

        public FadeTransition(List<AAfterEffect> transitionList,ASensorImporter sensor, AShadowImageProcesser to)
        {
            this.Parent = transitionList;

           
            m_fadein = new Fade.BlackFadeIn(Fadein_frame);
            m_fadeout = new Fade.BlackFadeOut(Fadeout_frame);


            Sensor = sensor;
            m_to = to;
            IsTransitioned = false;
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
