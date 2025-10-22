using UnityEngine;

public class AudioAnalyzer : MonoBehaviour
{
    public AudioClip audioClip;
    private float[] samples;
    private int sampleRate;

    void Start()
    {
        // 오디오 샘플을 로드합니다
        samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);
        sampleRate = audioClip.frequency;

        // FFT 분석
        AnalyzeAudio(samples);
    }

    void AnalyzeAudio(float[] samples)
    {
        int fftSize = 1024;
        float[] fftResult = new float[fftSize];
        for (int i = 0; i < samples.Length - fftSize; i += fftSize)
        {
            // FFT 분석을 위한 샘플 추출
            System.Array.Copy(samples, i, fftResult, 0, fftSize);

            // 여기서 FFT 분석을 하고, 음높이(Pitch)를 추출합니다.
            // 예: FFT 알고리즘을 통해 주파수 분석
            float pitch = GetPitchFromFFT(fftResult);
            Debug.Log("Pitch: " + pitch);
        }
    }

    float GetPitchFromFFT(float[] fftResult)
    {
        // FFT 결과에서 음 높이 추출하는 코드 구현
        // 예: 최대 주파수 값이 가장 중요한 음 높이
        return 0f;
    }
}
