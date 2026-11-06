using System.Collections;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damage = 1;
    public float knockback = 6f;
    public float restTime = 0f, warnTime = 0.6f, strikeTime = 0.4f;

    Collider zone; Renderer rend;

    IEnumerator Start()
    {
        zone = GetComponent<Collider>();
        if (restTime <= 0f) yield break;
        rend = GetComponent<Renderer>();
        Color orange = new Color(1f, 0.54f, 0.24f);
        while (true)
        {
            zone.enabled = false;
            rend.material.color = orange * 0.35f;
            yield return new WaitForSeconds(restTime);
            rend.material.color = orange;                  // telegraph
            yield return new WaitForSeconds(warnTime);
            rend.material.color = Color.red;
            zone.enabled = true;
            yield return new WaitForSeconds(strikeTime);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out Health health)) return;
        if (!health.TakeDamage(damage)) return;            // only a hit that lands knocks back
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;
        Vector3 away = body.position - transform.position;
        away.y = 0f;
        body.AddForce(away.normalized * knockback, ForceMode.Impulse);
    }
}
