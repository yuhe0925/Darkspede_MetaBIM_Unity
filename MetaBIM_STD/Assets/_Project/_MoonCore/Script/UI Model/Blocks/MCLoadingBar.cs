using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MCLoadingBar : MonoBehaviour
{


    public static MCLoadingBar Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    // Start is called before the first frame update
    public Transform LoadingBar;
    public CanvasGroup LoadingBarCanvasGroup;

    [Range(0, 1f)]
    public float Progress = 0;


    public void OnSetLoadingProgress(float _progress, bool _isSmoothTransication = false)
    {
        LoadingBar.gameObject.SetActive(true);

        if(_progress <  1 && _progress > 0)
        {
            LoadingBarCanvasGroup.alpha = 1;

            if(_isSmoothTransication)
            {
                LeanTween.cancelAll(this.gameObject);
                LeanTween.scaleX(LoadingBar.gameObject, _progress, 0.5f);

            }
            else{
                LoadingBar.localScale = new Vector3(_progress, 1, 1);
            }

        }
        else
        {
            OnLoadingComplete();
        }
    }





    public void OnLoadingComplete()
    {
        // fade out
        LeanTween.alphaCanvas(LoadingBarCanvasGroup, 0, 0.5f).setOnComplete(() =>
        {
            LoadingBar.localScale = new Vector3(0, 1, 1);
            LoadingBar.gameObject.SetActive(false);
        });
    }
}
