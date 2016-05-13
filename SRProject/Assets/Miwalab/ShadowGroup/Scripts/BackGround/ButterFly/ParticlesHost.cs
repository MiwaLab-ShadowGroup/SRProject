using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticlesHost : MonoBehaviour
{

    public List<AParticle> ParticleList = new List<AParticle>();
    public AParticle Paricle;
    public int ParticleNum;
    public Vector3 CreatePosition;
    public Quaternion CreateQuaternion;

    public float time;

    #region UnityMethods
    // Use this for initialization
    void Start()
    {
        time = 0;
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
            CreatePosition = new Vector3(   Random.value * usingBox.x - usingBox.x / 2,
                                            Random.value * usingBox.y - usingBox.y / 2,
                                            Random.value * usingBox.z - usingBox.z / 2);
            CreateQuaternion = Quaternion.Euler(90, 0, Random.value * 360);
            var item = Instantiate(Paricle, CreatePosition, CreateQuaternion) as AParticle;
            this.ParticleList.Add(item);
            item.ParentList = this.ParticleList;
        }
    }

    public Vector3 usingBox = new Vector3(10, 10, 10);

}
