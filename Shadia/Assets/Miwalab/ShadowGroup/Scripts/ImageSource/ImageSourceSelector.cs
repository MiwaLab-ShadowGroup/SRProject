using UnityEngine;
using System.Collections;

public class ImageSourceSelector : MonoBehaviour {

    public GameObject ImageSource2D;
    public GameObject ImageSource3D;

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        switch (Miwalab.ShadowGroup.Core.ApplicationSettings.CurrentMode)
        {
            case Miwalab.ShadowGroup.Core.ShadowMediaMode.ShadowMedia2D:
                ImageSource2D.SetActive(true);
                ImageSource3D.SetActive(false);
                break;
            case Miwalab.ShadowGroup.Core.ShadowMediaMode.ShadowMedia3D:
                ImageSource2D.SetActive(true);
                ImageSource3D.SetActive(true);
                break;
        }
	}
}
