using UnityEngine;

public class MovingHazard : MonoBehaviour
{
    public Vector3 direction = Vector3.right;
    public float distance = 5f, speed = 3f;
    Vector3 start; Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); start = transform.position; }

    void FixedUpdate() =>
        rb.MovePosition(start + direction * Mathf.PingPong(Time.time * speed, distance));
}
