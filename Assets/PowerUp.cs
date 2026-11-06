using System.Collections;
using UnityEngine;

public enum PowerUpType { Speed, Shield, Magnet }

public class PowerUp : MonoBehaviour
{
    public PowerUpType type = PowerUpType.Speed;
    public float multiplier = 1.6f, duration = 5f, respawnTime = 12f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        switch (type)
        {
            case PowerUpType.Speed:  other.GetComponent<PlayerController>().BoostSpeed(multiplier, duration); break;
            case PowerUpType.Shield: other.GetComponent<Health>().shielded = true; break;
            case PowerUpType.Magnet: other.GetComponent<PlayerController>().Magnet(6f); break;
        }
        StartCoroutine(Hide());   // only hides the pickup (GameObject stays active), the effect runs on the player
    }

    IEnumerator Hide()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(respawnTime);
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }
}
