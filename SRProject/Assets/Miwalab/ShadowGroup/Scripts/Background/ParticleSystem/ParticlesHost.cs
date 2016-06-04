﻿using UnityEngine;
using System.Collections.Generic;
using Miwalab.ShadowGroup.Background;
using Miwalab.ShadowGroup.Data;
using Miwalab.ShadowGroup.GUI;
using System;

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

        (BackgroundMediaUIHost.GetUI("Butterfly_R") as ParameterSlider).ValueChanged += Butterfly_R_ValueChanged;
        (BackgroundMediaUIHost.GetUI("Butterfly_G") as ParameterSlider).ValueChanged += Butterfly_G_ValueChanged;
        (BackgroundMediaUIHost.GetUI("Butterfly_B") as ParameterSlider).ValueChanged += Butterfly_B_ValueChanged;
        (BackgroundMediaUIHost.GetUI("Butterfly_BG_R") as ParameterSlider).ValueChanged += Butterfly_BG_R_ValueChanged;
        (BackgroundMediaUIHost.GetUI("Butterfly_BG_G") as ParameterSlider).ValueChanged += Butterfly_BG_G_ValueChanged;
        (BackgroundMediaUIHost.GetUI("Butterfly_BG_B") as ParameterSlider).ValueChanged += Butterfly_BG_B_ValueChanged;
        
    }

    private void Butterfly_BG_B_ValueChanged(object sender, EventArgs e)
    {
    }

    private void Butterfly_BG_G_ValueChanged(object sender, EventArgs e)
    {
    }

    private void Butterfly_BG_R_ValueChanged(object sender, EventArgs e)
    {
    }

    private void Butterfly_B_ValueChanged(object sender, EventArgs e)
    {
        color.b = (e as ParameterSlider.ChangedValue).Value;
    }

    private void Butterfly_G_ValueChanged(object sender, EventArgs e)
    {
        color.g = (e as ParameterSlider.ChangedValue).Value;
    }

    private void Butterfly_R_ValueChanged(object sender, EventArgs e)
    {
        color.r = (e as ParameterSlider.ChangedValue).Value;
    }

    // Update is called once per frame
    void Update()
    {
        time += 0.001f;
        if (this.ParticleList.Count < ParticleNum)
        {
            AddParticles(1);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            this.color.r -= 5;
        }
        if (Input.GetKey(KeyCode.W))
        {
            this.color.r += 5;
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.color.g -= 5;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.color.g += 5;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            this.color.b -= 5;
        }
        if (Input.GetKey(KeyCode.X))
        {
            this.color.b += 5;
        }

    }
    #endregion

    public void AddParticles(int num)
    {
        for (int i = 0; i < num; ++i)
        {
            HumanPoints humanpoints = new HumanPoints();
            switch(UnityEngine.Random.Range(0, 3)){
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
                int index = UnityEngine.Random.Range(0, humanpoints.Count - 1);
                CreatePosition = new Vector3(UnityEngine.Random.value - 1/ 2 + humanpoints[index].X,
                                                UnityEngine.Random.value * usingBox.y - usingBox.y / 2,
                                                UnityEngine.Random.value - 1 / 2 + humanpoints[index].Z);
            }
            else
            {
                CreatePosition = new Vector3(UnityEngine.Random.value * usingBox.x - usingBox.x / 2,
                                                UnityEngine.Random.value * usingBox.y - usingBox.y / 2,
                                                UnityEngine.Random.value * usingBox.z - usingBox.z / 2);
            }
            CreateQuaternion = Quaternion.Euler(90, 0, UnityEngine.Random.value * 360);
            var item = Instantiate(Paricle, CreatePosition, CreateQuaternion) as AParticle;
            this.ParticleList.Add(item);
            item.ParentList = this.ParticleList;
            var _color = color;
            float value = UnityEngine.Random.value * 0.4f - 0.2f;
            _color.r = color.r + value;
            _color.g = color.g + value;
            _color.b = color.b + value;
            item.setColor(_color);
            item.transform.SetParent(this.gameObject.transform,false);
        }
    }


}
