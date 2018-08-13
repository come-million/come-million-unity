using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineController : MonoBehaviour
{
    public List<PlayableDirector> playableDirectors;
    public List<TimelineAsset> timelines;

    public bool timelineIsPlaying = false;

    public int NumTimelinesPlaying = 0;
    public float TimerPlaystatePause = 0.0f;
    public float TimerPlaystatePlay = 0.0f;
    public float TimerTotal = 0.0f;

    public int CurrentTimelinePlaying = -1;
    public int NextTimelineToPlay = 0;

    public Material[] SkinMats;
    public float SkinBrightnessValue = 0.0f;
    public float TimeToPause = 15.0f;
    public float TimeForSkinTransition = 10.0f;
    public float SkinBrightnessMin = 0.05f;
    public float SkinBrightnessMax = 0.3f;
    public float PulseTimeScale = 1.0f;

    public float DebugTime = 0.0f;
    public bool HoldTimelinesPlayback = false;

    public bool DebugView = false;


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

        SkinBrightnessValue = 0.0f;
        UpdateSkinBrightness(0.0f);
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
        if (SkinMats != null && SkinMats.Length > 0)
        {
            SkinBrightnessValue = Mathf.Lerp(prevSkinBrightnessValue, SkinBrightnessValue, 0.005f);
            for (int i = 0; i < SkinMats.Length; i++)
            {
                float alphaValue = Mathf.Lerp(SkinBrightnessMin, SkinBrightnessMax, SkinBrightnessValue);
                //if (i < SkinMats.Length - 1)
                //    alphaValue += 0.1f * alphaValue * Mathf.Sin(PulseTimeScale * Time.timeSinceLevelLoad * (float)(i+1) * 2.0f * Mathf.PI);
                SkinMats[i].SetFloat("_Alpha", alphaValue);
                if (DebugView)
                    Debug.LogWarning("SkinBrightnessValue: " + SkinBrightnessValue.ToString());
            }
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
            Stop(CurrentTimelinePlaying);
            Play(NextTimelineToPlay);
            CurrentTimelinePlaying = NextTimelineToPlay;
            HoldTimelinesPlayback = true;
            NextTimelineToPlay = (NextTimelineToPlay + 1) % playableDirectors.Count;
        }
    }
}