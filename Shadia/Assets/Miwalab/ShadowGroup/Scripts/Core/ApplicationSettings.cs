using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.Core
{
    public class ApplicationSettings : MonoBehaviour
    {
        public static ShadowMediaMode _CurrentMode;
        public static ShadowMediaMode CurrentMode
        {
            get
            {
                return _CurrentMode;
            }
            private set
            {
                _CurrentMode = value;
            }
        }



        public void Start()
        {

            CurrentMode = ShadowMediaMode.ShadowMedia2D;

            Debug.Log("displays connected: " + Display.displays.Length);
            // Display.displays[0] is the primary, default display and is always ON.
            // Check if additional displays are available and activate each.
            if (Display.displays.Length > 1)
                Display.displays[1].Activate();
            if (Display.displays.Length > 2)
                Display.displays[2].Activate();
            if (Display.displays.Length > 3)
                Display.displays[3].Activate();
        }
       

        public void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }


        public void SaveAllParameters()
        {
            ShadowMediaUIHost.SaveAllSettings();
        }
        public void LoadAllParameters()
        {
            ShadowMediaUIHost.LoadAllSettings();
        }
        

        public void SetShadowMediaMode(int i)
        {
            switch (i)
            {
                case 0:
                    CurrentMode = ShadowMediaMode.ShadowMedia2D;
                    break;
                case 1:
                    CurrentMode = ShadowMediaMode.ShadowMedia3D;
                    break;
                default:
                    break;
            }
        }
    }
}
