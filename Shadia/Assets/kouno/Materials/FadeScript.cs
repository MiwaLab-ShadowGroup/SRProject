using UnityEngine;
using System.Collections;

public class FadeScript : MonoBehaviour {
    private string fade;
    public float fadeTime = 3.0f;

    // Use this for initialization
    void Start()
    {
        // ２秒後にFadeIn()を、５秒後にFadeOut()を呼び出す
        //Invoke("FadeIn", 2f);
        //Invoke("FadeOut", 5f);

        //はじめは白背景から始まると仮定
        fade = "white";
        
    }
     
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            switch(fade){
                case "white":
                    Invoke("FadeIn",0);
                    fade = "black";
                    break;

                case "black":
                    Invoke("FadeOut", 0);
                    fade = "white";
                    break;

            }
        }
    }

    void FadeIn()
    {
        // SetValue()を毎フレーム呼び出して、１秒間に０から１までの値の中間値を渡す
        iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "time", fadeTime, "onupdate", "SetValue"));
    }
    void FadeOut()
    {
        // SetValue()を毎フレーム呼び出して、１秒間に１から０までの値の中間値を渡す
        iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", fadeTime, "onupdate", "SetValue"));
    }
    void SetValue(float alpha)
    {
        // iTweenで呼ばれたら、受け取った値をImageのアルファ値にセット
        gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, alpha);
    }
}
