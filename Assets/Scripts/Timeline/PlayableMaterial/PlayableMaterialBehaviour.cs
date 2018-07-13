using System;
using System.Collections;
using System.Collections.Generic;
using ComeMillion;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class PlayableMaterialBehaviour : PlayableBehaviour
{
    public Material material;
    public int index;
    private TileGroup m_TrackBinding;

    bool m_FirstFrameHappened;

    int alphaProp = Shader.PropertyToID("_Alpha");

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as TileGroup;

        if (m_TrackBinding == null)
            return;


        if (!m_FirstFrameHappened)
        {
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<PlayableMaterialBehaviour> inputPlayable = (ScriptPlayable<PlayableMaterialBehaviour>)playable.GetInput(i);
            PlayableMaterialBehaviour input = inputPlayable.GetBehaviour();

            // Debug.LogFormat("weight: {0}, mat:{1}, idx:{2}", inputWeight, input.material, input.index);


            // if (inputWeight == 1)
            if (inputWeight > 0)
            {
                m_TrackBinding.materials[input.index] = input.material;
            }

            if (input.material != null)
                input.material.SetFloat(alphaProp, inputWeight);
        }
    }

    // public override void OnPlayableDestroy(Playable playable)
    // {
    //     m_FirstFrameHappened = false;

    //     if (m_TrackBinding == null)
    //         return;

    //     m_TrackBinding.materials[index] = material;
    // }
}
