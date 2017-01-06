using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Miwalab.ShadowGroup.ImageProcesser;
using UnityEngine.UI;
using Miwalab.ShadowGroup.Scripts.Sensors;
using Miwalab.ShadowGroup.Callibration;
using Miwalab.ShadowGroup.AfterEffect;
using Miwalab.ShadowGroup.Archive;
using System.Text.RegularExpressions;

public class ShadowMediaUIHost : MonoBehaviour
{

    #region singleton
    private static Dictionary<string, AParameterUI> m_ParameterUI = new Dictionary<string, AParameterUI>();
    protected static float m_lastUpdatedHeight;

    public static void setDebugText(string text)
    {
        GameObject.FindObjectOfType<ShadowMediaUIHost>()._debugText.text = text;
    }

    public static Dictionary<string, AParameterUI> GetInstance()
    {
        return m_ParameterUI;
    }

    public static void AddUI(string key, AParameterUI UIObject)
    {
        m_ParameterUI.Add(key, UIObject);
    }
    public static AParameterUI GetUI(string key)
    {
        return m_ParameterUI[key];
    }

    public static void ResetUI()
    {
        foreach (var p in m_ParameterUI)
        {
            GameObject.DestroyImmediate(p.Value.gameObject);
        }
        m_ParameterUI.Clear();

    }

    public static void LoadAllSettings()
    {
        string str = "";
        OpenFileDialog.OpenFileDialog.Read(ref str);
        if (str == "")
        {
            return;
        }

        Miwalab.ShadowGroup.Data.UIParameterDocument UIPD = new Miwalab.ShadowGroup.Data.UIParameterDocument();
        UIPD.Load(str);
        UIPD.SetParameter(GetInstance());
    }

    public static void SaveAllSettings()
    {
        string str = "";
        OpenFileDialog.OpenFileDialog.Save(ref str);
        if (str == "")
        {
            return;
        }
        Regex reg = new Regex(@"\.SAS$");
        if (!reg.IsMatch(str))
        {
            str += ".SAS";
        }


        Miwalab.ShadowGroup.Data.UIParameterDocument UIPD = new Miwalab.ShadowGroup.Data.UIParameterDocument();
        UIPD.SetupField(GetInstance());
        UIPD.Save(str);
    }
    #endregion

    public ParameterSlider m_slider;
    public ParameterCheckbox m_checkbox;
    public ParameterButton m_button;
    public ParameterText m_text;
    public GameObject m_Dropdown;

    public ASensorImporter m_Sensor;

    public Dropdown ImageProcessingMenu;
    public Dropdown ImportSettingMenu;
    public Dropdown CallibrationSettingMenu;
    public Dropdown AfterEffectSettingMenu;
    public Dropdown ArchiveSettingMenu;
    public Dropdown GenericSettingMenu;
    public Dropdown BackgroundTypeSettingMenu;
    public List<ShadowMeshRenderer> m_meshrenderer = new List<ShadowMeshRenderer>();

    public GameObject SettingPanel;
    public Canvas MainCanvas;

    public Text _debugText;

    public GameObject ButterflySet;
    public GameObject FishSet;
    public GameObject TigerSet;
    public GameObject UnitychanSet;
    private List<GameObject> List_backGround;


    protected List<Dropdown> m_MenuList;
    protected Dictionary<string, GameObject> m_PanelDictionary;
    public virtual void Start()
    {
        ResetUI();
        m_MenuList = new List<Dropdown>();
        m_PanelDictionary = new Dictionary<string, GameObject>();

        for (int i = 0; i < (int)ImageProcesserType.Count; ++i)
        {
            this.ImageProcessingMenu.options.Add(new Dropdown.OptionData(((ImageProcesserType)i).ToString()));
        }
        for (int i = 0; i < (int)ImportSettingType.Count; ++i)
        {
            this.ImportSettingMenu.options.Add(new Dropdown.OptionData(((ImportSettingType)i).ToString()));
        }
        for (int i = 0; i < (int)CallibrationSettingType.Count; ++i)
        {
            this.CallibrationSettingMenu.options.Add(new Dropdown.OptionData(((CallibrationSettingType)i).ToString()));
        }
        for (int i = 0; i < (int)AfterEffectSettingType.Count; ++i)
        {
            this.AfterEffectSettingMenu.options.Add(new Dropdown.OptionData(((AfterEffectSettingType)i).ToString()));
        }
        for (int i = 0; i < (int)ArchiveSettingType.Count; ++i)
        {
            this.ArchiveSettingMenu.options.Add(new Dropdown.OptionData(((ArchiveSettingType)i).ToString()));
        }
        for (int i = 0; i < (int)Miwalab.ShadowGroup.Core.ApplicationSettings.GenericSettingOption.Count; ++i)
        {
            this.GenericSettingMenu.options.Add(new Dropdown.OptionData(((Miwalab.ShadowGroup.Core.ApplicationSettings.GenericSettingOption)i).ToString()));
        }
        for (int i = 0; i < (int)Miwalab.ShadowGroup.Background.BackgroundType.Count; ++i)
        {
            this.BackgroundTypeSettingMenu.options.Add(new Dropdown.OptionData(((Miwalab.ShadowGroup.Background.BackgroundType)i).ToString()));
        }

        m_MenuList.Add(this.ImageProcessingMenu);
        m_MenuList.Add(this.ImportSettingMenu);
        m_MenuList.Add(this.CallibrationSettingMenu);
        m_MenuList.Add(this.AfterEffectSettingMenu);
        m_MenuList.Add(this.ArchiveSettingMenu);
        m_MenuList.Add(this.GenericSettingMenu);
        m_MenuList.Add(this.BackgroundTypeSettingMenu);

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
        this.m_currentImageProcesserSettingPanel = m_PanelDictionary[ImageProcesserType.Normal.ToString()];
        this.m_currentImportSettingPanel = m_PanelDictionary[ImportSettingType.Kinect.ToString()];
        this.m_currentCallibrationSettingPanel = m_PanelDictionary[CallibrationSettingType.CallibrationImport1.ToString()];
        this.m_currentAfterEffectSettingPanel = m_PanelDictionary[AfterEffectSettingType.Fade.ToString()];
        this.m_currentArchiveSettingPanel = m_PanelDictionary[ArchiveSettingType.Save.ToString()];
        this.m_currentGenericSettingPanel = m_PanelDictionary[Miwalab.ShadowGroup.Core.ApplicationSettings.GenericSettingOption.Mode.ToString()];
        this.m_currentBackgroundTypeSettingPanel = m_PanelDictionary[Miwalab.ShadowGroup.Background.BackgroundType.None.ToString()];


        //UI初期化
        this.SetupUIsImageprocess();





        this.CreateUIsImportKinectv2(m_PanelDictionary[ImportSettingType.Kinect.ToString()]);
        this.CreateUIsImportKinectAditional(m_PanelDictionary[ImportSettingType.Kinect_Aditional.ToString()]);
        this.CreateUIsImportNetwork(m_PanelDictionary[ImportSettingType.Network.ToString()]);

        this.CreateUIsCallibrationImport(m_PanelDictionary[CallibrationSettingType.CallibrationImport1.ToString()], 1);
        this.CreateUIsCallibrationExport(m_PanelDictionary[CallibrationSettingType.CallibrationExport1.ToString()], 1);
        this.CreateUIsCallibrationImport(m_PanelDictionary[CallibrationSettingType.CallibrationImport2.ToString()], 2);
        this.CreateUIsCallibrationExport(m_PanelDictionary[CallibrationSettingType.CallibrationExport2.ToString()], 2);
        this.CreateUIsCallibrationImport(m_PanelDictionary[CallibrationSettingType.CallibrationImport3.ToString()], 3);
        this.CreateUIsCallibrationExport(m_PanelDictionary[CallibrationSettingType.CallibrationExport3.ToString()], 3);
        this.CreateUIsCallibrationImport(m_PanelDictionary[CallibrationSettingType.CallibrationImport4.ToString()], 4);
        this.CreateUIsCallibrationExport(m_PanelDictionary[CallibrationSettingType.CallibrationExport4.ToString()], 4);

        this.CreateUIsCameraCllibration(m_PanelDictionary[CallibrationSettingType.CameraCllibration1.ToString()], 1);
        this.CreateUIsCameraCllibration(m_PanelDictionary[CallibrationSettingType.CameraCllibration2.ToString()], 2);


        this.CreateUIsAffterEffectFade(m_PanelDictionary[AfterEffectSettingType.Fade.ToString()]);

        this.CreateUIsArchiveSave(m_PanelDictionary[ArchiveSettingType.Save.ToString()]);
        this.CreateUIsArchivePlay(m_PanelDictionary[ArchiveSettingType.Play.ToString()]);
        this.CreateUIsArchiveRobot(m_PanelDictionary[ArchiveSettingType.Robot.ToString()]);

        this.CreateUIsBackgroundNone(m_PanelDictionary[Miwalab.ShadowGroup.Background.BackgroundType.None.ToString()]);
        this.CreateUIsBackgroundCircleCut(m_PanelDictionary[Miwalab.ShadowGroup.Background.BackgroundType.CircleCut.ToString()]);
        this.CreateUIsBackgroundButterfly(m_PanelDictionary[Miwalab.ShadowGroup.Background.BackgroundType.Butterfly.ToString()]);
        this.CreateUIsBackgroundFish(m_PanelDictionary[Miwalab.ShadowGroup.Background.BackgroundType.Fish.ToString()]);
        this.CreateUIsBackgroundTiger(m_PanelDictionary[Miwalab.ShadowGroup.Background.BackgroundType.Tiger.ToString()]);
        this.CreateUIsBackgroundUnitychan(m_PanelDictionary[Miwalab.ShadowGroup.Background.BackgroundType.Unitychan.ToString()]);
        this.CreateUIsGeneric();
        //代入の順番はGenericに合わせてください
        this.List_backGround = new List<GameObject>() { this.ButterflySet,this.FishSet,this.TigerSet,this.UnitychanSet };

        this.m_meshrenderer.ForEach(p => p.SetUpUIs());
        this.m_Sensor.setUpUI();

        this.ImageProcesserSettingPanelSet(false);
        this.ImportSettingPanelSet(false);
        this.CallibrationSettingPanelSet(false);
        this.m_Sensor.AddImageProcesser(new Normal());
    }


