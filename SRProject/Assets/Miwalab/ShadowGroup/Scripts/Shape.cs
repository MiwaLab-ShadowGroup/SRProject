using UnityEngine;
using System.Collections;

public class Shape : MonoBehaviour {

    public GameObject[] Verices;
    
    public Mesh mesh;
    public int[] indices;

    Vector3[] Verices_pos;
    public Vector2[] UvMaps;

    // Use this for initialization
    void Start()
    {
        this.mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = this.mesh;


        this.Verices_pos = new Vector3[this.Verices.Length];
        for(int i = 0; i < this.Verices.Length; i++)
        {
            this.Verices_pos[i] = this.Verices[i].transform.position;
        }

        this.mesh.vertices = this.Verices_pos;
        this.mesh.triangles = this.indices;
        this.mesh.uv = this.UvMaps;
        this.mesh.RecalculateNormals();
        this.mesh.RecalculateBounds();

    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0 ; i < this.Verices.Length; i++)
        {
            if(this.Verices[i].transform.position != this.Verices_pos[i])
            {
                this.Verices_pos[i] = this.Verices[i].transform.position;
                this.ReShape();
            }
        }


    }

    void ReShape()
    {
        this.mesh.vertices = this.Verices_pos;
        this.mesh.triangles = this.indices;
        this.mesh.uv = this.UvMaps;
        this.mesh.RecalculateNormals();
        this.mesh.RecalculateBounds();
    }
}
