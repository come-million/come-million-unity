using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public AudioSource audioSource;

    float[] fft = new float[128];
    float[] fft2 = new float[64];

    public class Filter
    {
        public float lpf = 0.0f;
        public float bpf = 0.0f;
        public float env = 0.0f;

        public float Process(float input, float cut, float bw)
        {
            lpf += cut * bpf;
            float hpf = input - lpf - bpf * bw;
            bpf += cut * hpf;
            float a = bpf * bpf;
            if (a > env)
                env = a;
            else
                env *= 0.999f;
            return env;
        }
    };

    public float LowCut = 0.01f;
    public float MidCut = 0.1f;
    public float HighCut = 0.4f;

    public float LowBW = 0.01f;
    public float MidBW = 0.01f;
    public float HighBW = 0.01f;

    private Filter FilterLow = new Filter();
    private Filter FilterMid = new Filter();
    private Filter FilterHigh = new Filter();

    void Start()
    {
        foreach (string device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource = GetComponent<AudioSource>();
		audioSource.clip = Microphone.Start(null, true, 1, 44100);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { 
            // wait for init 
        }

        audioSource.Play();
    }

    void Update()
    {
        audioSource.GetSpectrumData(fft, 0, FFTWindow.Rectangular);
        // Shader.SetGlobalFloatArray("_FFT", fft);
        System.Array.Copy(fft, fft2, fft2.Length);
        Shader.SetGlobalFloatArray("_FFT", fft2);

        Shader.SetGlobalFloat("_MicLow", FilterLow.env);
        Shader.SetGlobalFloat("_MicMid", FilterMid.env);
        Shader.SetGlobalFloat("_MicHigh", FilterHigh.env);
    }


    void OnAudioFilterRead(float[] data, int numchannels)
    {
        for (int n = 0; n < data.Length; n += numchannels)
        {
            FilterLow.Process(data[n], LowCut, LowBW);
            FilterMid.Process(data[n], MidCut, MidBW);
            FilterHigh.Process(data[n], HighCut, HighBW);
        }
    }
}
