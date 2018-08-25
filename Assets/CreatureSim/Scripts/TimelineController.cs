using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineController : MonoBehaviour
{
    public List<PlayableDirector> playableDirectors;
    public List<TimelineAsset> timelines;
    public List<float> timelineCamAlpha;

    public bool timelineIsPlaying = false;

    public int NumTimelinesPlaying = 0;
    public float TimerPlaystatePause = 0.0f;
    public float TimerPlaystatePlay = 0.0f;
    public float TimerTotal = 0.0f;

    public int CurrentTimelinePlaying = -1;
    public int NextTimelineToPlay = 0;

    public Material ARSkin1;
    public Material[] SkinMats;
    public float SkinBrightnessValue = 0.0f;
    public float ProjCamsMaxAlpha = 0.8f;
    public float ProjCamsMinAlpha = 0.1f;
    public float CurrentMaxCamAlpha = 0.0f;
    public float TimeToPause = 15.0f;
    public float TimeForSkinTransition = 10.0f;
    public float SkinBrightnessMin = 0.05f;
    public float SkinBrightnessMax = 0.3f;
    //public float PulseTimeScale = 1.0f;
    public float MinMicAlpha = 0.1f;
    public float MaxMicAlpha = 1.0f;
    public float CurrentMicAlpha = 0.0f;

    public DimAudioSpectrumGraph micSpectrum;

    public BuildUtils buildUtils;



    public Camera ProjectionCam1;
    public Camera ProjectionCam2;

    public bool HoldTimelinesPlayback = false;

    private string SkinName = "Skin";
    private string SkinNameError = "Skin Name Error!";
    private string TimelineNameSuffix = "Timeline";

    private const int timelineNamePrefixLength = 2;


    void Start()
    {
        if (playableDirectors == null || playableDirectors.Count <= 0)
        {
            PlayableDirector[] directorArray = GetComponentsInChildren<PlayableDirector>();

            if (directorArray != null && directorArray.Length > 0)
                playableDirectors.AddRange(directorArray);

            foreach (PlayableDirector dir in playableDirectors)
            {
                dir.time = 0.0f;                     
                dir.Stop();
            }
        }

        SkinBrightnessValue = 1.0f;
        UpdateSkinBrightness(0.0f);
    }


    
    public string GetCurrentTimelineName()
    {
        int currentTimeline = GetCurrentTimeline();
        if (currentTimeline < 0)
            return SkinName;

        if (currentTimeline >= timelines.Count)
            return SkinNameError;

        
        return timelines[currentTimeline].name.Remove(timelines[currentTimeline].name.IndexOf(TimelineNameSuffix) - timelineNamePrefixLength, TimelineNameSuffix.Length + timelineNamePrefixLength);
    }

    public int GetCurrentTimeline()
    {
        if (GetNumTimelinesPlaying() <= 0)
            return -1;

        return CurrentTimelinePlaying;
    }

    public int GetNumTimelinesPlaying()
    {
        int numCurrentlyPlaying = 0;
        for (int i = 0; i < playableDirectors.Count; i++)
        {
            PlayableDirector currentDir = playableDirectors[i];
            //PlayState timelineState = currentDir.state;
            //if (timelineState == PlayState.Playing)
            //    numCurrentlyPlaying++;
            if (currentDir.time > 0.0f && currentDir.time < currentDir.duration)
                numCurrentlyPlaying++;
        }

        return numCurrentlyPlaying;
    }

    public void Play(int index = -1)
    {
        for (int i = 0; i < playableDirectors.Count; i++)
        {
            if (index < 0 || index == i)
                playableDirectors[i].Play();
        }
    }

    public void Stop(int index = -1)
    {
        for (int i = 0; i < playableDirectors.Count; i++)
        {
            if (index < 0 || index == i)
                playableDirectors[i].Stop();
        }
    }

    //public void PlayFromTimelines(int index)
    //{
    //    TimelineAsset selectedAsset;

    //    if (timelines.Count <= index)
    //    {
    //        selectedAsset = timelines[timelines.Count - 1];
    //    }
    //    else
    //    {
    //        selectedAsset = timelines[index];
    //    }

    //    playableDirectors[0].Play(selectedAsset);
    //}



    public void UpdateSkinBrightness(float prevSkinBrightnessValue)
    {
        if (ARSkin1 != null)
        {
            float sumSpectrum = 0.0f;
            for(int i=0; i<micSpectrum.SpectrumValues.Length; i++)
            {
                sumSpectrum += (float)(1 + i) * micSpectrum.SpectrumValues[i];
            }


            CurrentMicAlpha = Mathf.Lerp(MinMicAlpha, MaxMicAlpha, sumSpectrum/3.0f);
            Shader.SetGlobalFloat("_MicLowGlobal", CurrentMicAlpha);
            //ARSkin1.SetFloat("_MicLow", micSpectrum.SpectrumValues[0]);
        }

        ////if (SkinMats != null && SkinMats.Length > 0)
        ////{
        ////    SkinBrightnessValue = Mathf.Lerp(prevSkinBrightnessValue, SkinBrightnessValue, 0.1f);
        ////    for (int i = 0; i < SkinMats.Length; i++)
        ////    {
        ////        float alphaValue = Mathf.Lerp(SkinBrightnessMin, SkinBrightnessMax, Mathf.Pow(SkinBrightnessValue, 2.0f));
        ////        //if (i < SkinMats.Length - 1)
        ////        //{
        ////        //    float differntTimedValuePerSkinMat = 1.0f + 0.1f * (float)i;
        ////        //    alphaValue = alphaValue * (0.3f + 0.7f) * Mathf.Abs(Mathf.Sin(PulseTimeScale * Time.timeSinceLevelLoad * differntTimedValuePerSkinMat * 2.0f * Mathf.PI));
        ////        //}
        ////        SkinMats[i].SetFloat("_Alpha", alphaValue);
        ////    }
        ////}
    }

    private void UpdateLayersVisibility()
    {
        int currentTimeline = GetCurrentTimeline();
        if (CurrentTimelinePlaying >= 0 && CurrentTimelinePlaying < timelineCamAlpha.Count)
            CurrentMaxCamAlpha = timelineCamAlpha[CurrentTimelinePlaying];

        if (!timelineIsPlaying)//currentTimeline < 0)
        {
            float newAlpha = Mathf.Lerp(CurrentMaxCamAlpha, ProjCamsMinAlpha, SkinBrightnessValue);

            float newAlpha1 = Mathf.Min(newAlpha, ProjectionCam1.backgroundColor.a);
            if (buildUtils != null)
                newAlpha1 *= buildUtils.MainBrightnessSliderValue;

            if (ProjectionCam1 != null)
                ProjectionCam1.backgroundColor = new Color(ProjectionCam1.backgroundColor.r, ProjectionCam1.backgroundColor.g, ProjectionCam1.backgroundColor.b, newAlpha1);

            float newAlpha2 = Mathf.Min(newAlpha, ProjectionCam2.backgroundColor.a);
            if (buildUtils != null)
                newAlpha2 *= buildUtils.MainBrightnessSliderValue;

            if (ProjectionCam2 != null)
                ProjectionCam2.backgroundColor = new Color(ProjectionCam2.backgroundColor.r, ProjectionCam2.backgroundColor.g, ProjectionCam2.backgroundColor.b, newAlpha2);
        }
        else if(currentTimeline == 2 || currentTimeline == 3 || currentTimeline == 5) // projection cam2
        {
            if (ProjectionCam2 != null)
            {
                float newAlpha = Mathf.Lerp(CurrentMaxCamAlpha, ProjCamsMinAlpha, SkinBrightnessValue);
                if (buildUtils != null)
                    newAlpha *= buildUtils.MainBrightnessSliderValue;

                ProjectionCam2.backgroundColor = new Color(ProjectionCam2.backgroundColor.r, ProjectionCam2.backgroundColor.g, ProjectionCam2.backgroundColor.b, newAlpha);
            }
        }
        else if (currentTimeline == 0 || currentTimeline == 1 || currentTimeline == 4) // projection cam1
        {
            if (ProjectionCam1 != null)
            {
                float newAlpha = Mathf.Lerp(CurrentMaxCamAlpha, ProjCamsMinAlpha, SkinBrightnessValue);

                if (buildUtils != null)
                    newAlpha *= buildUtils.MainBrightnessSliderValue;

                ProjectionCam1.backgroundColor = new Color(ProjectionCam1.backgroundColor.r, ProjectionCam1.backgroundColor.g, ProjectionCam1.backgroundColor.b, newAlpha);
            }
        }
        else
        {
            Debug.Assert(true, "Wrong timeline index!");
        }
    }

    private void Update()
    {
        TimerTotal = Time.timeSinceLevelLoad;

        NumTimelinesPlaying = GetNumTimelinesPlaying();

        timelineIsPlaying = NumTimelinesPlaying > 0;

        float prevSkinBrightnessValue = SkinBrightnessValue;
        if (NumTimelinesPlaying <= 0)
        {
            TimerPlaystatePause += Time.deltaTime;
            TimerPlaystatePlay = 0.0f;

            SkinBrightnessValue = Mathf.Clamp01(TimerPlaystatePause / TimeForSkinTransition);
        }
        else
        {
            TimerPlaystatePlay += Time.deltaTime;
            HoldTimelinesPlayback = false;
            TimerPlaystatePause = 0.0f;
            SkinBrightnessValue = Mathf.Clamp01(1.0f - Mathf.Clamp01(TimerPlaystatePlay / TimeForSkinTransition));
        }
        UpdateSkinBrightness(prevSkinBrightnessValue);        


        if (NumTimelinesPlaying <= 0 && TimerPlaystatePause > TimeToPause && !HoldTimelinesPlayback)
        {
            if (CurrentTimelinePlaying >= 0 && CurrentTimelinePlaying < playableDirectors.Count)
                Stop(CurrentTimelinePlaying);

            Play(NextTimelineToPlay);
            CurrentTimelinePlaying = NextTimelineToPlay;
            HoldTimelinesPlayback = true;
            NextTimelineToPlay = (NextTimelineToPlay + 1) % playableDirectors.Count;
        }

        UpdateLayersVisibility();
    }
}