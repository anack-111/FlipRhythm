using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ZigZagMapGenerator : EditorWindow
{
    // �Է�
    public AudioClip audioClip;
    public bool overrideBpm = false;
    public float manualBpm = 120f;

    // ������� �Ķ����
    public float segmentLength = 3.0f;    // �� ���ڴ� �� ����(���� Y)
    public float baseAngle = 45f;         // ������� �⺻ ����
    public float angleByEnergy = 20f;     // �������� ����� �߰��� ���̴� ����
    public float minAngle = 15f;          // �ʹ� �������� �ʰ� �ּҰ�
    public float maxAngle = 75f;          // �ʹ� �ް����� �ʰ� �ִ밢
    public bool toggleEveryBeat = true;   // �� ���ڸ��� �¡�� ��ȯ
    public float energyToggleThreshold = 1.25f; // ������ ���� ���� ��ȯ �ɼǿ�

    // �����/����
    public bool markReverseOnLowEnergy = true;
    public float reverseEnergyThreshold = 0.75f; // ��� ��� ������ reverse �÷���

    // ������
    public int analysisDownsample = 1024; // FFT ��� ���� ������ �����
    public int detectorWindow = 2048;     // ������ �̵���� ������(���ü�)
    public float noiseFloor = 0.0005f;

    [MenuItem("Tools/RhythmZigZag Map Generator")]
    static void Open() => GetWindow<ZigZagMapGenerator>("ZigZag Generator");

    void OnGUI()
    {
        GUILayout.Label("Audio", EditorStyles.boldLabel);
        audioClip = (AudioClip)EditorGUILayout.ObjectField("AudioClip", audioClip, typeof(AudioClip), false);

        overrideBpm = EditorGUILayout.Toggle("Override BPM", overrideBpm);
        if (overrideBpm) manualBpm = EditorGUILayout.FloatField("Manual BPM", manualBpm);

        EditorGUILayout.Space();
        GUILayout.Label("ZigZag Params", EditorStyles.boldLabel);
        segmentLength = EditorGUILayout.Slider("Segment Length (world)", segmentLength, 1f, 10f);
        baseAngle = EditorGUILayout.Slider("Base Angle (deg)", baseAngle, 5f, 85f);
        angleByEnergy = EditorGUILayout.Slider("Angle by Energy", angleByEnergy, 0f, 45f);
        minAngle = EditorGUILayout.Slider("Min Angle", minAngle, 0f, 45f);
        maxAngle = EditorGUILayout.Slider("Max Angle", maxAngle, 45f, 85f);

        toggleEveryBeat = EditorGUILayout.Toggle("Toggle Direction Every Beat", toggleEveryBeat);
        energyToggleThreshold = EditorGUILayout.Slider("Energy Toggle Threshold", energyToggleThreshold, 1.0f, 2.0f);

        EditorGUILayout.Space();
        GUILayout.Label("Reverse/FX", EditorStyles.boldLabel);
        markReverseOnLowEnergy = EditorGUILayout.Toggle("Mark Reverse on Low Energy", markReverseOnLowEnergy);
        reverseEnergyThreshold = EditorGUILayout.Slider("Reverse Energy Threshold", reverseEnergyThreshold, 0.5f, 1.2f);

        EditorGUILayout.Space();
        GUILayout.Label("Analysis", EditorStyles.boldLabel);
        analysisDownsample = EditorGUILayout.IntSlider("Downsample", analysisDownsample, 256, 4096);
        detectorWindow = EditorGUILayout.IntSlider("Energy Window", detectorWindow, 512, 8192);
        noiseFloor = EditorGUILayout.Slider("Noise Floor", noiseFloor, 0f, 0.01f);

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Beatmap SO"))
        {
            if (audioClip == null) { Debug.LogError("AudioClip�� �ʿ��մϴ�."); return; }
            Generate();
        }
    }

    void Generate()
    {
        // 1) ����� ���� �б�
        int channels = audioClip.channels;
        int totalSamples = audioClip.samples * channels;
        float[] data = new float[totalSamples];
        audioClip.GetData(data, 0);

        // 2) ��� �ռ� + �ٿ����
        List<float> mono = new List<float>();
        for (int i = 0; i < totalSamples; i += channels * (analysisDownsample / Mathf.Max(1, audioClip.frequency / 44100)))
        {
            float sum = 0f;
            for (int c = 0; c < channels; c++)
                sum += data[Mathf.Min(i + c, totalSamples - 1)];
            mono.Add(Mathf.Abs(sum / channels));
        }
        float[] monoArr = mono.ToArray();

        // 3) ������(���� ���) �ñ׳� ����
        float[] energy = new float[monoArr.Length];
        for (int i = 0; i < monoArr.Length; i++)
            energy[i] = monoArr[i] * monoArr[i];

        // �̵�������� ��� ������ ����
        float[] avg = MovingAverage(energy, Mathf.Max(8, detectorWindow / analysisDownsample));
        float globalMean = 0f;
        foreach (var e in energy) globalMean += e;
        globalMean /= energy.Length;
        globalMean = Mathf.Max(globalMean, noiseFloor);

        // 4) BPM ���� (���� �����ڸ����̼�)
        float bpm = overrideBpm ? manualBpm : EstimateBpm(energy, audioClip.frequency / (float)analysisDownsample);
        if (bpm < 60f || bpm > 220f) bpm = Mathf.Clamp(bpm, 60f, 220f);

        // 5) ���� Ÿ�ӽ����� ����
        float secPerBeat = 60f / bpm;
        float duration = audioClip.length;
        List<float> beatTimes = new List<float>();
        for (float t = 0f; t < duration; t += secPerBeat)
            beatTimes.Add(t);

        // 6) ��Ʈ�� ��� ������(��� ��� ����)
        //    beat time �ֺ��� ������ â�� ���ø��� ���� �Ǵ�
        float sampleRate = audioClip.frequency / (float)analysisDownsample;
        int window = Mathf.RoundToInt(0.05f * sampleRate); // ��50ms
        List<float> beatEnergyRatio = new List<float>();
        for (int b = 0; b < beatTimes.Count; b++)
        {
            int idx = Mathf.RoundToInt(beatTimes[b] * sampleRate);
            float sum = 0f; int cnt = 0;
            for (int k = -window; k <= window; k++)
            {
                int j = idx + k;
                if (j >= 0 && j < energy.Length) { sum += energy[j]; cnt++; }
            }
            float local = (cnt > 0) ? (sum / cnt) : globalMean;
            beatEnergyRatio.Add(Mathf.Max(local / globalMean, 0.0001f));
        }

        // 7) BeatmapSO ���� �� ���׸�Ʈ ä���
        var asset = ScriptableObject.CreateInstance<BeatmapSO>();
        asset.music = audioClip;
        asset.bpm = bpm;

        int dir = 1; // 1=��, -1=��
        for (int i = 0; i < beatTimes.Count; i++)
        {
            float eRatio = beatEnergyRatio[i];
            bool strong = eRatio >= energyToggleThreshold;

            if (toggleEveryBeat || strong) dir *= -1;

            // ������ ��� ���� ����
            float ang = Mathf.Clamp(baseAngle + angleByEnergy * (eRatio - 1f), minAngle, maxAngle);
            ang = dir > 0 ? +ang : -ang;

            // reverse ��ŷ(���� ������ ����)
            bool reverse = markReverseOnLowEnergy && (eRatio < reverseEnergyThreshold);

            asset.segments.Add(new BeatmapSO.Segment
            {
                time = beatTimes[i],
                length = segmentLength,   // �� ���ڴ� ����
                angle = ang,
                speed = 1f,               // ���ϸ� eRatio ������ �ӵ� ����
                reverse = reverse
            });
        }

        // 8) ����
        string path = EditorUtility.SaveFilePanelInProject("Save Beatmap", "Beatmap_" + audioClip.name, "asset", "");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Done", $"���� �Ϸ�\nBPM? {bpm:0.0}\nSegments: {asset.segments.Count}", "OK");
        }
    }

    static float[] MovingAverage(float[] src, int window)
    {
        window = Mathf.Max(1, window);
        float[] dst = new float[src.Length];
        float acc = 0; int n = 0;
        Queue<float> q = new Queue<float>();
        for (int i = 0; i < src.Length; i++)
        {
            acc += src[i]; q.Enqueue(src[i]); n++;
            if (n > window) { acc -= q.Dequeue(); n--; }
            dst[i] = acc / n;
        }
        return dst;
    }

    // ���� �ܼ��� BPM ����(�����ڸ����̼� �ٻ�). ��Ȯ�� ���� �ʾƵ� ������׿� ���.
    static float EstimateBpm(float[] energy, float sr)
    {
        // 60~220 BPM ���� Ž��
        float bestBpm = 120f;
        float bestScore = -1f;

        for (int bpm = 60; bpm <= 220; bpm++)
        {
            float secPerBeat = 60f / bpm;
            int lag = Mathf.Max(1, Mathf.RoundToInt(secPerBeat * sr));
            double score = 0;
            for (int i = lag; i < energy.Length; i++)
                score += energy[i] * energy[i - lag];
            if (score > bestScore) { bestScore = (float)score; bestBpm = bpm; }
        }
        return bestBpm;
    }
}
