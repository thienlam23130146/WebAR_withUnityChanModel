using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class VoiceDownloader : MonoBehaviour
{
    [Header("AudioSource")]
    public AudioSource targetAudioSource;

    [Header("Audio format trả về từ server")]
    public AudioType audioType = AudioType.WAV;

    private Coroutine downloadRoutine;

    public void PlayVoiceFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            BrowserConsole.Log(">> [Voice] AudioUrl rỗng - bỏ qua");
            return;
        }

        if (targetAudioSource == null)
        {
            BrowserConsole.Log(">> [Voice] ERROR: targetAudioSource CHƯA gán trong Inspector!");
            return;
        }

        if (downloadRoutine != null) StopCoroutine(downloadRoutine);
        downloadRoutine = StartCoroutine(DownloadAndPlay(url));
    }

    private IEnumerator DownloadAndPlay(string url)
    {
        BrowserConsole.Log(">> [Voice] Tải audio từ server: " + url);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
        {
            // Bắt buộc để uLipSync có thể đọc mẫu bằng clip.GetData trên WebGL:
            // clip phải được giải nén thành PCM, không stream.
            if (www.downloadHandler is DownloadHandlerAudioClip dh)
            {
                dh.streamAudio = false;
                dh.compressed = false;
            }

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                // Lỗi thường gặp trên WebGL: CORS (server thiếu Access-Control-Allow-Origin)
                // hoặc URL/định dạng sai.
                BrowserConsole.Log($">> [Voice] ERROR tải audio: {www.error} | responseCode={www.responseCode} | url={url}");
                downloadRoutine = null;
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            if (clip == null)
            {
                BrowserConsole.Log(">> [Voice] ERROR: clip null (decode thất bại - sai AudioType?)");
                downloadRoutine = null;
                yield break;
            }

            BrowserConsole.Log($">> [Voice] Tải xong. samples={clip.samples}, channels={clip.channels}, freq={clip.frequency}, length={clip.length:F2}s, loadState={clip.loadState}");

            targetAudioSource.Stop();
            targetAudioSource.clip = clip;
            targetAudioSource.Play();

            BrowserConsole.Log($">> [Voice] Play() gọi xong. isPlaying={targetAudioSource.isPlaying} (nếu false -> AudioContext có thể đang bị suspended, cần user tương tác)");
        }

        downloadRoutine = null;
    }
}
