using UnityEngine;
using System.Collections;

public class gamep: MonoBehaviour {
    public Vector3 move = Vector3.zero;
    public float rotationSpeed = 180.0f;
    public float  ookisa= 0;


    WindZone m_wind;
   
    void Start () {
        
        var wind = gameObject.GetComponent<WindZone>();
        if (wind != null)
        {
            this.m_wind = wind;
        }
        wind.mode = WindZoneMode.Directional;

        wind.windMain = 4.0f;
        wind.windTurbulence = 0.5f;
        wind.windPulseMagnitude = 0.2f;
        wind.windPulseFrequency = 0.01f;
 
    }

    public Vector3 m_Move = new Vector3();

    // Update is called once per frame
    void Update () {
        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        m_Move += (move - m_Move) / 30;
        m_wind.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, m_Move);
        m_wind.windMain += (move.magnitude * 10 - m_wind.windMain)/30;

        

    }
}

