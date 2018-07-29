using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
	public int deviceIndex;
    public AudioSource source;

    float[] fft = new float[128];
    float[] fft2 = new float[64];

    void Start()
    {
        // foreach (string device in Microphone.devices)
        // {
        //     Debug.Log("Name: " + device);
        // }
		
        // source = GetComponent<AudioSource>();
        // source.clip = Microphone.Start(Microphone.devices[deviceIndex], true, 10, 44100);
        // source.loop = true;
        // // while (!(Microphone.GetPosition(null) > 0)) { }
        // source.Play();
    }

    void Update()
    {
        source.GetSpectrumData(fft, 0, FFTWindow.Rectangular);
        // Shader.SetGlobalFloatArray("_FFT", fft);
        System.Array.Copy(fft, fft2, fft2.Length);
        Shader.SetGlobalFloatArray("_FFT", fft2);
    }
}
