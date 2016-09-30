using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.Thread
{
    public class ContinuouslyThread : NormalThread
    {

        public readonly int sleepTime = 100;
        public ContinuouslyThread(ThreadMethod method)
            : base(method)
        {
            this.IsContinue = true;
        }

        private bool IsContinue { set; get; }

        public override void Task()
        {
            while (IsContinue)
            {
                lock (SyncObject)
                {
                    this.m_method();
                }
            }
        }

        public override void Abort()
        {
            IsContinue = false;
            System.Threading.Thread.Sleep(sleepTime);
            base.Abort();
        }

    }
}
