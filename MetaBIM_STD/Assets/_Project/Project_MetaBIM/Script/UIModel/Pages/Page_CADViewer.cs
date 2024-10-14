using EzySlice;
using IfcToolkit;
using netDxf;
using netDxf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using TMPro;
using UnityEngine;



namespace MetaBIM
{
    public class Page_CADViewer : MonoBehaviour
    {
        public static Page_CADViewer Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Panel.OnOpenAction = OnOpenAction;
            Panel.OnCloseAction = OnCloseAction;
        }

        public PanelChange Panel;
        public bool IsPageOpend;


        [Header("--------------")]
        [Header("Cad File Loader")]

        public string CadFilePath;
        public DxfDocument CurrentCadDocument;


        [Header("--------------")]
        [Header("3D Viewer Space")]

        public Transform CadEntityParent;

        // =================================================================================

        public void OnOpenAction()
        {
            LoadCadAsset();
        }


        public void OnCloseAction()
        {

        }


        // Start is called before the first frame update
        void Start()
        {
            // Hardcore
            CadFilePath = Application.streamingAssetsPath + "/dwg/default.dxf";
        }

        
        // Update is called once per frame
        void Update()
        {


        }





        #region Loading Asset CAD 
        public void LoadCadAsset()
        {
            Debug.Log("loading cad file: " + CadFilePath);
            string result = CadManager.CadLoader(CadFilePath, out CurrentCadDocument);
            
            if(result == "version")
            {
                MCPopup.Instance.SetInformation(StringBuffer.Messaege_Popup_CadFileNotLoad.S);
                goto END;
            }
            else { 
                
            }

            /* fail loaded */
            string documentName = CurrentCadDocument.Name;
            DrawingEntities entity = CurrentCadDocument.Entities;

            /* load layers */
            Layers layers = CurrentCadDocument.Layers;
            Debug.Log("Layer loaded: " + layers.Count);


            // Get Layers
            foreach (var layer in layers)
            {
                Debug.Log("Layer : " + layer.Name + " | " + layers.GetReferences(layer.Name).Count);
                
   
            }



            Debug.Log("");
            END:
            return;
        }




        #endregion



        #region Asset Viewer, Render Entity 


        // drawing CAD entity Line
        public void DrawEntity_Line()
        {
            
        }

        #endregion



        #region Entity Interaction

        #endregion


        #region Data Analysis

        #endregion


        /* Utility */
        #region Utility

        [Header("--------------")]
        [Header("Utility")]

        public TextMeshProUGUI Text_ActionInfo;
        public CanvasGroup CanvasGroup_ActionInfoPanel;

        
        public void ClearList(List<GameObject> _itemList)
        {

        }


        public void ClearObjectList(List<GameObject> _list, bool isBlockDestroy = false)
        {
            if (_list.Count > 0)
            {
                foreach (GameObject item in _list)
                {
                    if (!isBlockDestroy)
                    {
                        Destroy(item);
                    }
                    else
                    {
                        // TODO

                    }
                }
            }

            _list.Clear();
        }

        
        public void debug(string _message, float _time = 1f)
        {
            CanvasGroup_ActionInfoPanel.alpha = 1f;
            Text_ActionInfo.text = _message;
            LeanTween.cancelAll();
            CancelInvoke();
            Invoke("OnMessageFade", _time);
        }
        public void OnMessageFade()
        {
            LeanTween.alphaCanvas(CanvasGroup_ActionInfoPanel, 0, 2f).setOnComplete(() =>
            {
                Text_ActionInfo.text = "";
            });
        }

        #endregion

    }

}
