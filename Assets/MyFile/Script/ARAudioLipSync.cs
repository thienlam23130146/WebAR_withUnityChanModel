using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ARAudioLipSync : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Mouth BlendShape")]
    public SkinnedMeshRenderer faceRenderer;
    public int mouthOpenBlendShapeIndex;
    public float sensitivity = 35f;
    public float smoothSpeed = 12f;

    private readonly float[] samples = new float[256];
    private Coroutine downloadRoutine;
    private float currentMouthWeight;

    public void PlayFromUrl(string audioUrl)
    {
        if (string.IsNullOrWhiteSpace(audioUrl))
        {
            Debug.LogWarning("[AR AUDIO] AudioUrl rỗng");
            return;
        }

        if (downloadRoutine != null)
            StopCoroutine(downloadRoutine);

        downloadRoutine = StartCoroutine(DownloadAndPlay(audioUrl));
    }

    private IEnumerator DownloadAndPlay(string audioUrl)
    {
        Debug.Log("[AR AUDIO] Đang tải: " + audioUrl);

        using UnityWebRequest request =
            UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.WAV);

        request.timeout = 20;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[AR AUDIO] Lỗi: " + request.error);
            downloadRoutine = null;
            yield break;
        }

        AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

        if (clip == null)
        {
            Debug.LogError("[AR AUDIO] Không đọc được WAV");
            downloadRoutine = null;
            yield break;
        }

        audioSource.Stop();

        if (audioSource.clip != null)
            Destroy(audioSource.clip);

        audioSource.clip = clip;
        audioSource.Play();

        Debug.Log("[AR AUDIO] PLAY SUCCESS");
        Debug.Log("[AR AUDIO] AudioSource Object = " + audioSource.gameObject.name);
        Debug.Log("[AR AUDIO] isPlaying = " + audioSource.isPlaying);
        Debug.Log("[AR AUDIO] Clip length = " + audioSource.clip.length);

        Debug.Log($"[AR AUDIO] Đang phát {clip.length:F2} giây");
        downloadRoutine = null;
    }

    private void Update()
    {
        float targetWeight = 0f;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.GetOutputData(samples, 0);

            float level = 0f;
            foreach (float sample in samples)
                level += Mathf.Abs(sample);

            level /= samples.Length;
            targetWeight = Mathf.Clamp01(level * sensitivity) * 100f;
        }

        currentMouthWeight = Mathf.Lerp(
            currentMouthWeight,
            targetWeight,
            smoothSpeed * Time.deltaTime
        );

        Debug.Log("[LIP] mouthWeight = " + currentMouthWeight);

        if (faceRenderer != null && mouthOpenBlendShapeIndex >= 0)
        {
            faceRenderer.SetBlendShapeWeight(
                mouthOpenBlendShapeIndex,
                currentMouthWeight
            );
        }
    }
}