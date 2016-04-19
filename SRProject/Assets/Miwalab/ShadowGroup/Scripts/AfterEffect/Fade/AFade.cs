using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.AfterEffect.Fade
{
    public abstract class AFade:ImageProcesser.AImageProcesser
    {
        protected int m_CurrentFrame;
        protected int m_FinishFrame;

        public bool IsFinished
        {
            get
            {
                return m_CurrentFrame >= m_FinishFrame;
            }
        }

        public void UpdateCurrentFrame()
        {
            ++m_CurrentFrame;
        }

        public AFade(int FinishFrame)
        {
            this.m_FinishFrame = FinishFrame;
        }

        public int GetCurrnetFrame()
        {
            return m_CurrentFrame;
        }
        public int GetFinishFrame()
        {
            return m_FinishFrame;
        }

    }
}
