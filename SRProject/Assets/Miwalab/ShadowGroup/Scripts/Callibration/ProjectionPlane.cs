using UnityEngine;
using System.Collections;

public class ProjectionPlane : MonoBehaviour {

    private Material m_material;
	// Use this for initialization

    public void SetMaterial(Material material)
    {
        this.m_material = material;
        this.GetComponent<MeshRenderer>().material = material;
    }
	
}
