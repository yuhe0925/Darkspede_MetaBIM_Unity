using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MetaBIM;
using UnityEngine.UI;
using SR = StringBuffer;

public class Page_Signin : MonoBehaviour
{
    public static Page_Signin Instance;
    public PanelChange Panel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Panel.OnOpenAction = OnOpenAction;
        Panel.OnCloseAction = OnCloseAction;
    }



    public void OnOpenAction()
    {
        SignInStage(0);
    }

    public void OnCloseAction()
    {

    }


    [Header("UI Element")]

    public TMP_InputField Input_Mobile;
    public TMP_InputField Input_PIN;
    public TMP_InputField Input_Title;

    public TextMeshProUGUI Text_CodeCountDown;

    public Button Button_CountDown;
    public Button Button_SignIN;

    [Header("Stage")]
    public GameObject[] Stage_EnterMobile;
    public GameObject[] Stage_EnterPIN;


    private float CountDown = 0;
    private string sessionMobile = "";
    private string sesstionToken = "";
    


    void Update()
    {
        if(CountDown > 0)
        {
            CountDown = CountDown - Time.deltaTime;
            Text_CodeCountDown.text = 
                SR.Signin_Message_GetCodePrefix.S + 
                (int)CountDown + 
                SR.Signin_Message_GetCodePrefix.S;
            
            Button_CountDown.interactable = false;
        }
        else
        {
            CountDown = 0;
            Text_CodeCountDown.text = SR.Signin_Message_GetCode.S;
            Button_CountDown.interactable = true;
        }
    }


    public void SignInStage(int _stage)
    {
        if(_stage == 0)
        {
            Input_PIN.text = "";
            Input_Mobile.text = "";
            sessionMobile = "";

            foreach (GameObject ob in Stage_EnterMobile)
            {
                ob.SetActive(true);
            }

            foreach (GameObject ob in Stage_EnterPIN)
            {
                ob.SetActive(false);
            }
        }
        else if (_stage == 1)
        {
            foreach (GameObject ob in Stage_EnterMobile)
            {
                ob.SetActive(false);
            }

            foreach (GameObject ob in Stage_EnterPIN)
            {
                ob.SetActive(true);
            }
        }
    }
    
    public void OnClick_SignIn_GetCode()
    {
        sessionMobile = Utility.ValidateMobile(Input_Mobile.text);

        if (sessionMobile == "" || sessionMobile == null)
        {
            MCPopup.Instance.SetWarning(SR.Signin_Message_InvalidMobile.S, SR.Messaege_Popup_Warning.S);
            return;
        }

        Button_CountDown.interactable = false;
        CountDown = Config.GetCodeCountDown;
        OnGetCode(sessionMobile);
    }


    public void OnGetCode(string _mobile)
    {

        StartCoroutine(DataProxy.Instance.OnRequestAuthCodeByMobile(_mobile, OnGetCode_Callback));
    }

    public void OnGetCode_Callback(bool _result, string _message)
    {
        if (_result)
        {

            DataProxyResponse<Profile> package = DataProxyResponse<Profile>.FromJson(_message);

            if (package.result)
            {
                AppController.Instance.CurrentProfile = package.package[0];
                sesstionToken = package.package[0].accesstoken;
                //Debug.Log("Auth Code: " + AppController.Instance.CurrentProfile.securityCode);
                MCPopup.Instance.SetInformation(SR.Signin_Message_AuthCode.S, SR.Messaege_Popup_Sent.S);
                SignInStage(1);
            }
            else
            {
                AppController.Instance.CurrentProfile = null; 
                if (_message.ToLower().Contains("target"))
                {
                    MCPopup.Instance.SetWarning(SR.Signin_Message_ProfileNotFound.S, SR.Messaege_Popup_Warning.S);
                }
                else
                {
                    MCPopup.Instance.SetWarning("Network error!", "Send Code Fail");
                }
            }
    
        }
        else
        {
            AppController.Instance.CurrentProfile = null;
            MCPopup.Instance.SetWarning("Network error!", "Send Code Fail");
            CountDown = 0;
            Button_CountDown.interactable = true;
        }
    }

    public void OnClick_SignIn()
    {
        OnSignIn();
    }

    public void OnSignIn()
    {
        Button_CountDown.interactable = false;
        string pin = Input_PIN.text;

        if(pin.Length != 6)
        {
            MCPopup.Instance.SetWarning("Enter 6 digits PIN that was sent to your mobile number", "PIN");
        }

        StartCoroutine(DataProxy.Instance.OnRequestLoginByMobile(sessionMobile, pin, sesstionToken,  OnSignIn_Callback));
    }

    public void OnSignIn_Callback(bool _result, string _message)
    {
        Button_CountDown.interactable = true;

        if (_result)
        {
            DataProxyResponse<Profile> response = DataProxyResponse<Profile>.FromJson(_message);
            
            if (response.result && response.package.Count == 1)
            {
                Profile profile = response.package[0];
                AppController.Instance.OnSuccessSignin(profile);
                SignInStage(0);
            }
            else
            {
                Debug.Log("Response content eroor(fail result or package size incorrect): " + _message);
                MCPopup.Instance.SetWarning("Please check if you entered correct PIN", "Sign in failed");
            }

  
        }
        else
        {
            MCPopup.Instance.SetWarning("Unable to verify account information.", "Network Error");
        }
    }

    public void OnClick_CancelPinInput()
    {
        Input_PIN.text = "";
        SignInStage(0);
    }


    
}
