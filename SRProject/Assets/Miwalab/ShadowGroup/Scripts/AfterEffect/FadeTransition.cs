using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.AfterEffect
{
    public class FadeTransition : ATransition
    {
        
        public FadeTransition(List<AAfterEffect> transitionList)
        {
            this.Parent = transitionList;

            m_fadein = new Fade.BlackFadeIn(100);
            m_fadeout = new Fade.BlackFadeOut(100);
        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            if (!m_fadeout.IsFinished)
            {
                m_fadeout.ImageProcess(ref src, ref dst);
            }
            else if (!m_fadein.IsFinished)
            {
                m_fadein.ImageProcess(ref src, ref dst);
            }
            else
            {
                OnFinished();
            }
        }
        
    }
}
