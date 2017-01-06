using UnityEngine;
using System.Collections;
using Miwalab.ShadowGroup.Callibration;

public class ProjectionPlane : MonoBehaviour {

    private Material m_material;
	// Use this for initialization

    public void SetMaterial(Material material)
    {
        this.m_material = material;
        this.GetComponent<MeshRenderer>().sharedMaterial = material;
    }
    
}
