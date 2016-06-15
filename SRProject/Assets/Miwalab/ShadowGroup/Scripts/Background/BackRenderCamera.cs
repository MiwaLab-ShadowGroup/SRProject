using UnityEngine;
using System.Collections;
using System;

namespace Miwalab.ShadowGroup.Background
{
    public enum BackRenderCameraSettingType
    {
        BackRenderCamera,
        Count
    }
    public class BackRenderCamera : MonoBehaviour
    {
        
        // Use this for initialization
        void Start()
        {
            (GUI.BackgroundMediaUIHost.GetUI("BRC_POS_X") as ParameterSlider).ValueChanged += BackRenderCamera_BRC_POS_X_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("BRC_POS_Y") as ParameterSlider).ValueChanged += BackRenderCamera_BRC_POS_Y_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("BRC_POS_Z") as ParameterSlider).ValueChanged += BackRenderCamera_BRC_POS_Z_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("BRC_ROT_X") as ParameterSlider).ValueChanged += BackRenderCamera_BRC_ROT_X_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("BRC_ROT_Y") as ParameterSlider).ValueChanged += BackRenderCamera_BRC_ROT_Y_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("BRC_ROT_Z") as ParameterSlider).ValueChanged += BackRenderCamera_BRC_ROT_Z_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("BRC_FIELD_VIEW") as ParameterSlider).ValueChanged += BackRenderCamera_BRC_FIELD_VIEW_ValueChanged;
        }

        private void BackRenderCamera_BRC_FIELD_VIEW_ValueChanged(object sender, EventArgs e)
        {
            var value = (e as ParameterSlider.ChangedValue).Value;
            this.GetComponent<Camera>().fieldOfView =value;
        }

        private void BackRenderCamera_BRC_ROT_Z_ValueChanged(object sender, EventArgs e)
        {
            var value = (e as ParameterSlider.ChangedValue).Value;
            var angle = this.gameObject.transform.rotation.eulerAngles;
            this.gameObject.transform.rotation = Quaternion.Euler(angle.x, angle.y, value);
        }

        private void BackRenderCamera_BRC_ROT_Y_ValueChanged(object sender, EventArgs e)
        {
            var value = (e as ParameterSlider.ChangedValue).Value;
            var angle = this.gameObject.transform.rotation.eulerAngles;
            this.gameObject.transform.rotation = Quaternion.Euler(angle.x, value, angle.z);
        }

        private void BackRenderCamera_BRC_ROT_X_ValueChanged(object sender, EventArgs e)
        {
            var value = (e as ParameterSlider.ChangedValue).Value;
            var angle = this.gameObject.transform.rotation.eulerAngles;
            this.gameObject.transform.rotation = Quaternion.Euler(value, angle.y, angle.z);
        }

        private void BackRenderCamera_BRC_POS_Z_ValueChanged(object sender, EventArgs e)
        {
            var value = (e as ParameterSlider.ChangedValue).Value;
            var position = this.gameObject.transform.position;
            this.gameObject.transform.position =  new Vector3(position.x, position.y, value);
        }

        private void BackRenderCamera_BRC_POS_Y_ValueChanged(object sender, EventArgs e)
        {
            var value = (e as ParameterSlider.ChangedValue).Value;
            var position = this.gameObject.transform.position;
            this.gameObject.transform.position= new Vector3(position.x, value, position.z);
        }

        private void BackRenderCamera_BRC_POS_X_ValueChanged(object sender, EventArgs e)
        {
            var value = (e as ParameterSlider.ChangedValue).Value;
            var position = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(value, position.y, position.z);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
