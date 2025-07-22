using UnityEngine;

public class SilentMicSpeechDetector : MonoBehaviour
{
    public string micDevice;
    public float loudnessThreshold = 0.01f;
    public float silenceTimeout = 1.0f;

    private AudioClip micClip;
    private float timeSinceLastSpeech = 0f;
    private bool isSpeaking = false;
    private int sampleWindow = 128;

    void Start()
    {
        // Get default microphone
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            micClip = Microphone.Start(micDevice, true, 10, 44100); // 10 sec looping clip
            Debug.Log("Microphone started: " + micDevice);
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    void Update()
    {
        float loudness = GetMicLoudness();

        if (loudness > loudnessThreshold)
        {
            if (!isSpeaking)
            {
                isSpeaking = true;
                Debug.Log("🔊 Started Speaking");
            }

            timeSinceLastSpeech = 0f;
        }
        else
        {
            timeSinceLastSpeech += Time.deltaTime;

            if (isSpeaking && timeSinceLastSpeech >= silenceTimeout)
            {
                isSpeaking = false;
                Debug.Log("🔇 Stopped Speaking");
            }
        }

        // Optional: Debug loudness
        Debug.Log($"🎙 Loudness: {loudness:F4}");
    }

    float GetMicLoudness()
    {
        if (micClip == null || !Microphone.IsRecording(micDevice))
            return 0f;

        int micPosition = Microphone.GetPosition(micDevice);
        if (micPosition < sampleWindow)
            return 0f;

        float[] samples = new float[sampleWindow];
        int startPosition = micPosition - sampleWindow;
        micClip.GetData(samples, startPosition);

        float total = 0f;
        foreach (float sample in samples)
            total += Mathf.Abs(sample);

        return total / sampleWindow;
    }
}
