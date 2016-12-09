using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.Core
{
    public class ApplicationSettings : MonoBehaviour
    {
        public Canvas _Canvas;

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

        [Header("OnOffを切り替えられる対象")]
        public List<GameObject> Objects = new List<GameObject>();

        public void Start()
        {

            CurrentMode = ShadowMediaMode.ShadowMedia2D;

            (ShadowMediaUIHost.GetUI("core_shadow_media_mode") as ParameterDropdown).ValueChanged += ShadowMediaModeChanged;
            (ShadowMediaUIHost.GetUI("core_switch_objects") as ParameterCheckbox).ValueChanged += SwitchObjectsSwitched;

            Debug.Log("displays connected: " + Display.displays.Length);
            // Display.displays[0] is the primary, default display and is always ON.
            // Check if additional displays are available and activate each.
            if (Display.displays.Length > 1)
                Display.displays[1].Activate();
            if (Display.displays.Length > 2)
                Display.displays[2].Activate();
            if (Display.displays.Length > 3)
                Display.displays[3].Activate();
            (ShadowMediaUIHost.GetUI("core_switch_objects") as ParameterCheckbox).OnValueChanged(false);

        }

        private void SwitchObjectsSwitched(object sender, EventArgs e)
        {
            var value = (e as ParameterCheckbox.ChangedValue).Value;
            foreach(var p in this.Objects)
            {
                p.SetActive(value);
            }
        }

        private void ShadowMediaModeChanged(object sender, EventArgs e)
        {
            CurrentMode = (ShadowMediaMode)(e as ParameterDropdown.ChangedValue).Value;
        }

        public void Update()
        {
            //終了
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            //キャンバスを隠す つける
            if ((Input.GetKey(KeyCode.LeftControl ) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.F))
            {
                this._Canvas.gameObject.SetActive(!this._Canvas.gameObject.activeInHierarchy);
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

        public enum GenericSettingOption
        {
            /// <summary>
            /// 再生するモードの設定
            /// </summary>
            Mode,
            /// <summary>
            /// 数を数える用　消したら処刑
            /// </summary>
            Count
        }
        



    }
}
