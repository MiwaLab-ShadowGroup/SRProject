using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.Test
{
    public class PointReceiveTest : MonoBehaviour
    {
        Background.HumanPointReceiver m_humanPointReceiver;

        void Start()
        {
            m_humanPointReceiver = Background.HumanPointReceiver.GetInstance();
        }

        void Update()
        {
            if(this.m_humanPointReceiver.HumanPointList1.Count > 0)
            {
                var point = this.m_humanPointReceiver.HumanPointList1[0];
                this.gameObject.transform.position = new Vector3(point.X, point.Y, point.Z);
            }
        }
    }
}
