using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageSource.ShadowMedia3D
{
    [RequireComponent(typeof(Camera))]
    public class VirtualLightSource3D : MonoBehaviour
    {
        public float _kinectRotation_rx = 0;
        public float _kinectRotation_ry = 0;

        private Camera _camera;

        void Start()
        {
            _camera = GetComponent<Camera>();
            (ShadowMediaUIHost.GetUI("kinect_height") as ParameterSlider).ValueChanged += VirtualLightSource3D_height_ValueChanged;
            (ShadowMediaUIHost.GetUI("kinect_angle") as ParameterSlider).ValueChanged += VirtualLightSource3D_angle_ValueChanged;
            (ShadowMediaUIHost.GetUI("kinect_radius") as ParameterSlider).ValueChanged += VirtualLightSource3D_radius_ValueChanged;
            (ShadowMediaUIHost.GetUI("Kinect_rot_x") as ParameterSlider).ValueChanged += VirtualLightSource3D_rot_x_ValueChanged;
            (ShadowMediaUIHost.GetUI("Kinect_rot_y") as ParameterSlider).ValueChanged += VirtualLightSource3D_rot_y_ValueChanged;
            (ShadowMediaUIHost.GetUI("Kinect_light_r") as ParameterSlider).ValueChanged += VirtualLightSource3D_light_r_ValueChanged;
            (ShadowMediaUIHost.GetUI("Kinect_LightMode") as ParameterDropdown).ValueChanged += VirtualLightSource3D_LightModeChanged;
            (ShadowMediaUIHost.GetUI("Kinect_ViewRange") as ParameterSlider).ValueChanged += VirtualLightSource3D_ViewRangeChanged;

            (ShadowMediaUIHost.GetUI("kinect_height") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("kinect_angle") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("kinect_radius") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Kinect_rot_x") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Kinect_rot_y") as ParameterSlider).ValueUpdate();

        }

        private void VirtualLightSource3D_radius_ValueChanged(object sender, EventArgs e)
        {
        }

        private void VirtualLightSource3D_angle_ValueChanged(object sender, EventArgs e)
        {
        }

        private void VirtualLightSource3D_height_ValueChanged(object sender, EventArgs e)
        {
            var pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, (e as ParameterSlider.ChangedValue).Value, pos.z);
        }

        private void VirtualLightSource3D_ViewRangeChanged(object sender, EventArgs e)
        {
            float t = (e as ParameterSlider.ChangedValue).Value;
            this._camera.fieldOfView = Mathf.Atan(t) * 180 / Mathf.PI * 2f;
        }

        private void VirtualLightSource3D_LightModeChanged(object sender, EventArgs e)
        {
        }

        private void VirtualLightSource3D_light_r_ValueChanged(object sender, EventArgs e)
        {
        }

        private void VirtualLightSource3D_rot_y_ValueChanged(object sender, EventArgs e)
        {
            _kinectRotation_ry = -(e as ParameterSlider.ChangedValue).Value;
            this.gameObject.transform.rotation = Quaternion.Euler(_kinectRotation_rx, _kinectRotation_ry, 0);
        }

        private void VirtualLightSource3D_rot_x_ValueChanged(object sender, EventArgs e)
        {
            _kinectRotation_rx = -(e as ParameterSlider.ChangedValue).Value;

            this.gameObject.transform.rotation = Quaternion.Euler(_kinectRotation_rx, _kinectRotation_ry, 0);
        }
        

        void Update()
        {

        }
    }
}
