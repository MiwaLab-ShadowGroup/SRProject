using UnityEngine;
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
    public int ParticleNum = 200;
    public Vector3 CreatePosition;
    public Quaternion CreateQuaternion;

    public float time;

    private HumanPointReceiver m_humanPointReceiver;

    public Camera renderCamera;

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

        (BackgroundMediaUIHost.GetUI("Particle_Size") as ParameterSlider).ValueChanged += Particle_Size_ValueChanged;
        (BackgroundMediaUIHost.GetUI("Particle_Num") as ParameterSlider).ValueChanged += Particle_Num_ValueChanged;

        (BackgroundMediaUIHost.GetUI("Particle_FadeWhite") as ParameterButton).Clicked += Particle_FadeWhite_Clicked;
        (BackgroundMediaUIHost.GetUI("Particle_FadeBlack") as ParameterButton).Clicked += Particle_FadeBlack_Clicked;
    }

    private void Particle_FadeBlack_Clicked(object sender, EventArgs e)
    {
        (BackgroundMediaUIHost.GetUI("Butterfly_BG_R") as ParameterSlider).m_slider.value = 0;
        (BackgroundMediaUIHost.GetUI("Butterfly_BG_G") as ParameterSlider).m_slider.value = 0;
        (BackgroundMediaUIHost.GetUI("Butterfly_BG_B") as ParameterSlider).m_slider.value = 0;
        (BackgroundMediaUIHost.GetUI("Butterfly_R") as ParameterSlider).m_slider.value = 1f;
        (BackgroundMediaUIHost.GetUI("Butterfly_G") as ParameterSlider).m_slider.value = 1f;
        (BackgroundMediaUIHost.GetUI("Butterfly_B") as ParameterSlider).m_slider.value = 1f;
    }

    private void Particle_FadeWhite_Clicked(object sender, EventArgs e)
    {
        (BackgroundMediaUIHost.GetUI("Butterfly_BG_R") as ParameterSlider).m_slider.value = 1f;
        (BackgroundMediaUIHost.GetUI("Butterfly_BG_G") as ParameterSlider).m_slider.value = 1f;
        (BackgroundMediaUIHost.GetUI("Butterfly_BG_B") as ParameterSlider).m_slider.value = 1f;
        (BackgroundMediaUIHost.GetUI("Butterfly_R") as ParameterSlider).m_slider.value = 0;
        (BackgroundMediaUIHost.GetUI("Butterfly_G") as ParameterSlider).m_slider.value = 0;
        (BackgroundMediaUIHost.GetUI("Butterfly_B") as ParameterSlider).m_slider.value = 0;
    }

    Vector3 m_particleSize = new Vector3(0.5f,0.5f,0.5f);
    private void Particle_Size_ValueChanged(object sender, EventArgs e)
    {
        float size = (e as ParameterSlider.ChangedValue).Value;
        m_particleSize = new Vector3(size, size, size);
    }

    private void Particle_Num_ValueChanged(object sender, EventArgs e)
    {
        ParticleNum = (int)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Butterfly_BG_B_ValueChanged(object sender, EventArgs e)
    {
        var color = m_backGroundColor;
        var value = (e as ParameterSlider.ChangedValue).Value;
        m_backGroundColor = new Color(color.r, color.g, value);
    }

    private void Butterfly_BG_G_ValueChanged(object sender, EventArgs e)
    {
        var color = m_backGroundColor;
        var value = (e as ParameterSlider.ChangedValue).Value;
        m_backGroundColor = new Color(color.r, value, color.b);

    }

    private void Butterfly_BG_R_ValueChanged(object sender, EventArgs e)
    {
        var color = m_backGroundColor;
        var value = (e as ParameterSlider.ChangedValue).Value;
        m_backGroundColor = new Color(value, color.g, color.b);

    }

    private void Butterfly_B_ValueChanged(object sender, EventArgs e)
    {
        m_particleColor.b = (e as ParameterSlider.ChangedValue).Value;
    }

    private void Butterfly_G_ValueChanged(object sender, EventArgs e)
    {
        m_particleColor.g = (e as ParameterSlider.ChangedValue).Value;
    }

    private void Butterfly_R_ValueChanged(object sender, EventArgs e)
    {
        m_particleColor.r = (e as ParameterSlider.ChangedValue).Value;
    }

    // Update is called once per frame
    void Update()
    {
        time += 0.001f;
        if (this.ParticleList.Count < ParticleNum)
        {
            AddParticles(1);
        }
        this.renderCamera.backgroundColor += (m_backGroundColor - this.renderCamera.backgroundColor) / 30;
        color += (m_particleColor - color) / 30;

    }
    #endregion

    public void AddParticles(int num)
    {
        for (int i = 0; i < num; ++i)
        {
            HumanPoints humanpoints = new HumanPoints();
            switch (UnityEngine.Random.Range(0, 3))
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
                int index = UnityEngine.Random.Range(0, humanpoints.Count - 1);
                CreatePosition = new Vector3(UnityEngine.Random.value - 1 / 2 + humanpoints[index].X,
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
            item.transform.localScale = m_particleSize;
            this.ParticleList.Add(item);
            item.ParentList = this.ParticleList;
            var _color = color;
            float value = UnityEngine.Random.value * 0.4f - 0.2f;
            _color.r = color.r + value;
            _color.g = color.g + value;
            _color.b = color.b + value;
            item.setColor(_color);
            item.transform.SetParent(this.gameObject.transform, false);

        }

        
    }
    private Color m_backGroundColor = new Color(0,0,0,0);
    private Color m_particleColor = new Color(1f,1f,1f,0);

}
