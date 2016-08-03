using UnityEngine;
using System.Collections;
using Miwalab.ShadowGroup.Background;
using Miwalab.ShadowGroup.Data;

public class SimpleHumanPosition : MonoBehaviour {

    private HumanPointReceiver m_humanPointReceiver;
    public int i = 0;
    // Use this for initialization
    void Start () {
        m_humanPointReceiver = HumanPointReceiver.GetInstance();
	}
	
	// Update is called once per frame
	void Update () {
        HumanPoints humanpoints = new HumanPoints();
        switch (i)
        {
            case 0:
                if (this.m_humanPointReceiver.HumanPointList1 != null)
                {
                    humanpoints = this.m_humanPointReceiver.HumanPointList1;
                }
                break;
            case 1:
                if (this.m_humanPointReceiver.HumanPointList2 != null)
                {
                    humanpoints = this.m_humanPointReceiver.HumanPointList2;
                }
                break;
            case 2:
                if (this.m_humanPointReceiver.HumanPointList3 != null)
                {
                    humanpoints = this.m_humanPointReceiver.HumanPointList3;
                }
                break;
            case 3:
                if (this.m_humanPointReceiver.HumanPointList4 != null)
                {
                    humanpoints = this.m_humanPointReceiver.HumanPointList4;
                }
                break;
            default:
                break;
        }

        if (humanpoints.Count > 0)
        {
            this.gameObject.transform.localPosition = new Vector3(humanpoints[0].X, humanpoints[0].Y, humanpoints[0].Z);
        }
    }
}
