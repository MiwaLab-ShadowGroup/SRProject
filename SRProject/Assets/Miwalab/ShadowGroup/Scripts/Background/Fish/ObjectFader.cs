using UnityEngine;
using System.Collections;
using System;

namespace Miwalab.ShadowGroup.Background
{
    public enum FadeState
    {
        NotFade,
        FadeStart,
        Fading,
        FadeFinished,
    }

    public enum FadeMode
    {
        NotFadeMode,
        BlackMode
    }

    [RequireComponent(typeof(Renderer))]
    public class ObjectFader : MonoBehaviour
    {


        private FadeState m_fadeState = FadeState.NotFade;
        private FadeMode m_fadeMode = FadeMode.NotFadeMode;

        private Material m_material;
        private Color m_color;

        private bool m_fadeStart = false;
        public bool UseKeybord = false;
        public bool IsBackRenderFader1 = false;
        public bool IsBackRenderFader2 = false;
        public bool IsFishFader = false;
        // Use this for initialization
        void Start()
        {
            m_material = GetComponent<Renderer>().material;
            m_color = m_material.color;
            if (IsBackRenderFader1)
            {
                (Miwalab.ShadowGroup.GUI.BackgroundMediaUIHost.GetUI("CP_1_Fade") as ParameterButton).Clicked += ParameterSlider_CP_1_Fade_Clicked;
            }
            if (IsBackRenderFader2)
            {
                (Miwalab.ShadowGroup.GUI.BackgroundMediaUIHost.GetUI("CP_2_Fade") as ParameterButton).Clicked += ParameterSlider_CP_2_Fade_Clicked;
            }
            if (IsFishFader)
            {
                (Miwalab.ShadowGroup.GUI.BackgroundMediaUIHost.GetUI("Fish_FadeOut") as ParameterButton).Clicked += ParameterSlider_Fish_FadeOut_Clicked;
            }
        }

        private void ParameterSlider_Fish_FadeOut_Clicked(object sender, EventArgs e)
        {
            this.m_fadeStart = true;
        }

        private void ParameterSlider_CP_2_Fade_Clicked(object sender, EventArgs e)
        {
            this.m_fadeStart = true;
        }

        private void ParameterSlider_CP_1_Fade_Clicked(object sender, EventArgs e)
        {
            this.m_fadeStart = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (UseKeybord)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    this.m_fadeStart = true;
                }
            }
            switch (m_fadeMode)
            {
                case FadeMode.NotFadeMode:
                    UpdateNotFadeMode();
                    break;
                case FadeMode.BlackMode:
                    UpdateBlackMode();
                    break;
                default:
                    break;
            }
            this.m_material.color = m_color;
        }

        private void UpdateBlackMode()
        {
            switch (m_fadeState)
            {
                case FadeState.NotFade:
                    UpdateBlackModeNotFade();
                    break;
                case FadeState.FadeStart:
                    UpdateBlackModeFadeStart();
                    break;
                case FadeState.Fading:
                    UpdateBlackModeFading();
                    break;
                case FadeState.FadeFinished:
                    UpdateBlackModeFadeFinished();
                    break;
                default:
                    break;
            }
        }

        private void UpdateBlackModeFadeFinished()
        {
            //finalize
            this.m_color.a = 0;

            this.m_fadeState = FadeState.NotFade;
            this.m_fadeMode = FadeMode.NotFadeMode;
        }

        private void UpdateBlackModeFading()
        {

            m_color.a -= 0.01f;

            //終了条件
            if (m_color.a < 0.02f)
            {
                this.m_fadeState = FadeState.FadeFinished;
            }
        }

        private void UpdateBlackModeFadeStart()
        {
            //初期化処理


            //初期化処理終了

            this.m_fadeState = FadeState.Fading;
        }

        private void UpdateBlackModeNotFade()
        {
            if (this.m_fadeStart)
            {
                this.m_fadeState = FadeState.FadeStart;
                this.m_fadeStart = false;
            }
        }

        private void UpdateNotFadeMode()
        {
            switch (m_fadeState)
            {
                case FadeState.NotFade:
                    UpdateNotFadeModeNotFade();
                    break;
                case FadeState.FadeStart:
                    UpdateNotFadeModeFadeStart();
                    break;
                case FadeState.Fading:
                    UpdateNotFadeModeFading();
                    break;
                case FadeState.FadeFinished:
                    UpdateNotFadeModeFadeFinished();
                    break;
                default:
                    break;
            }
        }

        private void UpdateNotFadeModeFadeFinished()
        {
            //finalize
            m_color.a = 1;
            this.m_fadeState = FadeState.NotFade;
            this.m_fadeMode = FadeMode.BlackMode;
        }

        private void UpdateNotFadeModeFading()
        {

            m_color.a += 0.01f;
            //終了条件
            if (m_color.a > 1f)
            {
                this.m_fadeState = FadeState.FadeFinished;
            }
        }

        private void UpdateNotFadeModeFadeStart()
        {
            //初期化処理

            //初期化処理終了

            this.m_fadeState = FadeState.Fading;
        }

        private void UpdateNotFadeModeNotFade()
        {
            if (m_fadeStart)
            {
                this.m_fadeState = FadeState.FadeStart;
                m_fadeStart = false;
            }
        }
    }
}