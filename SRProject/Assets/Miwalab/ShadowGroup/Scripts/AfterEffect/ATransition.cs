using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.AfterEffect
{
    public abstract class ATransition : AAfterEffect
    {
        protected Fade.AFadeIn m_fadein;
        protected Fade.AFadeOut m_fadeout;
        
        public int CurrentFrame
        {
            get
            {
               return m_fadein.GetCurrnetFrame() + m_fadeout.GetCurrnetFrame();
            }
        }
        public int FinishFrame
        {
            get
            {
                return m_fadein.GetFinishFrame() + m_fadeout.GetFinishFrame();
            }
        }

        public int TansitionFrame
        {
            get
            {
                return m_fadeout.GetFinishFrame();
            }
        }

        public bool IsFinished
        {
            get
            {
                return m_fadein.IsFinished && m_fadeout.IsFinished;
            }
        }

        public void OnFinished()
        {
            this.Parent.Remove(this);
        }

        protected List<AAfterEffect> Parent { set; get; }

    }
}
