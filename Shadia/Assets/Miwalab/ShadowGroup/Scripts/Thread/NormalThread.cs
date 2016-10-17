using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.Thread
{
    public class NormalThread : IThread
    {
        private System.Threading.Thread m_thread;

        public delegate void ThreadMethod();

        private bool IsFinish { set; get; }

        public object SyncObject { set; get; }

        protected ThreadMethod m_method;
        private int m_num;

        /// <summary>
        /// 処理を一回実行する
        /// </summary>
        /// <param name="method">実装する処理</param>
        public NormalThread(ThreadMethod method)
            :this(method,1)
        {
        }
        /// <summary>
        /// 処理を指定回数実行する
        /// </summary>
        /// <param name="method">実装する処理</param>
        /// <param name="num">回数</param>
        public NormalThread(ThreadMethod method, int num)
        {
            m_method = method;
            m_thread = new System.Threading.Thread(new System.Threading.ThreadStart(this.Task));
            m_num = num;
            IsFinish = false;
            this.SyncObject = new object();
        }

        public void Start()
        {
            this.m_thread.Start();
        }

        public virtual void Task()
        {
            for (int i = 0; i < m_num; ++i)
            {
                this.m_method();
            }
            IsFinish = true;
        }
        
        public virtual void Abort()
        {
            this.m_thread.Abort();
        }

        public bool IsFinished()
        {
            return this.IsFinish;
        }

        public void Sync()
        {
            lock (this.SyncObject)
            {

            }
        }

        public void Sync(SyncMethod method)
        {
            lock (this.SyncObject)
            {
                method();
            }
        }
    }
}
