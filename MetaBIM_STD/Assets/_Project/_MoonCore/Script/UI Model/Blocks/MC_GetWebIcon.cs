using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;


public class MC_GetWebIcon : MonoBehaviour
{
    //public RawImage Target;

    [Header("Image Effects")]
    public Image Image;

    [Header("Image Effects")]
    public TextMeshProUGUI Text_Status;
    public bool IsCache = true;
    public float FloatFadeTime = 0f;

    [Header("Image Config")]
    public string ImageUrl;
    public bool isLoading;
    public Vector2 ImageSize = new Vector2(256, 256);
    public int Padding = 0;
    public bool IsSetToRatio;
    public bool IsResize = false;
    public bool IsLoadOnStart = false;


    [Header("Redirect Events")]
    public float LongPressLoad = 1.5f;
    public UnityEvent RedirectEvent;

    private float initPress;

    // Start is called before the first frame update
    void Start()
    {
        initPress = -1;
        if (IsLoadOnStart)
        {
            SetBlock();
        }
 
    }


    public void SetBlock(string _url = "")
    {
        _url = _url.ToLower();
        //Debug.Log("MC_GetWebIcon.SetBlock: " + _url);
        
        if (_url == "default" || _url == "")
        {
            SetImage(Config.Domain + ImageUrl);
        }
        else if (_url == "none")
        {
            if (Image != null)
            {
                Image.gameObject.SetActive(false);
            }
        }
        else
        {
            SetImage(_url);
        }
    }


    public void OnClick_ReloadImage()
    {
        if (!isLoading)
        {
            Image.gameObject.SetActive(false);
            Davinci.ClearCache(ImageUrl);
            SetBlock(ImageUrl);
        }
    }



    private void SetImage(string _loadingURL)
    {
        if (Image != null)
        {
            //Debug.Log("Loading Image from : " + _loadingURL);
            
            Davinci.get()
            .load(_loadingURL)
            .setCached(false)
            //.setLoadingPlaceholder(ResourceHolder.Instance.Image_Loading)
            //.setErrorPlaceholder(ResourceHolder.Instance.Image_Error)
            .setCached(true).into(Image)
            .setFadeTime(FloatFadeTime)
            .withStartAction(() =>
            {
                isLoading = true;
                if (Text_Status != null)
                {
                    Text_Status.text = "";
                }
                //Debug.Log("Download has been started.");
            })
            .withDownloadProgressChangedAction((progress) =>
            {
                if (Text_Status != null)
                {
                    Text_Status.text = progress + "%";
                    Text_Status.fontSize = 14;
                }
                
                //Debug.Log("Download progress: " + progress);
            })
            .withDownloadedAction(() =>
            {

                //Debug.Log("Download has been completed.");
            })
            .withLoadedAction(() =>
            {
                if (Text_Status != null)
                {
                    Text_Status.text = "";
                }

                if (Image != null)
                {
                    if (Image.gameObject != null)
                    {
                        Image.gameObject.SetActive(true);
                    }
                }
                //Debug.Log("Image has been loaded.");

            })
            .withErrorAction((error) =>
            {
                if (Text_Status != null)
                {
                    Text_Status.text = ResourceHolder.Instance.Text_ImageLoadingFailed;
                    Text_Status.fontSize = 28;
                }
                //Debug.Log("Got error : " + error);
            })
            .withEndAction(() =>
            {
                isLoading = false;
                //Debug.Log("Operation has been finished.");
            })
            .start();
        }
    }




    public void OnClick_PressDown()
    {
        initPress = Time.time;
    }

    public void OnClick_PressUp()
    {
        float diff = Time.time - initPress;
        Debug.Log("OnClick_PressUp:" + diff);

        if (initPress > 0 && diff < LongPressLoad)
        {
            if (RedirectEvent != null)
            {
                RedirectEvent.Invoke();
            }
        }
        else
        {
            OnClick_ReloadImage();

        }

        initPress = 0;
    }


    /*

    IEnumerator DownloadImage(string MediaUrl)
    {
        Debug.Log("DownloadImage: " + MediaUrl);
        if (!MediaUrl.StartsWith("http"))
        {
            MediaUrl = Config.Domain + MediaUrl;
        }
        
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("MC_GetWebIcon: " + MediaUrl + ", can not be loaded.");
            Debug.Log(request.error);
            Target.gameObject.SetActive(false);
        }
        else
        {
            //Debug.Log("MC_GetWebIcon: " + MediaUrl + " loaded.");
            Target.gameObject.SetActive(true);

            // load texture from data
            Texture2D texture2d = ((DownloadHandlerTexture)request.downloadHandler).texture;

            // set minimap to texture
            Texture2D loadedTexture = new Texture2D(texture2d.width, texture2d.height, texture2d.format, true);

            loadedTexture.LoadImage(request.downloadHandler.data);

            Target.texture = loadedTexture;

            Utility.SizeToParent(Target, Padding);

        }
    }
    */


}
