using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;


public class UIController_Uploder : MonoBehaviour
{
    public UploaderConfiger Configer;


    public TextMeshProUGUI Text_UploadResult;

    // Start is called before the first frame update
    void Start()
    {
        if(Configer == null)
        {
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick_UploadFile()
    {
        //JSCaller.OnUploadFile(Configer.ToJson(), OnUploadFileComplete_Callback);
        // Start Loading?
    }


    public void OnUploadFileComplete_Callback(bool _result, string _message)
    {
        // Complete Loading
    }

    public void OnClick_PreviewUploadedFile()
    {

    }

    public void OnClick_RemoveUploadedFile()
    {
        //Text_UploadResult.gameObject.GetComponent<ML_Label>().SetML(ML_Controller.Instance.Language, Text_UploadResult.gameObject.GetComponent<ML_Label>().LID);

    }
}
