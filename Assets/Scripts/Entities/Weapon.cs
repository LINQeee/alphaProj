using UnityEngine;    

public class Weapon:MonoBehaviour
{
    [field: SerializeField] public Transform leftHandIK { get; private set; }
    [field: SerializeField] public AnimSet.AnimSetTypes parentAnimType { get; private set; }

    public void SyncWeaponIK(Transform leftHandTarget)
    {
        leftHandTarget.SetPositionAndRotation(leftHandIK.position, leftHandIK.rotation);
    }
}