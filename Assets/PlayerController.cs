using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveForce = 27f;
    Rigidbody rb; Vector3 input;
    float speedMultiplier = 1f; Coroutine boost; float magnetUntil;
    public bool Boosted => speedMultiplier > 1f;
    public bool Magnetic => Time.time < magnetUntil;

    void Awake() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        float h = Input.GetAxis("Horizontal"), v = Input.GetAxis("Vertical");
        input = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1f);
    }

    void FixedUpdate()
    {
        rb.AddForce(input * moveForce * speedMultiplier);
        if (Magnetic)
            foreach (Coin c in Coin.All)
                if (c != null && (c.transform.position - rb.position).sqrMagnitude < 32f)
                    c.transform.position = Vector3.MoveTowards(c.transform.position, rb.position, 8.7f * Time.fixedDeltaTime);
    }

    // Newest pickup wins: stop the running boost, start a fresh one.
    public void BoostSpeed(float multiplier, float seconds)
    {
        if (boost != null) StopCoroutine(boost);
        boost = StartCoroutine(Boost(multiplier, seconds));
    }

    IEnumerator Boost(float multiplier, float seconds)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(seconds);
        speedMultiplier = 1f;
        boost = null;
    }

    public void Magnet(float seconds) => magnetUntil = Time.time + seconds;

    // The player is deactivated while respawning, which stops coroutines: reset the boost.
    void OnDisable() { speedMultiplier = 1f; boost = null; magnetUntil = 0f; }
}
