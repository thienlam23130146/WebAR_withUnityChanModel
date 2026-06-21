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
        BrowserConsole.Log(">> down Audio from Server: " + url);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                targetAudioSource.clip = clip;
                targetAudioSource.Play();
                BrowserConsole.Log(">> Done down voice!uLipSync active");
            }
            else
            {
                BrowserConsole.Log(">> ERRoR Audio: " + www.error);
            }
        }
    }
}