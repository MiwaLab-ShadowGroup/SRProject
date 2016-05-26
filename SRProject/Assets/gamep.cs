using UnityEngine;
using System.Collections;

public class gamep: MonoBehaviour {
    public Vector3 move = Vector3.zero;
    public float rotationSpeed = 180.0f;
    public float  ookisa= 0;

  

   
    void Start () {
        
        var wind = gameObject.AddComponent<WindZone>();
        wind.mode = WindZoneMode.Directional;

        wind.windMain = -4.0f;
        wind.windTurbulence = 0.5f;
        wind.windPulseMagnitude = 0.2f;
        wind.windPulseFrequency = 0.01f;
 
    }

    // Update is called once per frame
    void Update () {
        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 playDir = move;
        ookisa = playDir.magnitude;
        if (playDir.magnitude > 0.1f)
        {
            ookisa = playDir.magnitude;
            //var wind = gameObject.AddComponent<WindZone>();
            //wind.mode = WindZoneMode.Directional;
            //wind.radius = 10.0f;
          //wind.windMain = ookisa;
            //wind.windTurbulence = 0.5f;
            //wind.windPulseMagnitude = 2.0f;
            //wind.windPulseFrequency = 0.01f;

            
            




            Quaternion q = Quaternion.LookRotation(playDir);
           transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotationSpeed * Time.deltaTime);

        }

    }
}

