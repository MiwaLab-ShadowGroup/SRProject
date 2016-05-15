using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Miwalab.ShadowGroup.Network
{
    public class RemoteManager
    {
        private TextAsset m_textAsset;

        public List<IPEndPoint> RemoteEPs { set; get; }

        public RemoteManager()
        {
            this.RemoteEPs = new List<IPEndPoint>();
        }

        public RemoteManager(TextAsset settingfile)
        {
            this.RemoteEPs = new List<IPEndPoint>();
            this.ReadRemoteEPs(settingfile);
        }

        public void ReadRemoteEPs(TextAsset settingfile)
        {
            this.RemoteEPs.Clear();
            Regex reg = new Regex(@"^(?<IP>.*?):(?<Port>\d+)$", RegexOptions.Singleline);
            var matchs = reg.Matches(settingfile.text);
            foreach (Match p in matchs)
            {
                string IP = p.Groups["IP"].Value;
                int Port = int.Parse(p.Groups["Port"].Value);
                this.RemoteEPs.Add(new IPEndPoint(IPAddress.Parse(IP), Port));
            }
        }

    }
}
