using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AssetLoader : MonoBehaviour
{
    // webpath
    public string BundleExtension = ".unity3d";
    public string BundleURL = "";
    public string BundleName = "";
    public int BundleCRC;
    public int BundleVersion;
    
    public bool isLoading = false;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            if (!isLoading) 
            {
                LoadignAsset();
            }
            else
            {
                Debug.Log("Still Loading");
            }
        }
    }



    public void LoadignAsset()
    {
        isLoading = true;
        StartCoroutine(DownloadAsset(LoadComplete));
    }

    
    public IEnumerator DownloadAsset(Action _callback)
    {

 
        string assetUrl = BundleURL + BundleName + BundleExtension;
        
        Debug.Log("Download Asset: " + assetUrl);
        UnityWebRequest bundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(assetUrl, (uint)BundleCRC, (uint)BundleCRC);
        UnityWebRequestAsyncOperation operation = bundleRequest.SendWebRequest();

        while (!operation.isDone)         
        {
            Debug.Log("Download Progress: " + operation.progress);
            yield return new WaitForSeconds(.1f);
        }

        Debug.Log("Load Asset: " + BundleName);
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(bundleRequest);
        AssetBundleRequest asset = bundle.LoadAssetAsync(BundleName);

        while (!asset.isDone)
        {
            Debug.Log("Loading Progress: " + asset.progress);
            yield return new WaitForSeconds(.1f);
        }

        Debug.Log("Create Object");
        GameObject obj = asset.asset as GameObject;
        Instantiate(obj);

        LoadComplete();
        //yield return StartCoroutine(LoadIntoScene(_callback));
    }


    public IEnumerator LoadIntoScene(Action _callback)
    {

        yield return null;
    }


   public void LoadComplete()
    {
        Debug.Log("Complete");
        isLoading = false;
    }


    
}
