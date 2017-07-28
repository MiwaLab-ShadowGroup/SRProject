using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ButterflyParticle : AParticle
{
    private float MoveRate;


    public override ParticleType getParticleType()
    {
        return ParticleType.Butterfly;
    }


    public override void MeshSetup()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                this.positions.Add((new Vector3(x, y) - new Vector3(9, 9) / 2) / 10);
            }
        }
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                this.UVs.Add(new Vector2((float)x / (10 - 1), (float)y / (10 - 1)));
            }
        }
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                var i = x * 10 + y;
                var steps = 10;
                this.indecies.Add(i);
                this.indecies.Add(i + 1);
                this.indecies.Add(i + steps);
                this.indecies.Add(i);
                this.indecies.Add(i + steps);
                this.indecies.Add(i + 1);

                this.indecies.Add(i + 1);
                this.indecies.Add(i + steps);
                this.indecies.Add(i + steps + 1);
                this.indecies.Add(i + 1);
                this.indecies.Add(i + steps + 1);
                this.indecies.Add(i + steps);

            }
        }

        base.MeshSetup();
        mesh.name = getParticleType().ToString();
        this.gameObject.name = getParticleType().ToString();
        this.meshRenderer.material = new Material(this.shader);
        this.meshRenderer.material.SetTexture("_MainTex", texture);

        
        this.meshRenderer.material.SetColor("_Color", color);
        this.meshRenderer.material.SetFloat("_Num", UnityEngine.Random.value * 100);

    }
    public override void MoveSetup()
    {
        base.MoveSetup();

    }

    public ButterflyParticle(List<AParticle> parent)
    {
        this.ParentList = parent;
    }

    // Use this for initialization
    void Start()
    {
        this.MeshSetup();
        this.MoveSetup();

        (ShadowMediaUIHost.GetUI("MoveRate") as ParameterSlider).ValueChanged += MoveRate_ValueChanged;

    }

    private void MoveRate_ValueChanged(object sender, EventArgs e)
    {
         MoveRate = (e as ParameterSlider.ChangedValue).Value;
    }

    // Update is called once per frame
    void Update()
    {
        this.time += 0.001f;
        this.meshRenderer.material.SetFloat("_MyTime", time);
        if (time < this.fadeInTime)
        {
            color.a = this.time / this.fadeInTime ;
        }
        else if (time < this.LifeTime + this.fadeInTime)
        {

        }
        else if (time < this.LifeTime + this.fadeInTime + this.fadeOutTime)
        {
            this.color.a = 1 - (this.time - this.LifeTime - this.fadeInTime) / (this.fadeOutTime) ;
        }
        else
        {
            this.finish();
        }

        this.meshRenderer.material.SetColor("_Color", color);
        var vec = new Vector3(UnityEngine.Random.value * MoveRate - MoveRate/2, UnityEngine.Random.value * MoveRate - MoveRate/2, UnityEngine.Random.value * MoveRate - MoveRate/2);
        this.rigidBody.AddForce(vec);
        this.rigidBody.AddTorque(0, UnityEngine.Random.value * 10 - 5, 0);
    }


}
