using System;
using UnityEngine;

[Serializable]
public class AnimSet
{
    [field: SerializeField] public AnimSetTypes animSetType { get; private set; }
    [field: SerializeField] public string baseLayerAnimName { get; private set; }
    [field: SerializeField] public string aimLayerAnimName { get; private set; }
    [field: SerializeField] public string inAirAnimName { get; private set; }
    [field: SerializeField] public string jumpAnimName { get; private set; }

    public enum AnimSetTypes
    {
        Unarmed,
        Sword
    }
}