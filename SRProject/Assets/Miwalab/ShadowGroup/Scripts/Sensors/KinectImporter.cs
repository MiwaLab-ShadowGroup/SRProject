using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class KinectImporter : MonoBehaviour
{
    KinectSensor m_sensor;
    // Use this for initialization
    void Start()
    {
        m_sensor = KinectSensor.GetDefault();
        if (m_sensor != null)
        {
            Debug.Log("The Kinect ID : " + m_sensor.UniqueKinectId);
            m_sensor.Open();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
