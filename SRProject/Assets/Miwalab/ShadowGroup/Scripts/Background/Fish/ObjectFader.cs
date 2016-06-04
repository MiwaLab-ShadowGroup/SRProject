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
        // Use this for initialization
        void Start()
        {
            m_material = GetComponent<Renderer>().material;
            m_color = m_material.color;
        }

        // Update is called once per frame
        void Update()
        {
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
            if (m_color.a < 0.02f )
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
            if (UnityEngine.Input.GetKeyDown(KeyCode.F))
            {
                this.m_fadeState = FadeState.FadeStart;
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
            if (UnityEngine.Input.GetKeyDown(KeyCode.F))
            {
                this.m_fadeState = FadeState.FadeStart;
            }
        }
    }
}