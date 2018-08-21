using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DimAudioSpectrumGraph : MonoBehaviour
{
    public bool DebugDraw = true;
    public bool InvertedDebugDraw = false;

    public float m_HScale = 20.0f;
	public float m_VScale = 70.0f;
	public float m_dBRange = 35.0f;
	public Color m_StartColor = Color.red;
	public Color m_EndColor = Color.yellow;
    public bool m_UseListener = true;
    public AudioSource m_AudioSource;

	private const int m_SpectrumSize = 1024;	// must be power of 2 in range 64 to 8192
    public float[] SpectrumValues;
    //public int m_NumFreqBands = 30;
    public float m_BandRatio = 1.5f;//1.25f;
    public float m_FreqCrossoverLowestFreq = 30.0f;
    public int m_NumFreqCrossoverValues = 80;
    public float[] m_FreqCrossover;// = new float[]{ 30, 63, 125, 250, 500, 1000, 2000, 4000, 8000, 16000 };

	private Texture2D m_BarTexture;
	private float m_SpectrumFreqStep;
	private float[] m_SpectrumLeft = new float[m_SpectrumSize];		// left audio channel
	private float[] m_SpectrumRight = new float[m_SpectrumSize];    // right audio channel
                                                                    // These arrays represent normalised amplitudes of each frequency band from 0Hz to the nyquist rate

    //private bool MatchCrossoverValuesAmountWithSurfaceStrands = false;//true;

	private void Start()
	{
        m_SpectrumLeft = new float[m_SpectrumSize];     // left audio channel
        m_SpectrumRight = new float[m_SpectrumSize];    // right audio channel


        //////if (LEDSurfaceColorsHandler.Instance != null && MatchCrossoverValuesAmountWithSurfaceStrands)
        ////if (LEDSurfaceColorsHandler.Instance != null && MatchCrossoverValuesAmountWithSurfaceStrands)
        ////{
        ////    m_NumFreqCrossoverValues = LEDSurfaceColorsHandler.NumSurfaceStrands;
        ////    m_FreqCrossover = new float[m_NumFreqCrossoverValues];
        ////    m_FreqCrossover[0] = m_FreqCrossoverLowestFreq;
        ////    for (int i = 1; i < m_FreqCrossover.Length; i++)
        ////    {
        ////        m_FreqCrossover[i] = m_FreqCrossover[i - 1] * m_BandRatio;
        ////    }
        ////}
        ////SpectrumValues = new float[m_NumFreqCrossoverValues];
        //////SpectrumValues = new float[m_FreqCrossover.Length];



        float nyquistFreq = AudioSettings.outputSampleRate / 2;
		m_SpectrumFreqStep = nyquistFreq / m_SpectrumSize;

#if false
		// automatic generation of crossover frequencies
        float hzMultiplier = Mathf.Pow(nyquistFreq, 1.0f / (float)m_FreqCrossover.Length);
		m_FreqCrossover[0] = hzMultiplier;
        for (int i = 1; i < m_FreqCrossover.Length; ++i)
			m_FreqCrossover[i] = m_FreqCrossover[i - 1] * hzMultiplier;
#endif

		// prepare texture for bar graph
		int width = (int)m_VScale;
		m_BarTexture = new Texture2D(width, 1);
		for (int i = 0; i < width; ++i)
			m_BarTexture.SetPixel(i, 0, Color.Lerp(m_StartColor, m_EndColor, (float)i / m_VScale));
		m_BarTexture.wrapMode = TextureWrapMode.Clamp;
		m_BarTexture.Apply();


#if false
		// debug
		{
            Utils.Log(string.Format("outputSampleRate={0} spectrumSize={1} numFreqBands={2} freqStep={3}Hz", AudioSettings.outputSampleRate, m_SpectrumSize, m_FreqCrossover.Length, m_SpectrumFreqStep));

			float spectrumFreq = 0.0f;
			int freqBand = 0;
			for (int i = 0; i < m_SpectrumSize; ++i)
			{
				spectrumFreq += m_SpectrumFreqStep;
				if (spectrumFreq >= m_FreqCrossover[freqBand] || i == (m_SpectrumSize - 1))
				{
					Utils.Log(string.Format("freqBand {0}: i={1} {2:0.00}Hz (crossover {3:0}Hz)", freqBand, i, spectrumFreq, m_FreqCrossover[freqBand]));
                    if (++freqBand >= m_FreqCrossover.Length)
						break;
				}
			}
		}
#endif
	}


	private void OnGUI()
	{
        //m_FreqCrossover[0] = m_FreqCrossoverLowestFreq;
        //for (int i = 1; i < m_FreqCrossover.Length; i++)
        //{
        //    m_FreqCrossover[i] = m_FreqCrossover[i - 1] * m_BandRatio;
        //}

        if (m_UseListener)
        {
            AudioListener.GetSpectrumData(m_SpectrumLeft, 0, FFTWindow.Rectangular);
            AudioListener.GetSpectrumData(m_SpectrumRight, 1, FFTWindow.Rectangular);
        }
        else if (m_AudioSource)
        {
            m_AudioSource.GetSpectrumData(m_SpectrumLeft, 0, FFTWindow.Rectangular);
        }

        //GuiHelper.SetOrigin(GuiHelper.Origin.BottomLeft);

        float spectrumFreq = 0.0f;
		int freqBand = 0;
		//float barWidth = m_HScale;
		float maxValue = 0.0f;
		//float accumValue = 0.0f;
		//int accumCount = 0;
		//float average = 0.0f;
		//float power = 0.0f;
		for (int i = 0; i < m_SpectrumSize; ++i)
		{
			maxValue = Mathf.Max(maxValue, m_SpectrumLeft[i]);
			maxValue = Mathf.Max(maxValue, m_SpectrumRight[i]);

			//accumValue += m_SpectrumLeft[i];
			//accumValue += m_SpectrumRight[i];
			//accumCount += 2;

			//if (m_SpectrumLeft[i] > 0.0001f) // arbitrary cut-off to filter out noise
			//{
			//	average += m_SpectrumLeft[i] * spectrumFreq;
			//	power += m_SpectrumLeft[i];
			//}
			//if (m_SpectrumRight[i] > 0.0001f) // arbitrary cut-off to filter out noise
			//{
			//	average += m_SpectrumRight[i] * spectrumFreq;
			//	power += m_SpectrumRight[i];
			//}

			spectrumFreq += m_SpectrumFreqStep;

			if (spectrumFreq >= m_FreqCrossover[freqBand] || i == (m_SpectrumSize - 1))
			{
				// decibels relative to full scale
				float dBFS = 20.0f * Mathf.Log10(maxValue);		// range -infinity -> 0

				// clamp to desired dB range for bar graph
				float value = Mathf.Clamp01(1.0f + (dBFS / m_dBRange));		// range 0.0 -> 1.0
                SpectrumValues[freqBand] = value;

                if (DebugDraw)
                {
                    ////float centre_x = Screen.width / 2 - (m_FreqCrossover.Length / 2) * barWidth + (freqBand + 0.5f) * barWidth;

                    ////if (InvertedDebugDraw)
                    ////{
                    ////    Vector2 bottom = new Vector2(centre_x, Screen.height - value * m_VScale);
                    ////    Vector2 top = new Vector2(centre_x, Screen.height);
                    ////    //GuiHelper.DrawLine(bottom, top, m_BarTexture, barWidth);
                    ////}
                    ////else
                    ////{
                    ////    Vector2 bottom = new Vector2(centre_x, 0);
                    ////    Vector2 top = new Vector2(centre_x, value * m_VScale);
                    ////    //GuiHelper.DrawLine(bottom, top, m_BarTexture, barWidth);
                    ////}                    
                }

				// reset for next frequency band
				maxValue = 0.0f;
                //accumValue = 0.0f;
                //accumCount = 0;

                freqBand++;
                if (freqBand >= m_FreqCrossover.Length || freqBand >= SpectrumValues.Length)
					break;
			}
		}

		// find dominant frequency
		//float dominantHz = (power > 0.0001f) ? (average / power) : 0.0f;
		//Utils.Log(dominantHz.ToString("0"));
	}
}

