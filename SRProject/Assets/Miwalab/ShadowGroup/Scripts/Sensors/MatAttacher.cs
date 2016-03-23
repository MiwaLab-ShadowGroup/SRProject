using UnityEngine;
using System.Collections;

public class MatAttacher : MonoBehaviour {

    public ASensorImporter importer;
    public Texture2D tex;
    public Vector2 textureSize;
    public Material material;
	// Use this for initialization
	void Start () {
	    if(importer == null)
        {
            Debug.Log("CameraImporter is not attached.");
            return;
        }
        tex = new Texture2D((int)textureSize.x, (int)textureSize.y);
        material = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        if (importer == null)
        {
            return;
        }
        if(importer.getCvMat() == null)
        {
            return;
        }
        byte[] data = new byte[0];
        try {
            data = importer.getCvMat().ToBytes(".PNG");
        }
        catch
        {
            Debug.Log("trying now...");
        }
        tex.LoadImage(data);
        material.mainTexture = tex;
    }
}
