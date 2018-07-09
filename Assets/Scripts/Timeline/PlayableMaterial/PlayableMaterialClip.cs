using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class PlayableMaterialClip : PlayableAsset, ITimelineClipAsset
{
    public PlayableMaterialBehaviour template = new PlayableMaterialBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<PlayableMaterialBehaviour>.Create(graph, template);
        return playable;
    }
}