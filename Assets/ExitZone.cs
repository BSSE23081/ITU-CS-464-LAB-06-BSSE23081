using UnityEngine;

public class ExitZone : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.ExitUnlocked)
            GameManager.Instance.Win();
    }
}
