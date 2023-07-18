using System;
using UnityEngine;

[Serializable]
public class HorseInteractionTransforms
{
    [field: SerializeField]
    public Transform leftMountPlace { get; set; }
    [field: SerializeField]
    public Transform rightMountPlace { get; set; }
    [field: SerializeField]
    public Transform riderPlace { get; set; }
}