using System;
using UnityEngine;

[Serializable]
public class TransformPair
{
    public Transform child { private get; set; }
    [field: SerializeField] public Transform parent { private get; set; }

    public void SyncPair()
    {
        child.SetPositionAndRotation(parent.position, parent.rotation);
    }
}