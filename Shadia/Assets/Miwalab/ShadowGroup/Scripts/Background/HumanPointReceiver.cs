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

        private const string clientName1 = "PointReceive1";
        private const string clientName2 = "PointReceive2";
        private const string clientName3 = "PointReceive3";
        private const string clientName4 = "PointReceive4";
        private Network.NetworkHost m_networkHost;
        private Thread.ThreadHost m_threadHost;
        /// <summary>
        /// 人の位置．相対座標
        /// </summary>
        public Data.HumanPoints HumanPointList1 { set; get; }
        public Data.HumanPoints HumanPointList2 { set; get; }
        public Data.HumanPoints HumanPointList3 { set; get; }
        public Data.HumanPoints HumanPointList4 { set; get; }
        private HumanPointReceiver()
        {
            this.HumanPointList1 = new Data.HumanPoints();
            m_networkHost = Network.NetworkHost.GetInstance();
            m_threadHost = Thread.ThreadHost.GetInstance();

            m_networkHost.AddClient(Network.NetworkSettings.HumanPointReceiver_PositionReceive1, clientName1);
            m_threadHost.CreateNewThread(new Thread.ContinuouslyThread(this.ReceiveThread1), clientName1);
            m_threadHost.ThreadStart(clientName1);

            m_networkHost.AddClient(Network.NetworkSettings.HumanPointReceiver_PositionReceive2, clientName2);
            m_threadHost.CreateNewThread(new Thread.ContinuouslyThread(this.ReceiveThread2), clientName2);
            m_threadHost.ThreadStart(clientName2);

            m_networkHost.AddClient(Network.NetworkSettings.HumanPointReceiver_PositionReceive3, clientName3);
            m_threadHost.CreateNewThread(new Thread.ContinuouslyThread(this.ReceiveThread3), clientName3);
            m_threadHost.ThreadStart(clientName3);

            m_networkHost.AddClient(Network.NetworkSettings.HumanPointReceiver_PositionReceive4, clientName4);
            m_threadHost.CreateNewThread(new Thread.ContinuouslyThread(this.ReceiveThread4), clientName4);
            m_threadHost.ThreadStart(clientName4);
        }

        private void ReceiveThread1()
        {
            int length = 0;
            byte[] data = m_networkHost.Receive(clientName1,ref length);
            if(!(length > 0))
            {
                return;
            }
            //Lockが発生するので若干重いかも
            this.HumanPointList1.setData(data);
        }

        private void ReceiveThread2()
        {
            int length = 0;
            byte[] data = m_networkHost.Receive(clientName2, ref length);
            if (!(length > 0))
            {
                return;
            }
            //Lockが発生するので若干重いかも
            this.HumanPointList2.setData(data);
        }

        private void ReceiveThread3()
        {
            int length = 0;
            byte[] data = m_networkHost.Receive(clientName3, ref length);
            if (!(length > 0))
            {
                return;
            }
            //Lockが発生するので若干重いかも
            this.HumanPointList3.setData(data);
        }

        private void ReceiveThread4()
        {
            int length = 0;
            byte[] data = m_networkHost.Receive(clientName4, ref length);
            if (!(length > 0))
            {
                return;
            }
            //Lockが発生するので若干重いかも
            this.HumanPointList4.setData(data);
        }
    }
}
