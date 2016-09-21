using UnityEngine;
using System.Collections;
using System;

public class RobotSet : MonoBehaviour
{

    public GameObject Robot;

    // Use this for initialization
    void Start()
    {
        (ShadowMediaUIHost.GetUI("RobotSet") as ParameterCheckbox).ValueChanged += RobotSet_ValueChanged;

        (ShadowMediaUIHost.GetUI("RobotSet") as ParameterCheckbox).ValueUpdate();


    }


    // Update is called once per frame
    void Update()
    {

    }

    private void RobotSet_ValueChanged(object sender, EventArgs e)
    {
        this.Robot.SetActive((e as ParameterCheckbox.ChangedValue).Value);
    }

}
