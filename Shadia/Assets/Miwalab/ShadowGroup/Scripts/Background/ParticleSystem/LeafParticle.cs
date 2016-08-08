using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LeafParticle : AParticle
{
    public override ParticleType getParticleType()
    {
        return ParticleType.Leaf;
    }


    public override void MeshSetup()
    {
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                this.positions.Add((new Vector3(x, y) - new Vector3(1, 1) / 2) / 2);
            }
        }
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                this.UVs.Add(new Vector2((float)x / (2 - 1), (float)y / (2 - 1)));
            }
        }
        for (int x = 0; x < 1; x++)
        {
            for (int y = 0; y < 1; y++)
            {
                var i = x * 2 + y;
                var steps = 2;
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

        var collider =this.gameObject.GetComponent<MeshCollider>();
        collider.sharedMesh = this.mesh;

        this.meshRenderer.material.SetColor("_Color", color);

    }
    public override void MoveSetup()
    {
        base.MoveSetup();

    }


    GameObject windZone;
    gamep _gamep;
    public LeafParticle(List<AParticle> parent)
    {
        this.ParentList = parent;
        
    }

    // Use this for initialization
    void Start()
    {
        this.MeshSetup();
        this.MoveSetup();
        windZone = GameObject.FindGameObjectWithTag("WindZone");
        _gamep = windZone.GetComponent<gamep>();
    }

    // Update is called once per frame
    void Update()
    {
        this.time += 0.001f;
        if (time < this.fadeInTime)
        {
            color.a = this.time / this.fadeInTime;
        }
        else if (time < this.LifeTime + this.fadeInTime)
        {

        }
        else if (time < this.LifeTime + this.fadeInTime + this.fadeOutTime)
        {
            this.color.a = 1 - (this.time - this.LifeTime - this.fadeInTime) / (this.fadeOutTime);
        }
        else
        {
            this.finish();
        }

        this.meshRenderer.material.SetColor("_Color", color);
        var vec = new Vector3(UnityEngine.Random.value * 20 - 10, 9 - UnityEngine.Random.value * 20, UnityEngine.Random.value * 20 - 10);
        this.rigidBody.AddForce(vec + _gamep.m_Move);
        this.rigidBody.AddTorque(UnityEngine.Random.value * 40 - 20, UnityEngine.Random.value * 40 - 20, UnityEngine.Random.value * 40 - 20);
    }


}
