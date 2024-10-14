using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class LoadingHandler : MonoBehaviour
{
    public static LoadingHandler Instance;

    public int LoadingCount;


    public GameObject LoadingPanel;
    public GameObject LoadingFloater;

    public float LoadingPanelTimeOut = 30f;


    public GameObject FullpageLoadingFloater;
    public TextMeshProUGUI Text_ProgressText;

    private float LoadingPanelTimeOutCounter;
    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        LoadingPanelTimeOutCounter = -1;
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadingPanel.SetActive(false);
        FullpageLoadingFloater.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (LoadingPanelTimeOutCounter > 0)
        {
            LoadingPanel.SetActive(true);
            LoadingPanelTimeOutCounter = LoadingPanelTimeOutCounter - Time.deltaTime;
        }
        else
        {
            LoadingPanel.SetActive(false);
        }
    }

    public void StartLoading()
    {
        LoadingPanelTimeOutCounter = LoadingPanelTimeOut;
        LoadingCount++;
    }




    public void OnStartLoading()
    {
        LoadingPanelTimeOutCounter = LoadingPanelTimeOut;
    }
    public void CompleteLoading()
    {
        LoadingCount--;

        if (LoadingCount < 1)
        {
            LoadingPanelTimeOutCounter = 0;
        }
    }


    public void Cancel()
    {
        LoadingPanelTimeOutCounter = 0;
    }





    public void OnFullPageLoadingStart(string _mesasge=  "")
    {
        //Debug.Log("OnFullPageLoadingStart");
        FullpageLoadingFloater.SetActive(true);
        Text_ProgressText.text = _mesasge;
    }

    public void OnFullPageLoadingSetProgress(string _progress)
    {
        Text_ProgressText.text = _progress;
    }

    public void OnFullPageLoadingEnd()
    {
        //Debug.Log("OnFullPageLoadingEnd");
        FullpageLoadingFloater.SetActive(false);
        Text_ProgressText.text = "";
    }
}
