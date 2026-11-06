using UnityEngine;

public class Health : MonoBehaviour
{
    public int max = 3;
    public float iFrames = 1f;
    public bool shielded;
    int current; float invulnerableUntil; Renderer rend;

    public int Current => current;
    public int Max => max;

    void Awake() { current = max; rend = GetComponentInChildren<Renderer>(); }

    public bool TakeDamage(int amount)
    {
        if (current <= 0 || Time.time < invulnerableUntil) return false;
        invulnerableUntil = Time.time + iFrames;          // i-frames first, even if the shield absorbs it
        if (shielded) { shielded = false; return false; }
        current -= amount;
        if (current <= 0) GameManager.Instance.PlayerDied();   // <= 0, never == 0
        return true;
    }

    public void Restore(float protection)
    {
        current = max;
        invulnerableUntil = Time.time + protection;
    }

    void Update()
    {
        // blink while invulnerable
        rend.enabled = Time.time >= invulnerableUntil || ((int)(Time.time * 12f) % 2 == 0);
    }
}
