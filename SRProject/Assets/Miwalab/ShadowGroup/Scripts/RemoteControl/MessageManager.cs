using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Miwalab.ShadowGroup.RemoteControl
{
    public class MessageManager
    {
        private System.Threading.Thread ReceivingThread;
        private UdpClient listener;
        public bool IsAlive { set; get; }
        public MessageManager()
        {
            this.ReceivingThread = new System.Threading.Thread(this.ReceivingTask);
        }
        public void Start(int port)
        {
            this.listener = new UdpClient(port);
            this.ReceivingThread.Start();
        }

        private void ReceivingTask()
        {
            this.IsAlive = true;
            while (IsAlive)
            {
                while(listener.Available > 0)
                {
                    IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = listener.Receive(ref from);
                }
            }
        }
    }
}
