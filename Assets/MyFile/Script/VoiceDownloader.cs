using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class VoiceDownloader : MonoBehaviour
{
    [Header("AudioSource")]
    public AudioSource targetAudioSource;

    private Coroutine downloadRoutine;

    public void PlayVoiceFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        if (downloadRoutine != null) StopCoroutine(downloadRoutine);
        downloadRoutine = StartCoroutine(DownloadAndPlay(url));
    }

    private IEnumerator DownloadAndPlay(string url)
    {
        Debug.Log(">> Bắt đầu tải Audio từ Server: " + url);

        // Lưu ý: Đang dùng AudioType.WAV. Nếu bạn dùng mp3 thì đổi thành AudioType.MPEG
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                // Nhét file âm thanh vào loa và phát
                targetAudioSource.clip = clip;
                targetAudioSource.Play();

                Debug.Log(">> Tải thành công! Đang phát âm thanh, uLipSync sẽ tự động nhép miệng.");
            }
            else
            {
                Debug.LogError(">> Lỗi tải Audio: " + www.error);
            }
        }
    }
}