using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public AudioSource audioSource;

    float[] fft = new float[128];
    float[] fft2 = new float[64];

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
    }
}