    private void CreateUIsGeneric()
    {
        this.CreateUIsGenericModes(m_PanelDictionary[Miwalab.ShadowGroup.Core.ApplicationSettings.GenericSettingOption.Mode.ToString()]);
    }

   

    private void SetupUIsImageprocess()
    {
        this.CreateUIsImageporcessingWhite(m_PanelDictionary[ImageProcesserType.White.ToString()]);
        this.CreateUIsImageporcessingNormal(m_PanelDictionary[ImageProcesserType.Normal.ToString()]);
        this.CreateUIsImageporcessingTimeDelay(m_PanelDictionary[ImageProcesserType.TimeDelay.ToString()]);
        this.CreateUIsImageporcessingZanzou(m_PanelDictionary[ImageProcesserType.DoubleAfterImage.ToString()]);

        this.CreateUIsImageporcessingSpike(m_PanelDictionary[ImageProcesserType.Spike.ToString()]);
        this.CreateUIsImageporcessingPolygon(m_PanelDictionary[ImageProcesserType.Polygon.ToString()]);
        this.CreateUIsImageporcessingTamuraSkeleton(m_PanelDictionary[ImageProcesserType.TamuraSkeleton.ToString()]);
        this.CreateUIsImageporcessingParticle(m_PanelDictionary[ImageProcesserType.Particle.ToString()]);
        this.CreateUIsImageporcessingBlack(m_PanelDictionary[ImageProcesserType.Black.ToString()]);
        this.CreateUIsImageporcessingParticle2D(m_PanelDictionary[ImageProcesserType.Particle2D.ToString()]);
        this.CreateUIsImageporcessingParticleVector(m_PanelDictionary[ImageProcesserType.ParticleVector.ToString()]);
        this.CreateUIsImageporcessingAttraction(m_PanelDictionary[ImageProcesserType.Attraction.ToString()]);
        this.CreateUIsImageporcessingElectrical(m_PanelDictionary[ImageProcesserType.Electrical.ToString()]);
        this.CreateUIsImageporcessingHandsTo(m_PanelDictionary[ImageProcesserType.HandsTo.ToString()]);
        this.CreateUIsImageporcessingHandElbow(m_PanelDictionary[ImageProcesserType.HandElbow.ToString()]);
        this.CreateUIsImageporcessingMoveShadow(m_PanelDictionary[ImageProcesserType.MoveShadow.ToString()]);
        this.CreateUIsImageporcessingTwins(m_PanelDictionary[ImageProcesserType.Twins.ToString()]);
        this.CreateUIsImageporcessingAhead(m_PanelDictionary[ImageProcesserType.Ahead.ToString()]);
        this.CreateUIsImageporcessingColorful(m_PanelDictionary[ImageProcesserType.Colorful.ToString()]);
        this.CreateUIsImageporcessingCanny(m_PanelDictionary[ImageProcesserType.Canny.ToString()]);
        this.CreateUIsImageporcessingLeastSquare(m_PanelDictionary[ImageProcesserType.LeastSquare.ToString()]);
        this.CreateUIsImageporcessingLSAhead(m_PanelDictionary[ImageProcesserType.LSAhead.ToString()]);
        this.CreateUIsImageporcessingMixColor(m_PanelDictionary[ImageProcesserType.MixColor.ToString()]);
        this.CreateUIsImageporcessingChangeColor(m_PanelDictionary[ImageProcesserType.ChangeColor.ToString()]);
        this.CreateUIsImageporcessingPersonalColor(m_PanelDictionary[ImageProcesserType.PersonalColor.ToString()]);
        this.CreateUIsImageporcessingPtsImgProcesser(m_PanelDictionary[ImageProcesserType.PtsImgProcesser.ToString()]);
        this.CreateUIsImageporcessingBrightCheck(m_PanelDictionary[ImageProcesserType.BrightCheck.ToString()]);
        this.CreateUIsImageporcessingEachMoveParticle(m_PanelDictionary[ImageProcesserType.EachMoveParticle.ToString()]);
        this.CreateUIsImageporcessingFlowParticlesShadow(m_PanelDictionary[ImageProcesserType.FlowParticlesShadow.ToString()]);
        this.CreateUIsImageporcessingPainterShadow(m_PanelDictionary[ImageProcesserType.PainterShadow.ToString()]);

    }

