using System.Runtime.InteropServices;
using UnityEngine;

public static class BrowserConsole
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void BrowserLog(string msg);
#endif

    public static void Log(string msg)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        BrowserLog(msg);
#else
        Debug.Log(msg);
#endif
    }
}