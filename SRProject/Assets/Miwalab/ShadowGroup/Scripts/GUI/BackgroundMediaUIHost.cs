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
        public override void Start()
        {
            ResetUI();
            m_MenuList = new List<Dropdown>();
            m_PanelDictionary = new Dictionary<string, GameObject>();
            for (int i = 0; i < (int)CallibrationSettingType.Count; ++i)
            {
                this.CallibrationSettingMenu.options.Add(new Dropdown.OptionData(((CallibrationSettingType)i).ToString()));

            }

            m_MenuList.Add(this.CallibrationSettingMenu);
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

            this.CreateUIsCallibrationImport(m_PanelDictionary[CallibrationSettingType.CallibrationImport.ToString()]);
            this.CreateUIsCallibrationExport(m_PanelDictionary[CallibrationSettingType.CallibrationExport.ToString()]);

            this.m_meshrenderer.SetUpUIs();
            this.CallibrationSettingPanelSet(true);

        }
    }
}
