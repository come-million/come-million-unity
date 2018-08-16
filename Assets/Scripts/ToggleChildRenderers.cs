using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleChildRenderers : MonoBehaviour
{
    public bool ChildrenEnabled
    {
        set
        {
            System.Array.ForEach(GetComponentsInChildren<MeshRenderer>(), m => m.enabled = value);
        }
    }

    //public bool Initial = true;

    //void Awake()
    //{
    //    ChildrenEnabled = Initial;
    //}
}
