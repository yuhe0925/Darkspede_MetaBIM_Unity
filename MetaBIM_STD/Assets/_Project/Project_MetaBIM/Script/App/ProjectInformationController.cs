using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectInformationController : MonoBehaviour
{
    public static ProjectInformationController Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        MainPanel.OnOpenAction = OnOpenAction;
        MainPanel.OnCloseAction = OnCloseAction;
    }

    [Header("Controller")]
    public OnlineMaps Map;
    public Camera MapCamera;


    [Header("UI Element")]

    public PanelChange MainPanel;


    public void OnOpenAction()
    {
        Map.gameObject.SetActive(true);
        MapCamera.gameObject.SetActive(true);


        MapController.Instance.LoadSupplyTransportInfo();
    }


    public void OnCloseAction()
    {
        Map.gameObject.SetActive(false);
        MapCamera.gameObject.SetActive(false);
    }


    public void OnClick_Open()
    {
        MainPanel.OnPanelOpen();    
    }

    public void OnClick_Close()
    {
        MainPanel.OnPanelClose();
    }
}
