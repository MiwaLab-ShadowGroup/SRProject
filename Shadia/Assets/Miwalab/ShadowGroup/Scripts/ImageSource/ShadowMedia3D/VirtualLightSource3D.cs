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

        public float _virtualLightRotation_y = 0;
        public float _virtualLightPosZ = 0;
        public bool virtualLightDivide = false;

        public KinectImporter.LightSourceMode _lightMode;
        private Camera _camera;

        void Start()
        {
            _camera = GetComponent<Camera>();
            (ShadowMediaUIHost.GetUI("kinect_height") as ParameterSlider).ValueChanged += VirtualLightSource3D_height_ValueChanged;
            (ShadowMediaUIHost.GetUI("kinect_angle") as ParameterSlider).ValueChanged += VirtualLightSource3D_angle_ValueChanged;
            (ShadowMediaUIHost.GetUI("kinect_radius") as ParameterSlider).ValueChanged += VirtualLightSource3D_radius_ValueChanged;
            (ShadowMediaUIHost.GetUI("Kinect_light_r") as ParameterSlider).ValueChanged += VirtualLightSource3D_light_r_ValueChanged;
            (ShadowMediaUIHost.GetUI("Kinect_LightMode") as ParameterDropdown).ValueChanged += VirtualLightSource3D_LightModeChanged;
            (ShadowMediaUIHost.GetUI("Kinect_ViewRange") as ParameterSlider).ValueChanged += VirtualLightSource3D_ViewRangeChanged;
            (ShadowMediaUIHost.GetUI("kinect_divide") as ParameterCheckbox).ValueChanged += VirtualLightSource3D_divide_ValueChanged;


            (ShadowMediaUIHost.GetUI("kinect_height") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("kinect_angle") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("kinect_radius") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("kinect_divide") as ParameterCheckbox).ValueUpdate();
        }

        private void VirtualLightSource3D_divide_ValueChanged(object sender, EventArgs e)
        {
            this.virtualLightDivide = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void VirtualLightSource3D_radius_ValueChanged(object sender, EventArgs e)
        {
            _virtualLightPosZ = (e as ParameterSlider.ChangedValue).Value;
            var quat = Quaternion.Euler(0, _virtualLightRotation_y, 0);
            this.gameObject.transform.position = quat*( new Vector3(0, 0, _virtualLightPosZ)); 
        }

        private void VirtualLightSource3D_angle_ValueChanged(object sender, EventArgs e)
        {

            _virtualLightRotation_y = -(e as ParameterSlider.ChangedValue).Value;
            var quat = this.gameObject.transform.rotation = Quaternion.Euler(0, _virtualLightRotation_y, 0);

            this.gameObject.transform.position = quat * (new Vector3(0, 0, _virtualLightPosZ));
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


        void Update()
        {

        }
    }
}
