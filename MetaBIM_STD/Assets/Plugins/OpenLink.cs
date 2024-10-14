using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public static void Open(string url)
    {
    #if !UNITY_EDITOR
		openWindow(url);
    #endif
    }

    [DllImport("__Internal")]
    private static extern void openWindow(string url);
}
