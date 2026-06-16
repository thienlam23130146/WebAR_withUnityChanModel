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
        Debug.Log(">> down Audio from Server: " + url);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                targetAudioSource.clip = clip;
                targetAudioSource.Play();
            }
            else
            {
                Debug.LogError(">> ERRoR Audio: " + www.error);
            }
        }
    }
}