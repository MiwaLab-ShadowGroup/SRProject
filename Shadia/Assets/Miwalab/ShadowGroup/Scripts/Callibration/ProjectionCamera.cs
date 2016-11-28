using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.Callibration
{
    [RequireComponent(typeof(Camera))]
    public class ProjectionCamera : MonoBehaviour
    {
        public int _CameraNumber;
        private Camera _ProjectionCamera;
        void Start()
        {
            _ProjectionCamera = GetComponent<Camera>();
            (ShadowMediaUIHost.GetUI("clb_camera_mode" + _CameraNumber) as ParameterDropdown).ValueChanged += ProjectionCamera_cameramode_ValueChanged;
            (ShadowMediaUIHost.GetUI("clb_camera_pos_x" + _CameraNumber) as ParameterSlider).ValueChanged += ProjectionCamera_pos_x_ValueChanged;
            (ShadowMediaUIHost.GetUI("clb_camera_pos_y" + _CameraNumber) as ParameterSlider).ValueChanged += ProjectionCamera_pos_y_ValueChanged;
            (ShadowMediaUIHost.GetUI("clb_camera_pos_z" + _CameraNumber) as ParameterSlider).ValueChanged += ProjectionCamera_pos_z_ValueChanged;

        }

        private void ProjectionCamera_pos_x_ValueChanged(object sender, EventArgs e)
        {
            var pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3((e as ParameterSlider.ChangedValue).Value, pos.y, pos.z);
        }

        private void ProjectionCamera_pos_y_ValueChanged(object sender, EventArgs e)
        {
            var pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, (e as ParameterSlider.ChangedValue).Value, pos.z);
        }

        private void ProjectionCamera_pos_z_ValueChanged(object sender, EventArgs e)
        {
            var pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, (e as ParameterSlider.ChangedValue).Value);
        }


        private void ProjectionCamera_cameramode_ValueChanged(object sender, EventArgs e)
        {
            switch ((Miwalab.ShadowGroup.Callibration.ProjectionCameraMode)(e as ParameterDropdown.ChangedValue).Value)
            {
                case Miwalab.ShadowGroup.Callibration.ProjectionCameraMode.Orthographic:
                    _ProjectionCamera.orthographic = true;
                    break;
                case Miwalab.ShadowGroup.Callibration.ProjectionCameraMode.Perthpective:
                    _ProjectionCamera.orthographic = false;
                    break;
            }
        }

    }
}
