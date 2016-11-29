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
            (ShadowMediaUIHost.GetUI("clb_camera_pos_rx" + _CameraNumber) as ParameterSlider).ValueChanged += ProjectionCamera_pos_rx_ValueChanged;
            (ShadowMediaUIHost.GetUI("clb_camera_pos_ry" + _CameraNumber) as ParameterSlider).ValueChanged += ProjectionCamera_pos_ry_ValueChanged;
            (ShadowMediaUIHost.GetUI("clb_camera_pos_rz" + _CameraNumber) as ParameterSlider).ValueChanged += ProjectionCamera_pos_rz_ValueChanged;
            (ShadowMediaUIHost.GetUI("clb_camera_fview" + _CameraNumber) as ParameterSlider).ValueChanged += ProjectionCamera_fview_ValueChanged;

        }

        private void ProjectionCamera_fview_ValueChanged(object sender, EventArgs e)
        {
            _ProjectionCamera.fieldOfView = (e as ParameterSlider.ChangedValue).Value;
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

        private void ProjectionCamera_pos_rx_ValueChanged(object sender, EventArgs e)
        {
            var rot = this.gameObject.transform.rotation.eulerAngles;
            this.gameObject.transform.rotation = Quaternion.Euler((e as ParameterSlider.ChangedValue).Value, rot.y, rot.z);
        }

        private void ProjectionCamera_pos_ry_ValueChanged(object sender, EventArgs e)
        {
            var rot = this.gameObject.transform.rotation.eulerAngles;
            this.gameObject.transform.rotation = Quaternion.Euler(rot.x, (e as ParameterSlider.ChangedValue).Value, rot.z);
        }

        private void ProjectionCamera_pos_rz_ValueChanged(object sender, EventArgs e)
        {
            var rot = this.gameObject.transform.rotation.eulerAngles;
            this.gameObject.transform.rotation = Quaternion.Euler(rot.x, rot.y, (e as ParameterSlider.ChangedValue).Value);
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
