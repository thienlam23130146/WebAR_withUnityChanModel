using System.Runtime.InteropServices;
using UnityEngine;

public static class BrowserConsole
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void BrowserLog(string msg);
#endif

    // Tự động chạy 1 lần khi game khởi động, ngay cả khi không có GameObject nào gọi tới.
    // Mục đích: chuyển hướng TOÀN BỘ Debug.Log/LogWarning/LogError (kể cả của uLipSync,
    // ARAudioLipSync...) ra console của trình duyệt, vì Debug.Log bị "nuốt" trong
    // bản build WebGL release (non-development).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Application.logMessageReceived -= OnLog; // tránh đăng ký trùng
        Application.logMessageReceived += OnLog;
    }

    private static void OnLog(string condition, string stackTrace, LogType type)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Chỉ kèm stack trace cho lỗi để console khỏi rối.
        if (type == LogType.Error || type == LogType.Exception)
            BrowserLog($"[{type}] {condition}\n{stackTrace}");
        else
            BrowserLog($"[{type}] {condition}");
#endif
    }

    public static void Log(string msg)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        BrowserLog(msg);
#else
        Debug.Log(msg);
#endif
    }
}
