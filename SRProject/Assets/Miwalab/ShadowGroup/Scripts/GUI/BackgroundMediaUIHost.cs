using Miwalab.ShadowGroup.Scripts.Callibration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Miwalab.ShadowGroup.GUI
{
    public class BackgroundMediaUIHost : ShadowMediaUIHost
    {
        public Dropdown BackRenderCameraSettingMenu;
        private GameObject m_currentBackRenderCameraSettingPanel;
        public override void Start()
        {
            ResetUI();
            m_MenuList = new List<Dropdown>();
            m_PanelDictionary = new Dictionary<string, GameObject>();
            for (int i = 0; i < (int)CallibrationSettingType.Count; ++i)
            {
                this.CallibrationSettingMenu.options.Add(new Dropdown.OptionData(((CallibrationSettingType)i).ToString()));
            }

            for (int i = 0; i < (int)Background.BackRenderCameraSettingType.Count; ++i)
            {
                this.BackRenderCameraSettingMenu.options.Add(new Dropdown.OptionData(((Background.BackRenderCameraSettingType)i).ToString()));
            }

            m_MenuList.Add(this.CallibrationSettingMenu);
            m_MenuList.Add(this.BackRenderCameraSettingMenu);

            foreach (var menu in this.m_MenuList)
            {
                foreach (var option in menu.options)
                {
                    GameObject item = Instantiate(SettingPanel);
                    item.transform.SetParent(MainCanvas.gameObject.transform, false);
                    m_PanelDictionary.Add(option.text, item);
                    item.SetActive(false);
                }
            }
            this.m_currentCallibrationSettingPanel = m_PanelDictionary[CallibrationSettingType.CallibrationImport.ToString()];
            this.m_currentBackRenderCameraSettingPanel = m_PanelDictionary[Background.BackRenderCameraSettingType.BackRenderCamera.ToString()];

            this.CreateUIsCallibrationImport(m_PanelDictionary[CallibrationSettingType.CallibrationImport.ToString()]);
            this.CreateUIsCallibrationExport(m_PanelDictionary[CallibrationSettingType.CallibrationExport.ToString()]);
            this.CreateUIsBackRenderCamera(m_PanelDictionary[Background.BackRenderCameraSettingType.BackRenderCamera.ToString()]);

            this.m_meshrenderer.SetUpUIs();
            this.CallibrationSettingPanelSet(false);
            this.BackRenderCameraSettingPanelSet(true);

        }

        public void BackRenderCameraSettingPanelSet(bool value)
        {
            m_currentBackRenderCameraSettingPanel.SetActive(value);
        }

        private void CreateUIsBackRenderCamera(GameObject parent)
        {
            m_lastUpdatedHeight = 0;
            AddFloatUI(parent, "BRC_POS_X", 20, -20, 0);
            AddFloatUI(parent, "BRC_POS_Y", 20, -20, 0);
            AddFloatUI(parent, "BRC_POS_Z", 20, -20, 0);
            AddFloatUI(parent, "BRC_ROT_X", 180, -180, 0);
            AddFloatUI(parent, "BRC_ROT_Y", 180, -180, 0);
            AddFloatUI(parent, "BRC_ROT_Z", 180, -180, 0);
            AddFloatUI(parent, "BRC_FIELD_VIEW", 179, 1, 60);
        }
    }
}
