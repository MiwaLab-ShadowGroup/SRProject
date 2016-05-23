using UnityEngine;
using System.Collections.Generic;
using Miwalab.ShadowGroup.Background;
using Miwalab.ShadowGroup.Data;

public class ParticlesHost : MonoBehaviour
{
    public Vector3 usingBox = new Vector3(10, 10, 10);
    public Color color = new Color(1, 1, 1, 0);

    public List<AParticle> ParticleList = new List<AParticle>();
    public AParticle Paricle;
    public int ParticleNum;
    public Vector3 CreatePosition;
    public Quaternion CreateQuaternion;

    public float time;

    private HumanPointReceiver m_humanPointReceiver;
    

    #region UnityMethods
    // Use this for initialization
    void Start()
    {
        time = 0;
        m_humanPointReceiver = HumanPointReceiver.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        time += 0.001f;
        if (this.ParticleList.Count < ParticleNum)
        {
            AddParticles(1);
        }
    }
    #endregion

    public void AddParticles(int num)
    {
        for (int i = 0; i < num; ++i)
        {
            HumanPoints humanpoints = new HumanPoints();
            switch(Random.Range(0, 3)){
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
                CreatePosition = new Vector3(Random.value - 1/ 2 + humanpoints[Random.Range(0,humanpoints.Count-1)].X,
                                                Random.value - 1/ 2,
                                                Random.value - 1 / 2 + humanpoints[Random.Range(0, humanpoints.Count - 1)].Z);
            }
            else
            {
                CreatePosition = new Vector3(Random.value * usingBox.x - usingBox.x / 2,
                                                Random.value * usingBox.y - usingBox.y / 2,
                                                Random.value * usingBox.z - usingBox.z / 2);
            }
            CreateQuaternion = Quaternion.Euler(90, 0, Random.value * 360);
            var item = Instantiate(Paricle, CreatePosition, CreateQuaternion) as AParticle;
            this.ParticleList.Add(item);
            item.ParentList = this.ParticleList;
            var _color = color;
            float value = UnityEngine.Random.value * 0.4f - 0.2f;
            _color.r = color.r + value;
            _color.g = color.g + value;
            _color.b = color.b + value;
            item.setColor(_color);

        }
    }


}
