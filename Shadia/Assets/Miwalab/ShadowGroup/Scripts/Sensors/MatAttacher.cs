using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Miwalab.ShadowGroup.Network;

public class MatAttacher : MonoBehaviour {

    public ASensorImporter importer;
    public Texture2D tex;
    public Vector2 textureSize;
    public Miwalab.ShadowGroup.ImageSource.ImageSorceBlender _blender;
    public RemoteShadowImageManager _remoteShadowImageManager;

    public List<ProjectionPlane> m_ProjectionPlane = new List<ProjectionPlane>();
	// Use this for initialization
	void Start () {
        Debug.Assert(_remoteShadowImageManager != null);
	    if(importer == null)
        {
            Debug.Log("CameraImporter is not attached.");
            return;
        }
        tex = new Texture2D((int)textureSize.x, (int)textureSize.y);

        _blender._matattacher = this;

        var material = _blender.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        this.setMaterial(material);
        material.mainTexture = tex;

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
            data = importer.getCvMat().ToBytes(".png");
        }
        catch
        {
            Debug.Log("trying now...");
        }
        //ここで送信
        _remoteShadowImageManager.SendAllClient(data);
        Debug.Log(data.Length);
        //
        
        tex.LoadImage(data);
    }


    public void setMaterial(Material material)
    {
        foreach (var p in this.m_ProjectionPlane)
        {
            p.SetMaterial(material);
        }
    }
}
