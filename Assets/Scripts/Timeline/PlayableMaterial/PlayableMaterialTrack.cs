using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;
using ComeMillion;

[TrackColor(0.9454092f, 0.9779412f, 0.3883002f)]
[TrackClipType(typeof(PlayableMaterialClip))]
[TrackBindingType(typeof(TileGroup))]
public class PlayableMaterialTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        foreach (var c in GetClips())
        {
            PlayableMaterialClip clip = (PlayableMaterialClip)c.asset;
            if(clip.template != null && clip.template.material != null)
				c.displayName = clip.template.material.name;
        }

        return ScriptPlayable<PlayableMaterialBehaviour>.Create(graph, inputCount);
    }

//     public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
//     {
// #if UNITY_EDITOR
//         TileGroup trackBinding = director.GetGenericBinding(this) as TileGroup;
//         if (trackBinding == null)
//             return;

//         var serializedObject = new UnityEditor.SerializedObject(trackBinding);
//         var iterator = serializedObject.GetIterator();
//         while (iterator.NextVisible(true))
//         {
//             if (iterator.hasVisibleChildren)
//                 continue;

//             driver.AddFromName<TileGroup>(trackBinding.gameObject, iterator.propertyPath);
//         }
// #endif
//         base.GatherProperties(director, driver);
//     }
}