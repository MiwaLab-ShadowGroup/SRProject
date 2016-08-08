using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AParticle : MonoBehaviour
{
    public enum ParticleType
    {
        Butterfly,
        Leaf,
        Default,
        Num,
    }

    public virtual void MeshSetup()
    {
        this.mesh = new Mesh();
        this.mesh.vertices = positions.ToArray();
        this.mesh.uv = UVs.ToArray();
        this.mesh.triangles = indecies.ToArray();
        this.mesh.RecalculateBounds();
        this.meshFilter = GetComponent<MeshFilter>();
        this.meshFilter.sharedMesh = mesh;
        this.meshRenderer = GetComponent<MeshRenderer>();
    }
    public void setColor(Color color)
    {
        this.color = color;
    }
    public virtual void MoveSetup()
    {
        this.rigidBody = GetComponent<Rigidbody>();
    }


    protected void finish()
    {
        this.ParentList.Remove(this);
        Destroy(this.gameObject);
    }
    public Color color;

    public List<AParticle> ParentList = null;
    public float fadeInTime = 0.5f;
    public float LifeTime = 0.5f;
    public float fadeOutTime = 0.5f;

    public Rigidbody rigidBody;
    public abstract ParticleType getParticleType();
    public Shader shader;
    public Texture2D texture;
    public Mesh mesh;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public List<Vector3> positions = new List<Vector3>();
    public List<Vector2> UVs = new List<Vector2>();
    public List<int> indecies = new List<int>();
    public float time;

}