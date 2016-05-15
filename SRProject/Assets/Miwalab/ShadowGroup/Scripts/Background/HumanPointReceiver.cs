using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.Background
{
    /// <summary>
    /// Singletonなので実体化は関数で そろそろなんでもかんでもシングルトンにするのやめたほうがいいだろうか・・・
    /// </summary>
    public class HumanPointReceiver
    {
        private static HumanPointReceiver m_actual;
        public static HumanPointReceiver GetInstance()
        {
            if (m_actual == null)
            {
                //受信開始 呼ばれないとスレッドもまわらない
                m_actual = new HumanPointReceiver();
            }
            return m_actual;
        }

        private const string clientName = "PointReceive";
        private Network.NetworkHost m_networkHost;
        private Thread.ThreadHost m_threadHost;
        /// <summary>
        /// 人の位置．相対座標
        /// </summary>
        public Data.HumanPoints HumanPointList { set; get; }
        private HumanPointReceiver()
        {
            this.HumanPointList = new Data.HumanPoints();
            m_networkHost = Network.NetworkHost.GetInstance();
            m_threadHost = Thread.ThreadHost.GetInstance();

            m_networkHost.AddClient(Network.NetworkSettings.HumanPointReceiver_PositionReceive, clientName);
            m_threadHost.CreateNewThread(new Thread.ContinuouslyThread(this.ReceiveThread), clientName);
            m_threadHost.ThreadStart(clientName);
        }

        private void ReceiveThread()
        {
            int length = 0;
            byte[] data = m_networkHost.Receive(clientName,ref length);
            if(!(length > 0))
            {
                return;
            }
            //Lockが発生するので若干重いかも
            this.HumanPointList.setData(data);
        }
    }
}
