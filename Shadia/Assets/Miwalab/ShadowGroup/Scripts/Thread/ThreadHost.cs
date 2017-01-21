using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Miwalab.ShadowGroup.Thread
{
    public class ThreadHost
    {
        /// <summary>
        /// 実態 singleton
        /// </summary>
        static ThreadHost m_actual;

        private Dictionary<string, IThread> m_theadList;

        private ThreadHost()
        {
            m_theadList = new Dictionary<string, IThread>();
        }

        public static ThreadHost GetInstance()
        {
            if (m_actual == null)
            {
                m_actual = new ThreadHost();
            }
            return m_actual;
        }

        public void CreateNewThread(IThread thread,string tag)
        {
            if (m_theadList.ContainsKey(tag))
            {
                Debug.LogWarning(tag +" thread is already contains." );
                return;
            }
            this.m_theadList.Add(tag, thread);
        }

        public void ThreadStart(string tag)
        {
            if (!m_theadList.ContainsKey(tag))
            {
                Debug.LogWarning(tag + " thread is NOT contains or alreadey finished.");
                return;
            }
            if (!this.m_theadList[tag].IsFinished())
            {
                this.m_theadList[tag].Start();
            }
        }

        public void AbortAndDeleteThread(string tag)
        {
            if (!m_theadList.ContainsKey(tag))
            {
                Debug.LogWarning(tag + " thread is NOT contains or alreadey finished.");
                return;
            }
            this.m_theadList[tag].Abort();
            this.m_theadList.Remove(tag);
        }

        /// <summary>
        /// 更新処理．すでに終わったスレッドを削除．どこかで一フレーム一回だけやればよい．
        /// </summary>
        public void Update()
        {
            foreach (var p in this.m_theadList)
            {
                if (p.Value.IsFinished())
                {
                    this.m_theadList.Remove(p.Key);
                }
            }
        }

        /// <summary>
        /// 全削除
        /// </summary>
        public void AllThreadAbort()
        {
            foreach(var p in this.m_theadList)
            {
                p.Value.Abort();
            }
            this.m_theadList.Clear();
        }
    }
}
