using UnityEngine;

public class RiderTrigger : MonoBehaviour
{
    [SerializeField] private HorseController horseController;

    private void OnTriggerEnter(Collider other)
    {
        SetIsCanMountIfPlayer(other.GetComponent<RiderManager>(), true);
    }

    private void OnTriggerExit(Collider other)
    {
        SetIsCanMountIfPlayer(other.GetComponent<RiderManager>(), false);
    }

    private void SetIsCanMountIfPlayer(RiderManager riderManager, bool value)
    {
        if (riderManager == null) return;
        riderManager.isCanMount = value;
        riderManager.currentHorse = horseController;
    }
}