using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MCAnimationLoading : MonoBehaviour
{
    public bool IsPlaying = true;
    public Image LoadingImage;
    public float Speed = 1f;

    public float Process;


    private void OnEnable()
    {
        if (IsPlaying)
        {
            Play();
        }
    }

    public void Play()
    {
        Forward();
    }

    public void Stop()
    {
        if (LoadingImage != null)
        {
            LoadingImage.transform.localScale = new Vector3(1, 1, 1);
        }
    }


    public void Forward()
    {
        if (LoadingImage != null)
        {
            LoadingImage.transform.localScale = new Vector3(1, 1, 1);
        }
        LeanTween.value(1f, 0, Speed).setOnUpdate(SetFill).setOnComplete(Backward);
    }

    public void Backward()
    {
        if (LoadingImage != null)
        {
            LoadingImage.transform.localScale = new Vector3(-1, 1, 1);
        }
        LeanTween.value(0, 1f, Speed).setOnUpdate(SetFill).setOnComplete(Forward);
    }


    public void SetFill(float _value)
    {
        if (LoadingImage != null)
        {
            LoadingImage.fillAmount = _value;
        }
    }

    public void OnDisable()
    {
        LeanTween.cancel(this.gameObject);
    }

}
