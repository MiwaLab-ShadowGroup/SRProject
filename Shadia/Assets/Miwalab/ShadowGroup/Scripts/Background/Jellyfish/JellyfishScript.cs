using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishScript : MonoBehaviour
{

    public GameObject gameObject;
    ParticleSystem particle;
    ParticleSystem.MainModule mm;
    ParticleSystem.EmissionModule em;
    ParticleSystem.NoiseModule nm;　//※きいた！unity上でノイズモジュールにチェックいれること
    ParticleSystem.ColorOverLifetimeModule colt;
    Gradient grad = new Gradient();

    //変えたい変数
    public int emitnum;
    public int totalnum;
    public float lifetime;
    public float speed;
    public float size;
    public float noise_strength;
    public float noise_frequency;
    Color color;
    public float R = 255.0f;
    public float G = 255.0f;
    public float B = 255.0f;

    // Use this for initialization
    void Start()
    {
        //UI
        (ShadowMediaUIHost.GetUI("Jellyfish_Size") as ParameterSlider).ValueChanged += JellyfishScript_Size;
        (ShadowMediaUIHost.GetUI("Jellyfish_TotalNum") as ParameterSlider).ValueChanged += JellyfishScript_TotalNum;
        (ShadowMediaUIHost.GetUI("Jellyfish_Speed") as ParameterSlider).ValueChanged += JellyfishScript_Speed;
        (ShadowMediaUIHost.GetUI("Noise_Strength") as ParameterSlider).ValueChanged += JellyfishScript_NoiseStrength;
        (ShadowMediaUIHost.GetUI("Noise_Frequency") as ParameterSlider).ValueChanged += JellyfishScript_NoiseFrequency;
        (ShadowMediaUIHost.GetUI("Jellyfish_Lifetime") as ParameterSlider).ValueChanged += JellyfishScript_Lifetime;
        (ShadowMediaUIHost.GetUI("Jellyfish_EmitNum") as ParameterSlider).ValueChanged += JellyfishScript_EmitNum;

        (ShadowMediaUIHost.GetUI("White") as ParameterButton).Clicked += White_Clicked;
        (ShadowMediaUIHost.GetUI("Red") as ParameterButton).Clicked += Red_Clicked;
        (ShadowMediaUIHost.GetUI("Blue") as ParameterButton).Clicked += Blue_Clicked;
        (ShadowMediaUIHost.GetUI("Green") as ParameterButton).Clicked += Green_Clicked;
        (ShadowMediaUIHost.GetUI("Yellow") as ParameterButton).Clicked += Yellow_Clicked;
        (ShadowMediaUIHost.GetUI("Cyan") as ParameterButton).Clicked += Cyan_Clicked;
        (ShadowMediaUIHost.GetUI("Pink") as ParameterButton).Clicked += Pink_Clicked;
        (ShadowMediaUIHost.GetUI("Jellyfish_R") as ParameterSlider).ValueChanged += JellyfishScript_R;
        (ShadowMediaUIHost.GetUI("Jellyfish_G") as ParameterSlider).ValueChanged += JellyfishScript_G;
        (ShadowMediaUIHost.GetUI("Jellyfish_B") as ParameterSlider).ValueChanged += JellyfishScript_B;

        //(ShadowMediaUIHost.GetUI("test2") as ParameterCheckbox).ValueChanged += JellyfishScript2_ValueChanged1;

        this.particle = this.gameObject.GetComponent<ParticleSystem>();        
        this.gameObject.SetActive(true);//ゲームオブジェクトが起動

        this.mm = this.particle.main;
        this.em = this.particle.emission;
        this.nm = this.particle.noise; //きかない
        this.colt = this.particle.colorOverLifetime;      
    }

    private void JellyfishScript_B(object sender, System.EventArgs e)
    {
        this.B = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void JellyfishScript_G(object sender, System.EventArgs e)
    {
        this.G = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void JellyfishScript_R(object sender, System.EventArgs e)
    {
        this.R = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Pink_Clicked(object sender, System.EventArgs e)
    {
        R = 255.0f; G = 0.0f; B = 255.0f;
    }

    private void Cyan_Clicked(object sender, System.EventArgs e)
    {
        R = 0.0f; G = 255.0f; B = 255.0f;
    }

    private void Yellow_Clicked(object sender, System.EventArgs e)
    {
        R = 255.0f; G = 255.0f; B = 0.0f;
    }

    private void Green_Clicked(object sender, System.EventArgs e)
    {
        R = 0.0f; G = 255.0f; B = 0.0f;
    }

    private void Blue_Clicked(object sender, System.EventArgs e)
    {
        R = 0.0f; G = 0.0f; B = 255.0f;
    }

    private void Red_Clicked(object sender, System.EventArgs e)
    {
        R = 255.0f; G = 0.0f; B = 0.0f;
    }

    private void White_Clicked(object sender, System.EventArgs e)
    {
        R = 255.0f; G = 255.0f; B = 255.0f;
    }

    private void JellyfishScript_EmitNum(object sender, System.EventArgs e)
    {
        this.emitnum = (int)(e as ParameterSlider.ChangedValue).Value;

    }

    private void JellyfishScript_Lifetime(object sender, System.EventArgs e)
    {
        this.lifetime = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void JellyfishScript_NoiseFrequency(object sender, System.EventArgs e)
    {
        this.noise_frequency = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void JellyfishScript_NoiseStrength(object sender, System.EventArgs e)
    {
        this.noise_strength = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void JellyfishScript_Speed(object sender, System.EventArgs e)
    {
        this.speed = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void JellyfishScript_TotalNum(object sender, System.EventArgs e)
    {
        this.totalnum = (int)(e as ParameterSlider.ChangedValue).Value;
    }

    private void JellyfishScript_Size(object sender, System.EventArgs e)
    {
        this.size=(float)(e as ParameterSlider.ChangedValue).Value;
    }

    //UI
    //private void JellyfishScript2_ValueChanged1(object sender, System.EventArgs e)
    //{
    //    this.test = (e as ParameterCheckbox.ChangedValue).Value;
    //}


    // Update is called once per frame
    void Update()
    {
        //一度に出る量
        this.em.rateOverTime = this.emitnum;

        //画面上にある最大数
        this.mm.maxParticles = this.totalnum;

        //寿命
        this.mm.startLifetime = this.lifetime;

        //モデルのおおきさ
        this.mm.startSize = this.size;

        //初速度
        this.mm.startSpeed = this.speed;


        //ゆれ
        this.nm.strength = this.noise_strength;
        this.nm.frequency = this.noise_frequency;

        color = new Color(R / 255.0f, G / 255.0f, B / 255.0f, 1.0f);

        this.grad.SetKeys(new GradientColorKey[]
                        { new GradientColorKey(this.color, 0.0f)},
                          new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.1f), new GradientAlphaKey(1.0f, 0.9f), new GradientAlphaKey(0.0f, 1.0f) });
        this.colt.color = grad;
    }
}


