using Miwalab.ShadowGroup.Scripts.Callibration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Miwalab.ShadowGroup.GUI
{
    public enum CameraProjectionSettingType
    {
        Fade,
        Count
    }

    public class BackgroundMediaUIHost : ShadowMediaUIHost
    {
        public Dropdown BackRenderCameraSettingMenu;
        public Dropdown BackgroundTypeSettingMenu;
        public Dropdown CameraProjectionSettingMenu;
        private GameObject m_currentBackRenderCameraSettingPanel;
        private GameObject m_currentBackgroundTypeSettingPanel;
        private GameObject m_currentCameraProjectionSettingPanel;
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

            for (int i = 0; i < (int)Background.BackgroundType.Count; ++i)
            {
                this.BackgroundTypeSettingMenu.options.Add(new Dropdown.OptionData(((Background.BackgroundType)i).ToString()));
            }
            for (int i = 0; i < (int)CameraProjectionSettingType.Count; ++i)
            {
                this.CameraProjectionSettingMenu.options.Add(new Dropdown.OptionData(((CameraProjectionSettingType)i).ToString()));
            }

            m_MenuList.Add(this.CallibrationSettingMenu);
            m_MenuList.Add(this.BackRenderCameraSettingMenu);
            m_MenuList.Add(this.BackgroundTypeSettingMenu);
            m_MenuList.Add(this.CameraProjectionSettingMenu);

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
            this.m_currentBackgroundTypeSettingPanel = m_PanelDictionary[Background.BackgroundType.Fish.ToString()];
            this.m_currentCameraProjectionSettingPanel = m_PanelDictionary[CameraProjectionSettingType.Fade.ToString()];


            this.CreateUIsCallibrationImport(m_PanelDictionary[CallibrationSettingType.CallibrationImport.ToString()]);
            this.CreateUIsCallibrationExport(m_PanelDictionary[CallibrationSettingType.CallibrationExport.ToString()]);
            this.CreateUIsBackRenderCamera(m_PanelDictionary[Background.BackRenderCameraSettingType.BackRenderCamera.ToString()]);
            this.CreateUIsBackgroundFish(m_PanelDictionary[Background.BackgroundType.Fish.ToString()]);
            this.CreateUIsBackgroundButterfly(m_PanelDictionary[Background.BackgroundType.Butterfly.ToString()]);
            this.CreateUIsCameraProjectionSetting(m_PanelDictionary[CameraProjectionSettingType.Fade.ToString()]);

            this.m_meshrenderer.SetUpUIs();
            this.CallibrationSettingPanelSet(false);
            this.BackRenderCameraSettingPanelSet(false);
            this.BackgroundTypeSettingPanelSet(true);
        }

        public GameObject ButterflySet;
        public GameObject FishSet;
        public void ChangeBackgroundTypeSettingOptionTo(int number)
        {
            Background.BackgroundType type = (Background.BackgroundType)number;

            switch (type)
            {
                case Background.BackgroundType.Butterfly:
                    //一回作って使いまわす
                    this.m_currentBackgroundTypeSettingPanel = this.m_PanelDictionary[Background.BackgroundType.Butterfly.ToString()];
                    this.SwitchOffOtherPanelsExceptOf(this.m_currentBackgroundTypeSettingPanel);
                    FishSet.SetActive(false);
                    ButterflySet.SetActive(true);
                    break;
                case Background.BackgroundType.Fish:
                    this.m_currentBackgroundTypeSettingPanel = this.m_PanelDictionary[Background.BackgroundType.Fish.ToString()];
                    this.SwitchOffOtherPanelsExceptOf(this.m_currentBackgroundTypeSettingPanel);
                    FishSet.SetActive(true);
                    ButterflySet.SetActive(false);
                    break;

            }

        }

        public void ChangeCameraProjectionSettingOptionTo(int number)
        {
            CameraProjectionSettingType type = (CameraProjectionSettingType)number;

            switch (type)
            {
                case CameraProjectionSettingType.Fade:
                    //一回作って使いまわす
                    this.m_currentBackgroundTypeSettingPanel = this.m_PanelDictionary[CameraProjectionSettingType.Fade.ToString()];
                    this.SwitchOffOtherPanelsExceptOf(this.m_currentBackgroundTypeSettingPanel);
                    break;

            }

        }

        public void BackRenderCameraSettingPanelSet(bool value)
        {
            m_currentBackRenderCameraSettingPanel.SetActive(value);
        }
        public void BackgroundTypeSettingPanelSet(bool value)
        {
            m_currentBackgroundTypeSettingPanel.SetActive(value);
        }
        public void CameraProjectionSettingPanelSet(bool value)
        {
            m_currentCameraProjectionSettingPanel.SetActive(value);
        }

        private void CreateUIsBackgroundButterfly(GameObject parent)
        {
            m_lastUpdatedHeight = 0;
            AddFloatUI(parent, "Butterfly_R", 1f, 0, 1f);
            AddFloatUI(parent, "Butterfly_G", 1f, 0, 1f);
            AddFloatUI(parent, "Butterfly_B", 1f, 0, 1f);
            m_lastUpdatedHeight += 10;
            AddFloatUI(parent, "Butterfly_BG_R", 1f, 0, 0);
            AddFloatUI(parent, "Butterfly_BG_G", 1f, 0, 0);
            AddFloatUI(parent, "Butterfly_BG_B", 1f, 0, 0);
            m_lastUpdatedHeight += 10;
            AddFloatUI(parent, "Particle_Size", 1f, 0.1f, 0.5f);
            AddFloatUI(parent, "Particle_Num", 200, 0, 200);
            m_lastUpdatedHeight += 10;
            AddButtonUI(parent,"Particle_FadeWhite");
            AddButtonUI(parent, "Particle_FadeBlack");
        }

        private void CreateUIsCameraProjectionSetting(GameObject parent)
        {
            m_lastUpdatedHeight = 0;
            AddButtonUI(parent, "CP_1_Fade");
            AddButtonUI(parent, "CP_2_Fade");
        }

        private void CreateUIsBackgroundFish(GameObject parent)
        {
            m_lastUpdatedHeight = 0;
            AddButtonUI(parent, "Fish_FadeOut");
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