    public void ChangeGeneralSettingOptionTo(int number)
    {
        var type = (Miwalab.ShadowGroup.Core.ApplicationSettings.GenericSettingOption)number;
        switch (type)
        {
            case Miwalab.ShadowGroup.Core.ApplicationSettings.GenericSettingOption.Mode:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new Normal() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Normal.ToString()];
                break;
    }
        this.SwitchOffOtherPanelsExceptOf(this.m_currentGenericSettingPanel);

    }
    

    public void ChangeImageProcessingOptionTo(int number)
    {
        ImageProcesserType type = (ImageProcesserType)number;
        switch (type)
        {
            case ImageProcesserType.Normal:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new Normal() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Normal.ToString()];
                break;
            case ImageProcesserType.VividNormal:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new VividNormal() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.VividNormal.ToString()];
                break;
            case ImageProcesserType.Spike:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Spike() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Spike.ToString()];
                break;
            case ImageProcesserType.Black:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Black() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Black.ToString()];
                break;
            case ImageProcesserType.White:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new White() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.White.ToString()];
                break;
            case ImageProcesserType.DoubleAfterImage:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Zanzou() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.DoubleAfterImage.ToString()];
                break;
            case ImageProcesserType.Polygon:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Polygon() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Polygon.ToString()];
                break;
            case ImageProcesserType.TamuraSkeleton:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new TamuraSkelton() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.TamuraSkeleton.ToString()];
                break;
            case ImageProcesserType.TimeDelay:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Timedelay() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.TimeDelay.ToString()];
                break;
            case ImageProcesserType.Particle:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Miwalab.ShadowGroup.ImageProcesser.Particle() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Particle.ToString()];
                break;
            case ImageProcesserType.Particle2D:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Miwalab.ShadowGroup.ImageProcesser.ShadowParticle2D() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Particle2D.ToString()];
                break;
            case ImageProcesserType.ParticleVector:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Miwalab.ShadowGroup.ImageProcesser.ParticleVector() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.ParticleVector.ToString()];
                break;
            case ImageProcesserType.Attraction:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Attraction() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Attraction.ToString()];
                break;
            case ImageProcesserType.Electrical:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Electrical() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Electrical.ToString()];
                break;
            case ImageProcesserType.HandsTo:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new HandsTo() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.HandsTo.ToString()];
                break;
            case ImageProcesserType.HandElbow:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new HandElbow() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.HandElbow.ToString()];
                break;
            case ImageProcesserType.MoveShadow:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new MoveShadow() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.MoveShadow.ToString()];
                break;
            case ImageProcesserType.Twins:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Twins() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Twins.ToString()];
                break;
            case ImageProcesserType.Ahead:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Ahead() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Ahead.ToString()];
                break;
            case ImageProcesserType.Colorful:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Colorful() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Colorful.ToString()];
                break;
            case ImageProcesserType.Canny:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Canny() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Canny.ToString()];
                break;
            case ImageProcesserType.LeastSquare:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new LeastSquare() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.LeastSquare.ToString()];
                break;
            case ImageProcesserType.LSAhead:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(), new LSAhead() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.LSAhead.ToString()];
                break;
            case ImageProcesserType.MixColor:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new MixColor() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.MixColor.ToString()];
                break;
            case ImageProcesserType.PersonalColor:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new PersonalColor() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.PersonalColor.ToString()];
                break;
            case ImageProcesserType.ChangeColor:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new ChangeColor() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.ChangeColor.ToString()];
                break;
            case ImageProcesserType.PtsImgProcesser:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new PtsImgProcesser() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.PtsImgProcesser.ToString()];
                break;
            case ImageProcesserType.BrightCheck:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new BrightCheck() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.BrightCheck.ToString()];
                break;
            case ImageProcesserType.EachMoveParticle:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Miwalab.ShadowGroup.ImageProcesser.EachMoveParticle() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.EachMoveParticle.ToString()];
                break;
            case ImageProcesserType.FlowParticlesShadow:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Miwalab.ShadowGroup.ImageProcesser.FlowParticlesShadow() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.FlowParticlesShadow.ToString()];
                break;
            case ImageProcesserType.PainterShadow:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new List<AShadowImageProcesser>() { new PtsImgProcesser(),new Miwalab.ShadowGroup.ImageProcesser.PainterShadow() }));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.PainterShadow.ToString()];
                break;
            case ImageProcesserType.CellAutomaton:
                break;

        }
        this.SwitchOffOtherPanelsExceptOf(this.m_currentImageProcesserSettingPanel);

    }
    public void ChangeImportSettingOptionTo(int number)
    {
        ImportSettingType type = (ImportSettingType)number;

        this.m_currentImportSettingPanel = this.m_PanelDictionary[type.ToString()];
        this.SwitchOffOtherPanelsExceptOf(this.m_currentImportSettingPanel);
    }

    public void ChangeCallibrationSettingOptionTo(int number)
    {
        CallibrationSettingType type = (CallibrationSettingType)number;
        this.m_currentCallibrationSettingPanel = this.m_PanelDictionary[type.ToString()];
        this.SwitchOffOtherPanelsExceptOf(this.m_currentCallibrationSettingPanel);
    }

    public void ChangeFadeSettingOptionTo(int number)
    {
        AfterEffectSettingType type = (AfterEffectSettingType)number;
        this.m_currentCallibrationSettingPanel = this.m_PanelDictionary[type.ToString()];
        this.SwitchOffOtherPanelsExceptOf(this.m_currentAfterEffectSettingPanel);
    }
    public void ChangeArchiveSettingOptionTo(int number)
    {
        ArchiveSettingType type = (ArchiveSettingType)number;
        this.m_currentArchiveSettingPanel = this.m_PanelDictionary[type.ToString()];
        this.SwitchOffOtherPanelsExceptOf(this.m_currentArchiveSettingPanel);
    }

    protected void SwitchOffOtherPanelsExceptOf(GameObject currentPanel)
    {
        foreach (var panel in m_PanelDictionary)
        {
            panel.Value.SetActive(false);
        }
        currentPanel.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    #region createUIMethods

    private void CreateUIsImageporcessingPainterShadow(GameObject gameObject)
    {
        m_lastUpdatedHeight = 0;
        
    }

    private void CreateUIsArchiveSave(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddButtonUI(parent, "ChooseDepthSaveFile");
        AddButtonUI(parent, "SaveDepthStart");
        AddButtonUI(parent, "SaveDepthStop");

        AddButtonUI(parent, "ChooseCameraSaveFile");
        AddButtonUI(parent, "SaveCameraStart");
        AddButtonUI(parent, "SaveCameraStop");

    }
    private void CreateUIsArchivePlay(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddButtonUI(parent, "ACV_ChooseFile");
        AddButtonUI(parent, "ACV_PlayStart");
        AddBooleanUI(parent, "ACV_Pause", false);
        AddBooleanUI(parent, "ACV_Robot", false);

    }
    private void CreateUIsArchiveRobot(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddButtonUI(parent, "ChooseRobotFile");
        AddButtonUI(parent, "PlayRobotStart");
        AddBooleanUI(parent, "RobotSet", false);

        AddFloatUI(parent, "RobotOffset_X", 8, -8, 0);
        AddFloatUI(parent, "RobotOffset_Y", 3, -3, 0);
        AddFloatUI(parent, "RobotOffset_Z", 15, -15, 0);

    }

    private void CreateUIsImageporcessingParticle(GameObject parent)
    {
        m_lastUpdatedHeight = 0;

        AddFloatUI(parent, "Interval_of_Contour", 100, 1, 5);
        AddFloatUI(parent, "Velocity", 100, 0, 10);
        AddFloatUI(parent, "Lifetime_Frame", 100, 1, 30);
        AddFloatUI(parent, "threthOPFsize", 5000, 0, 1500);
        AddFloatUI(parent, "Particle_bgd_R", 255, 0, 255);
        AddFloatUI(parent, "Particle_bgd_G", 255, 0, 255);
        AddFloatUI(parent, "Particle_bgd_B", 255, 0, 255);

    }

    private void CreateUIsImageporcessingParticle2D(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "P2D_Max_V", 50, 0, 20);
        AddFloatUI(parent, "P2D_Size_Max", 20, -1, 1);
        AddFloatUI(parent, "P2D_Size_Min", 20, -1, 0);
        m_lastUpdatedHeight += 10;

        AddFloatUI(parent, "P2D_Num_Init", 10000, 1000, 1000);
        AddFloatUI(parent, "P2D_Center_X", 1f, -1f, 0);
        AddFloatUI(parent, "P2D_Center_Y", 1f, -1f, 0);
    }

    private void CreateUIsImageporcessingEachMoveParticle(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "EMP_Num_Init", 10000, 1000, 1000);
        AddFloatUI(parent, "EMP_Size_Max", 20, -1, 1);
        AddFloatUI(parent, "EMP_Size_Min", 20, -1, 0);
        AddFloatUI(parent, "EMP_Max_V", 20, 0, 0.1f);
        m_lastUpdatedHeight += 5;
        AddBooleanUI(parent, "EMP_UseOwnMove", true);
        AddFloatUI(parent, "EMP_Response", 50, 1, 20);
        m_lastUpdatedHeight += 5;
        AddFloatUI(parent, "EMP_VK", 5, -5, 0);
        m_lastUpdatedHeight += 5;
        AddBooleanUI(parent, "EMP_ColorUse", true);
    }

    private void CreateUIsImageporcessingFlowParticlesShadow(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "FP_Num_Init", 10000, 1000, 1000);
        AddFloatUI(parent, "FP_Size_Max", 20, -1, 1);
        AddFloatUI(parent, "FP_Size_Min", 20, -1, 0);
        AddFloatUI(parent, "FP_Max_V", 20, 0, 0.1f);
        m_lastUpdatedHeight += 5;
        AddBooleanUI(parent, "FP_ColorUse", true);
        m_lastUpdatedHeight += 5;
        AddFloatUI(parent, "FP_Force_X", 1, -1, 0);
        AddFloatUI(parent, "FP_Force_Y", 1, -1, 0);


    }


    private void CreateUIsImageporcessingParticleVector(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "PV_Num_Init", 10000, 1000, 1000);

        AddFloatUI(parent, "PV_Size_Max", 20, -1, 1);
        AddFloatUI(parent, "PV_Size_Min", 20, -1, 0);

        AddBooleanUI(parent, "PV_UseShadowImage", false);
        AddBooleanUI(parent, "PV_UseAvarage", false);
        AddBooleanUI(parent, "PV_UseFade", false);

        AddBooleanUI(parent, "PV_DebugMode", false);

        AddButtonUI(parent, "PV_Reset");
    }

    private void CreateUIsAffterEffectFade(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Frame_of_FadeIn", 1000, 1, 50);
        AddFloatUI(parent, "Frame_of_FadeOut", 1000, 1, 50);
        AddBooleanUI(parent, "White_Fade", false);
    }

    private void CreateUIsImageporcessingHandsTo(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "HandsTo_con_R", 255, 0, 0);
        AddFloatUI(parent, "HandsTo_con_G", 255, 0, 0);
        AddFloatUI(parent, "HandsTo_con_B", 255, 0, 200);
        AddFloatUI(parent, "HandsTo_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "HandsTo_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "HandsTo_bgd_B", 255, 0, 0);
        AddFloatUI(parent, "HandsTo_moveTh", 500, 0, 500);
        AddFloatUI(parent, "HandsTo_centerCtl", 100, 0, 0);
    }
    private void CreateUIsImageporcessingAttraction(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Attraction_con_R", 255, 0, 0);
        AddFloatUI(parent, "Attraction_con_G", 255, 0, 0);
        AddFloatUI(parent, "Attraction_con_B", 255, 0, 200);
        AddFloatUI(parent, "Attraction_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "Attraction_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "Attraction_bgd_B", 255, 0, 0);
        AddFloatUI(parent, "Attraction_ctl", 1, 0, 0.1f);
        AddFloatUI(parent, "Attraction_deleteTh", 640, 0, 500);
        AddFloatUI(parent, "Attraction_Rate", 40, 1, 6);

    }

    private void CreateUIsImageporcessingElectrical(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Electrical_con_R", 255, 0, 0);
        AddFloatUI(parent, "Electrical_con_G", 255, 0, 0);
        AddFloatUI(parent, "Electrical_con_B", 255, 0, 200);
        AddFloatUI(parent, "Electrical_bgd_R", 255, 0, 255);
        AddFloatUI(parent, "Electrical_bgd_G", 255, 0, 255);
        AddFloatUI(parent, "Electrical_bgd_B", 255, 0, 255);
        AddFloatUI(parent, "Electrical_ctl", 1, 0, 0.1f);
        AddFloatUI(parent, "Electrical_deleteTh", 500, 0, 50);
        AddFloatUI(parent, "Electrical_Rate", 100, 5, 50);
        AddBooleanUI(parent, "Electrical_UseFade", true);

    }

    private void CreateUIsImageporcessingHandElbow(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "HandElbow_con_R", 255, 0, 0);
        AddFloatUI(parent, "HandElbow_con_G", 255, 0, 0);
        AddFloatUI(parent, "HandElbow_con_B", 255, 0, 200);
        AddFloatUI(parent, "HandElbow_rot_S", 3, -3, 0);
        AddFloatUI(parent, "HandElbow_rot_B", 20, 5, 5);
        AddFloatUI(parent, "HandElbow_bodyThick", 1, 0, 0.8f);
        AddFloatUI(parent, "HandElbow_Rate", 21, 1, 5);
        AddFloatUI(parent, "HandElbow_CtlRate", 3, -3, 0);
        AddFloatUI(parent, "HandElbow_speedRate", 50, -50, 10);
        AddBooleanUI(parent, "HandElbow_UseFade", false);
        AddBooleanUI(parent, "HandElbow_UseExa", false);
        AddBooleanUI(parent, "HandElbow_UseBec", false);
        AddBooleanUI(parent, "HandElbow_UseAtt", false);
        m_lastUpdatedHeight += 10;
    
    }
    private void CreateUIsImageporcessingMoveShadow(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "MoveShadow_con_R", 255, 0, 0);
        AddFloatUI(parent, "MoveShadow_con_G", 255, 0, 0);
        AddFloatUI(parent, "MoveShadow_con_B", 255, 0, 200);
        AddFloatUI(parent, "MoveShadow_rot_S", 3, -3, 0);
        AddFloatUI(parent, "MoveShadow_rot_B", 3, -3, 0);
        AddFloatUI(parent, "MoveShadow_bodyThick", 1, 0, 0.8f);
        AddFloatUI(parent, "MoveShadow_AttRate", 20, 1, 5);
        AddBooleanUI(parent, "MoveShadow_UseFade", false);
        AddBooleanUI(parent, "MoveShadow_UseSwp", false);
        AddBooleanUI(parent, "MoveShadow_UseSwpAcc", false);
        AddBooleanUI(parent, "MoveShadow_UseRot", false);
        AddBooleanUI(parent, "MoveShadow_UseAdd", false);
        AddBooleanUI(parent, "MoveShadow_UseDiv", false);
        AddFloatUI(parent, "MoveShadow_CtlRate", 10, 0.5f, 1);
        AddFloatUI(parent, "MoveShadow_speedRate", 50, -50, 10);
        m_lastUpdatedHeight += 10;

    }
    private void CreateUIsImageporcessingTwins(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Twins_con_R", 255, 0, 0);
        AddFloatUI(parent, "Twins_con_G", 255, 0, 0);
        AddFloatUI(parent, "Twins_con_B", 255, 0, 200);
        AddFloatUI(parent, "Twins_bodyThick", 1, 0, 0.8f);
        AddFloatUI(parent, "Twins_AttRate", 20, 1, 5);
        AddFloatUI(parent, "Twins_AccRate", 10, 0.5f, 1);
        AddBooleanUI(parent, "Twins_UseFade", false);
        AddBooleanUI(parent, "Twins_UseDiv", false);
        AddBooleanUI(parent, "Twins_UseAve", false);
        AddBooleanUI(parent, "Twins_UseSwpRot", false);
        AddBooleanUI(parent, "Twins_UseSwpAcc", false);
        AddBooleanUI(parent, "Twins_UseRot", false);
        AddBooleanUI(parent, "Twins_UseAtt", false);
        AddBooleanUI(parent, "Twins_Reverce", false);
        AddBooleanUI(parent, "Twins_Bright", false);
        AddBooleanUI(parent, "Twins_AddImg", false);
        m_lastUpdatedHeight += 10;

    }

    private void CreateUIsImageporcessingAhead(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Ahead_con_R", 255, 0, 0);
        AddFloatUI(parent, "Ahead_con_G", 255, 0, 0);
        AddFloatUI(parent, "Ahead_con_B", 255, 0, 200);
        AddFloatUI(parent, "Ahead_bgd_R", 255, 0, 255);
        AddFloatUI(parent, "Ahead_bgd_G", 255, 0, 255);
        AddFloatUI(parent, "Ahead_bgd_B", 255, 0, 255);
        AddFloatUI(parent, "Ahead_preRotRate", 5, 1, 2);
        AddFloatUI(parent, "Ahead_preMoveRate", 10, 0, 1);
        AddBooleanUI(parent, "Ahead_MoveMax", false);
        AddBooleanUI(parent, "Ahead_Fill", true);
        AddBooleanUI(parent, "Ahead_Skelton", false);
        AddBooleanUI(parent, "Ahead_UseFade", false);
        AddBooleanUI(parent, "Ahead_UseDiv", false);
        AddBooleanUI(parent, "Ahead_UseRot", false);
        AddBooleanUI(parent, "Ahead_UsePre", false);
        AddBooleanUI(parent, "Ahead_AddImg", true);
        m_lastUpdatedHeight += 10;


    }

    private void CreateUIsImageporcessingColorful(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Colorful_con_R", 255, 0, 0);
        AddFloatUI(parent, "Colorful_con_G", 255, 0, 0);
        AddFloatUI(parent, "Colorful_con_B", 255, 0, 200);
        AddFloatUI(parent, "Colorful_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "Colorful_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "Colorful_bgd_B", 255, 0, 0);
        AddFloatUI(parent, "Colorful_change_x", 100, -100, 0);
        AddFloatUI(parent, "Colorful_change_y", 100, -100, 0);
        AddFloatUI(parent, "Colorful_Rate", 200, 1, 100);
        AddBooleanUI(parent, "Colorful_UseFade", true);
        m_lastUpdatedHeight += 10;

        AddButtonUI(parent, "Colorful_CC_Blue");
        AddButtonUI(parent, "Colorful_CC_Orange");
        AddButtonUI(parent, "Colorful_CC_Yellow");
        AddButtonUI(parent, "Colorful_CC_Pink");
        AddButtonUI(parent, "Colorful_CC_Green");

    }


    private void CreateUIsImageporcessingTamuraSkeleton(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddBooleanUI(parent, "TamuraSkeleton_Invert", false);
    }
    private void CreateUIsImageporcessingBlack(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
    }
    private void CreateUIsImageporcessingWhite(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
    }
    private void CreateUIsImageporcessingZanzou(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Zanzou_ins_R", 255, 0, 255);
        AddFloatUI(parent, "Zanzou_ins_G", 255, 0, 255);
        AddFloatUI(parent, "Zanzou_ins_B", 255, 0, 255);
        AddFloatUI(parent, "Zanzou_out_R", 255, 0, 0);
        AddFloatUI(parent, "Zanzou_out_G", 255, 0, 255);
        AddFloatUI(parent, "Zanzou_out_B", 255, 0, 0);
        AddFloatUI(parent, "Zanzou_in_tm", 1, 0, 0.2f);
        AddFloatUI(parent, "Zanzou_ou_tm", 10, 0, 10);
        AddFloatUI(parent, "Zanzou_param", 1000, 0, 230);
        AddBooleanUI(parent, "Zanzou_Invert", false);

        m_lastUpdatedHeight += 10;
        //色指定
        AddButtonUI(parent, "Zanzou_CC_Orange");
        AddButtonUI(parent, "Zanzou_CC_Pink");
        AddButtonUI(parent, "Zanzou_CC_Yellow");
        AddButtonUI(parent, "Zanzou_CC_Blue");
        AddButtonUI(parent, "Zanzou_CC_Green");

    }

    private void CreateUIsImageporcessingPolygon(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Polygon_con_R", 255, 0, 0);
        AddFloatUI(parent, "Polygon_con_G", 255, 0, 0);
        AddFloatUI(parent, "Polygon_con_B", 255, 0, 200);
        AddFloatUI(parent, "Polygon_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "Polygon_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "Polygon_bgd_B", 255, 0, 0);
        AddFloatUI(parent, "Polygon_change_x", 100, -100, 0);
        AddFloatUI(parent, "Polygon_change_y", 100, -100, 0);
        AddFloatUI(parent, "Polygon_Rate", 40, 1, 6);
        AddBooleanUI(parent, "Polygon_UseFade", true);
        m_lastUpdatedHeight += 10;

        AddButtonUI(parent, "Polygon_CC_Blue");
        AddButtonUI(parent, "Polygon_CC_Orange");
        AddButtonUI(parent, "Polygon_CC_Yellow");
        AddButtonUI(parent, "Polygon_CC_Pink");
        AddButtonUI(parent, "Polygon_CC_Green");

    }

    private void CreateUIsImageporcessingPtsImgProcesser(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddEnumUI(parent, "PtsImg_Type", ImageProcesserType.Normal);
        AddFloatUI(parent, "PtsImg_Dilate", 10, 0, 0);
        AddFloatUI(parent, "PtsImg_GaussianSize1", 150, 1, 1);
        AddFloatUI(parent, "PtsImg_LightAnd", 200, 0, 1);
        AddFloatUI(parent, "PtsImg_GaussianSize2", 150, 1, 1);
        AddFloatUI(parent, "PtsImg_Threshold", 254, 0, 0);
        AddFloatUI(parent, "PtsImg_findAreaTh", 1000, 0, 100);
        AddFloatUI(parent, "PtsImg_Rate", 200, 1, 60);
        AddBooleanUI(parent, "PtsImg_UseFade", false);
        AddBooleanUI(parent, "PtsImg_contFind", false);
        m_lastUpdatedHeight += 10;
    }

    private void CreateUIsImageporcessingBrightCheck(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Bright_bright", 255, 0, 255);
        AddFloatUI(parent, "Bright_pt_B_x", 600, -100, 200);
        AddFloatUI(parent, "Bright_pt_B_y", 600, -100, 200);
        AddFloatUI(parent, "Bright_pt_D_x", 600, -100, 200);
        AddFloatUI(parent, "Bright_pt_D_y", 600, -100, 200);
        AddFloatUI(parent, "Bright_rad_x" ,1,0,1);
        AddFloatUI(parent, "Bright_rad_y", 1, 0, 1);
        AddFloatUI(parent, "Bright_rad_size", 100, 1, 50);

        AddFloatUI(parent, "Bright_angle", 360, -360, 360);
        AddBooleanUI(parent, "Bright_vsbGLD", true);
        AddBooleanUI(parent, "Bright_vsbTxt", true);
        m_lastUpdatedHeight += 10;
    }

    private void CreateUIsImageporcessingCanny(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Canny_con_R", 255, 0, 0);
        AddFloatUI(parent, "Canny_con_G", 255, 0, 0);
        AddFloatUI(parent, "Canny_con_B", 255, 0, 200);
        AddFloatUI(parent, "Canny_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "Canny_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "Canny_bgd_B", 255, 0, 0);
        AddFloatUI(parent, "Canny_Rate", 200, 50, 100);
        AddFloatUI(parent, "Canny_Speed", 1, 0.01f, 0.5f);
        AddBooleanUI(parent, "Canny_UseFade", true);
        m_lastUpdatedHeight += 10;

        AddButtonUI(parent, "Canny_CC_Blue");
        AddButtonUI(parent, "Canny_CC_Orange");
        AddButtonUI(parent, "Canny_CC_Yellow");
        AddButtonUI(parent, "Canny_CC_Pink");
        AddButtonUI(parent, "Canny_CC_Green");

    }

    private void CreateUIsImageporcessingLeastSquare(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "LeastSquare_con_R", 255, 0, 0);
        AddFloatUI(parent, "LeastSquare_con_G", 255, 0, 0);
        AddFloatUI(parent, "LeastSquare_con_B", 255, 0, 200);
        AddFloatUI(parent, "LeastSquare_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "LeastSquare_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "LeastSquare_bgd_B", 255, 0, 0);
        AddFloatUI(parent, "LeastSquare_Rate", 200, 10, 100);
        AddFloatUI(parent, "LeastSquare_useFrame", 20, 2, 8);
        AddFloatUI(parent, "LeastSquare_preFrame", 20, 0, 4);
        AddBooleanUI(parent, "LeastSquare_UseFade", false);
        m_lastUpdatedHeight += 10;

    }

    private void CreateUIsImageporcessingLSAhead(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "LSAhead_con_R", 255, 0, 0);
        AddFloatUI(parent, "LSAhead_con_G", 255, 0, 0);
        AddFloatUI(parent, "LSAhead_con_B", 255, 0, 200);
        AddFloatUI(parent, "LSAhead_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "LSAhead_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "LSAhead_bgd_B", 255, 0, 0);
        AddFloatUI(parent, "LSAhead_Rate", 200, 10, 100);
        AddFloatUI(parent, "LSAhead_useFrame", 40, 2, 15);
        AddFloatUI(parent, "LSAhead_preFrame", 20, 0, 7);
        AddBooleanUI(parent, "LSAhead_UseFade", false);
        AddBooleanUI(parent, "LSAhead_useCubeCurve", false);
        AddBooleanUI(parent, "LSAhead_rec", false);
        m_lastUpdatedHeight += 10;

    }

    private void CreateUIsImageporcessingMixColor(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "MixColor_con_R", 255, 0, 0);
        AddFloatUI(parent, "MixColor_con_G", 255, 0, 0);
        AddFloatUI(parent, "MixColor_con_B", 255, 0, 200);
        AddFloatUI(parent, "MixColor_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "MixColor_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "MixColor_bgd_B", 255, 0, 0);
        AddFloatUI(parent, "MixColor_Rate", 200, 10, 100);
        AddFloatUI(parent, "MixColor_Speed", 0.5f, 0, 0.2f);
        AddFloatUI(parent, "MixColor_Mix", 100, 1, 50);
        AddBooleanUI(parent, "MixColor_UseFade", false);
        m_lastUpdatedHeight += 10;
    }

    private void CreateUIsImageporcessingPersonalColor(GameObject parent)
    {
        m_lastUpdatedHeight = 0;

        AddBooleanUI(parent, "PersonalColor_UseFade", false);
        AddFloatUI(parent, "PersonalColor_minArea", 1000, 0, 500);
        m_lastUpdatedHeight += 10;
    }

    private void CreateUIsImageporcessingChangeColor(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "ChangeColor_con_R", 255, 0, 0);
        AddFloatUI(parent, "ChangeColor_con_G", 255, 0, 0);
        AddFloatUI(parent, "ChangeColor_con_B", 255, 0, 200);

        AddBooleanUI(parent, "ChangeColor_UseFade", false);
        AddBooleanUI(parent, "ChangeColor_UseBlur", false);
        m_lastUpdatedHeight += 10;
    }

    private void CreateUIsImageporcessingSpike(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Spike_inval", 100, 1, 10);
        AddFloatUI(parent, "Spike_lngth", 50, 1, 5);
        AddFloatUI(parent, "Spike_rdius", 5, 1, 1);
        AddFloatUI(parent, "Spike_con_R", 255, 0, 27);
        AddFloatUI(parent, "Spike_con_G", 255, 0, 206);
        AddFloatUI(parent, "Spike_con_B", 255, 0, 135);
        AddFloatUI(parent, "Spike_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "Spike_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "Spike_bgd_B", 255, 0, 0);
        m_lastUpdatedHeight += 10;

        AddButtonUI(parent, "Spike_CC_Blue");
        AddButtonUI(parent, "Spike_CC_Orange");
        AddButtonUI(parent, "Spike_CC_Yellow");
        AddButtonUI(parent, "Spike_CC_Pink");
        AddButtonUI(parent, "Spike_CC_Green");

    }

    private void CreateUIsImportKinectv2(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Kinect_x_min", 0, -5, -5);
        AddFloatUI(parent, "Kinect_x_max", 5, 0, 5);
        AddFloatUI(parent, "Kinect_y_min", 0, -2, -1);
        AddFloatUI(parent, "Kinect_y_max", 5, 0, 4);
        AddFloatUI(parent, "Kinect_z_min", 8, 0, 0);
        AddFloatUI(parent, "Kinect_z_max", 8, 0, 8);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Kinect_pos_x", 10, -10, 0);
        AddFloatUI(parent, "Kinect_pos_y", 10, -10, 0);
        AddFloatUI(parent, "Kinect_pos_z", 10, -10, 0);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Kinect_rot_x", 180, -180, 0);
        AddFloatUI(parent, "Kinect_rot_y", 180, -180, 0);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Kinect_screen_r", 10, 0, 2.5f);
        AddFloatUI(parent, "Kinect_light_r", 10, 0, 8f);
        m_lastUpdatedHeight += 10;
        AddEnumUI(parent, "Kinect_LightMode", KinectImporter.LightSourceMode.Normal);
        m_lastUpdatedHeight += 10;
        AddBooleanUI(parent, "Archive", false);
        m_lastUpdatedHeight += 10;
        AddBooleanUI(parent, "Kinect_Depth", false);

        AddFloatUI(parent, "kinect_height", 5, 0, 0.85f);
        AddFloatUI(parent, "kinect_angle", 180, -180, -90);
        AddFloatUI(parent, "kinect_radius", 10, 0, 5.4f);

    }

    private void CreateUIsImportKinectAditional(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Kinect_ViewRange", 4, 0.1f, 1);
        AddFloatUI(parent, "Kinect_CircleCut", 64, 0, 25);
        AddBooleanUI(parent, "kinect_use_bone", true);
    }

    private void CreateUIsImportNetwork(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddTextUI(parent, "Network_CIPCServerIP");
        AddTextUI(parent, "Network_CIPCServerPort");
        AddButtonUI(parent, "Network_CIPCServerConnect");
        AddBooleanUI(parent, "Network_Send", false);
        AddBooleanUI(parent, "Network_Receive", false);
    }

    /// <summary>
    /// normal shadow
    /// </summary>
    /// <param name="parent"></param>
    private void CreateUIsImageporcessingNormal(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddBooleanUI(parent, "Normal_Invert", false);
    }
    private void CreateUIsImageporcessingTimeDelay(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "TimeDelay_DelayTime", 1000, 0, 0);
    }

    protected void CreateUIsCallibrationImport(GameObject parent, int num)
    {
        m_lastUpdatedHeight = 0;

        AddFloatUI(parent, "Clb_I_TL_X" + num, 2, -1, 0);
        AddFloatUI(parent, "Clb_I_TL_Y" + num, 2, -1, 0);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Clb_I_BL_X" + num, 2, -1, 0);
        AddFloatUI(parent, "Clb_I_BL_Y" + num, 2, -1, 1);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Clb_I_BR_X" + num, 2, -1, 1);
        AddFloatUI(parent, "Clb_I_BR_Y" + num, 2, -1, 1);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Clb_I_TR_X" + num, 2, -1, 1);
        AddFloatUI(parent, "Clb_I_TR_Y" + num, 2, -1, 0);
        m_lastUpdatedHeight += 20;
        AddEnumUI(parent, "clb_i_import_mode" + num, ShadowMeshRenderer.ImportMode.Quadrangle);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "clb_i_r_top" + num, 5, 0, 1);
        AddBooleanUI(parent, "clb_i_use_up_top" + num, true);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "clb_i_r_btm" + num, 5, 0, 1);
        AddBooleanUI(parent, "clb_i_use_up_btm" + num, true);

        AddFloatUI(parent, "clb_i_condence" + num, 1, 0, 1);


        m_lastUpdatedHeight += 10;

        

        m_lastUpdatedHeight += 10;
        AddButtonUI(parent, "Clb_I_Save" + num);
        AddButtonUI(parent, "Clb_I_Load" + num);

        AddButtonUI(parent, "Save_ClbUV" + num);
        AddButtonUI(parent, "Load_ClbUV" + num);

    }

    protected void CreateUIsCallibrationExport(GameObject parent, int num)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Clb_E_TL_X" + num, 2000, -1000, 0);
        AddFloatUI(parent, "Clb_E_TL_Y" + num, 2000, -1000, 0);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Clb_E_BL_X" + num, 2000, -1000, 0);
        AddFloatUI(parent, "Clb_E_BL_Y" + num, 2000, -1000, Screen.height);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Clb_E_BR_X" + num, 2000, -1000, Screen.width);
        AddFloatUI(parent, "Clb_E_BR_Y" + num, 2000, -1000, Screen.height);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Clb_E_TR_X" + num, 2000, -1000, Screen.width);
        AddFloatUI(parent, "Clb_E_TR_Y" + num, 2000, -1000, 0);
        m_lastUpdatedHeight += 20;

        AddBooleanUI(parent, "Clb_E_Vsbl" + num, true);
        m_lastUpdatedHeight += 20;
        AddEnumUI(parent, "clb_e_mode" + num, ShadowMeshRenderer.PlaneMode.Plane);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "clb_e_pos_x" + num, 10, -10, 0);
        AddFloatUI(parent, "clb_e_pos_y" + num, 10, -10, 0);
        AddFloatUI(parent, "clb_e_pos_z" + num, 10, -10, 0);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "clb_e_radius" + num, 5, 0, 2.5f);
        AddFloatUI(parent, "clb_e_height" + num, 5, 0, 2.5f);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "clb_e_in_rad" + num, 5, 0, 0.5f);
        m_lastUpdatedHeight += 10;

        AddFloatUI(parent, "clb_e_startAngle" + num, 2 * Mathf.PI, 0, Mathf.PI / 4f );
        AddFloatUI(parent, "clb_e_finishAngle" + num, 2 * Mathf.PI, 0, Mathf.PI / 4f * 3f);


        m_lastUpdatedHeight += 20;


        AddButtonUI(parent, "Clb_E_Save" + num);
        AddButtonUI(parent, "Clb_E_Load" + num);

    }

    protected void CreateUIsCameraCllibration(GameObject parent, int num)
    {
        m_lastUpdatedHeight = 0;
        AddEnumUI(parent, "clb_camera_mode" + num, ProjectionCameraMode.Orthographic);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "clb_camera_pos_x" + num, 10, -10, 0);
        AddFloatUI(parent, "clb_camera_pos_y" + num, 10, -10, 0);
        AddFloatUI(parent, "clb_camera_pos_z" + num, 10, -10, 0);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "clb_camera_pos_rx" + num, 90, -90, 0);
        AddFloatUI(parent, "clb_camera_pos_ry" + num, 90, -90, 0);
        AddFloatUI(parent, "clb_camera_pos_rz" + num, 90, -90, 0);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "clb_camera_fview" + num, 179, 1, 45);
    }

    private void CreateUIsGenericModes(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddEnumUI(parent, "core_shadow_media_mode", Miwalab.ShadowGroup.Core.ShadowMediaMode.ShadowMedia2D);
        AddBooleanUI(parent, "core_switch_objects", false);
    }



    #endregion

    #region parts
    protected void AddFloatUI(GameObject parent, string ParameterName, float max, float min, float @default)
    {
        var slider = Instantiate<ParameterSlider>(m_slider);
        slider.Title = ParameterName;
        slider.Max = max;
        slider.Min = min;
        slider.DefaultValue = @default;
        slider.gameObject.transform.SetParent(parent.transform.FindChild("Viewport").FindChild("Content"), false);
        var recttransform = slider.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0, -m_lastUpdatedHeight);

        AddUI(ParameterName, slider);

        m_lastUpdatedHeight += slider.getSize().height;

    }
    protected void AddBooleanUI(GameObject parent, string ParameterName, bool @default)
    {
        var checkBox = Instantiate<ParameterCheckbox>(m_checkbox);
        checkBox.Title = ParameterName;
        checkBox.DefaultValue = @default;
        checkBox.gameObject.transform.SetParent(parent.transform.FindChild("Viewport").FindChild("Content"), false);
        var recttransform = checkBox.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0, -m_lastUpdatedHeight);

        AddUI(ParameterName, checkBox);

        m_lastUpdatedHeight += checkBox.getSize().height;

    }
    protected void AddButtonUI(GameObject parent, string ParameterName)
    {
        var button = Instantiate<ParameterButton>(m_button);
        button.Title = ParameterName;
        button.gameObject.transform.SetParent(parent.transform.FindChild("Viewport").FindChild("Content"), false);
        var recttransform = button.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0, -m_lastUpdatedHeight);

        AddUI(ParameterName, button);

        m_lastUpdatedHeight += button.getSize().height;

    }

    protected void AddTextUI(GameObject parent, string ParameterName)
    {
        var Text = Instantiate<ParameterText>(m_text);
        Text.Title = ParameterName;
        Text.gameObject.transform.SetParent(parent.transform.FindChild("Viewport").FindChild("Content"), false);
        var recttransform = Text.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0, -m_lastUpdatedHeight);

        AddUI(ParameterName, Text);

        m_lastUpdatedHeight += Text.getSize().height;

    }

    protected void AddEnumUI<T>(GameObject parent, string ParameterName, T defaultValue)
        where T : struct
    {
        var _object = Instantiate(m_Dropdown);
        var titleText = _object.GetComponentInChildren<Text>(true);
        var dropdown = _object.GetComponentInChildren<Dropdown>(true);

        var item =  _object.AddComponent<ParameterDropdown>();
        item.m_titleText = titleText;
        item.m_Dropdown = dropdown; 

        dropdown.onValueChanged.AddListener( new UnityEngine.Events.UnityAction<int>( item.OnValueChanged));

        item.Title = ParameterName;

        item.initialize(defaultValue);
        _object.transform.SetParent(parent.transform.FindChild("Viewport").FindChild("Content"), false);
        var recttransform = _object.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0, -m_lastUpdatedHeight);

        AddUI(ParameterName, item);

        m_lastUpdatedHeight += item.getSize().height;

    }
    #endregion

    #region Panel OnOff
    private GameObject m_currentImageProcesserSettingPanel;
    public void ImageProcesserSettingPanelSet(bool value)
    {
        m_currentImageProcesserSettingPanel.SetActive(value);

    }
    private GameObject m_currentImportSettingPanel;
    public void ImportSettingPanelSet(bool value)
    {
        m_currentImportSettingPanel.SetActive(value);
    }
    protected GameObject m_currentCallibrationSettingPanel;
    public void CallibrationSettingPanelSet(bool value)
    {
        m_currentCallibrationSettingPanel.SetActive(value);
    }
    private GameObject m_currentAfterEffectSettingPanel;
    public void AfetrEffectSettingPanelSet(bool value)
    {
        m_currentAfterEffectSettingPanel.SetActive(value);
    }
    private GameObject m_currentArchiveSettingPanel;
    public void ArchiveSettingPanelSet(bool value)
    {
        m_currentArchiveSettingPanel.SetActive(value);
    }
    private GameObject m_currentGenericSettingPanel;
    public void GenericSettingPanelSet(bool value)
    {
        m_currentGenericSettingPanel.SetActive(value);

    }
    private GameObject m_currentBackgroundTypeSettingPanel;
     public void BackgroundTypeSettingPanelSet(bool value)
    {
        m_currentBackgroundTypeSettingPanel.SetActive(value);

    }


    #endregion

    #region background
 
    public void ChangeBackgroundTypeSettingOptionTo(int number)
    {
        Miwalab.ShadowGroup.Background.BackgroundType type = (Miwalab.ShadowGroup.Background.BackgroundType)number;

        this.m_currentBackgroundTypeSettingPanel = this.m_PanelDictionary[type.ToString()];
        this.SwitchOffOtherPanelsExceptOf(this.m_currentBackgroundTypeSettingPanel);
        if (number == 0) {
            for (int i = 0; i < this.List_backGround.Count ; ++i)
            {
                    this.List_backGround[i].SetActive(false);
            }
        }
        else if (number == 1)
        {

        }
        else
        {
            for (int i = 0; i < this.List_backGround.Count; ++i)
            {
                if (i  == number - 2)
                {
                    this.List_backGround[i].SetActive(true);
                }
                else
                {
                    this.List_backGround[i].SetActive(false);
                }
            }
        }
    }

    private void CreateUIsBackgroundNone(GameObject parent)
    {
        m_lastUpdatedHeight = 0;


    }

    private void CreateUIsBackgroundCircleCut(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "CircleCut_radius", 5, 0.1f, 3f);
        AddBooleanUI(parent, "CircleCut_active", true);
    }

    private void CreateUIsBackgroundButterfly(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Butterfly_R", 1f, 0, 1f);
        AddFloatUI(parent, "Butterfly_G", 1f, 0, 1f);
        AddFloatUI(parent, "Butterfly_B", 1f, 0, 1f);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Particle_Size", 1f, 0.1f, 0.5f);
        AddFloatUI(parent, "Particle_Num", 200, 0, 200);
        m_lastUpdatedHeight += 10;
        AddButtonUI(parent, "Particle_FadeWhite");
        AddButtonUI(parent, "Particle_FadeBlack");
    }

    private void CreateUIsBackgroundFish(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
      

    }

    private void CreateUIsBackgroundTiger(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Tiger_Size", 2, 0.1f, 1);
        AddFloatUI(parent, "Tiger_theta0", 3.14f, -3.14f, 0);
        AddFloatUI(parent, "Tiger_radius", 4, 1, 2.5f);
        AddFloatUI(parent, "Tiger_spdRate", 2, 0, 1);

    }
    private void CreateUIsBackgroundUnitychan(GameObject parent)
    {
        m_lastUpdatedHeight = 0;


    }

    #endregion

}
