using UnityEngine;

public enum PatrolMode { Loop, PingPong, Random }

public class EnemyPatrol : MonoBehaviour
{
    public Vector3[] points;
    public PatrolMode mode = PatrolMode.Loop;
    public float speed = 2.6f, waitTime = 0f, turnSpeed = 360f, sight = 0f;

    enum State { Patrol, Chase, Return }
    State state = State.Patrol;
    int index, step = 1; float waitUntil; Material mat;
    static readonly Color Calm = new Color(1f, 0.54f, 0.24f), Angry = new Color(1f, 0.2f, 0.2f);

    void Start() { mat = GetComponent<Renderer>().material; }

    void Update()
    {
        bool see = CanSee();
        switch (state)
        {
            case State.Patrol:
            {
                if (Time.time >= waitUntil)
                {
                    Vector3 target = points[index];
                    Move(target, speed);
                    if (transform.position == target) { index = NextIndex(); waitUntil = Time.time + waitTime; }
                }
                if (see) state = State.Chase;
                break;
            }
            case State.Chase:
            {
                Vector3 p = GameManager.Instance.PlayerT.position; p.y = transform.position.y;
                Move(p, speed * 1.7f);                       // faster than patrol, still slower than the player
                if (!see) state = State.Return;
                break;
            }
            default:
            {
                int b = Nearest(); Vector3 q = points[b];
                Move(q, speed);
                if (transform.position == q) { index = b; state = State.Patrol; }
                if (see) state = State.Chase;
                break;
            }
        }
        Color c = state == State.Chase ? Angry : Calm;
        if (mat.color != c) mat.color = c;
    }

    void Move(Vector3 target, float spd)
    {
        Vector3 to = target - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, spd * Time.deltaTime);
        to.y = 0f;
        if (to != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(to), turnSpeed * Time.deltaTime);
    }

    bool CanSee()
    {
        if (sight <= 0f || !GameManager.Instance.PlayerAlive) return false;
        Vector3 a = transform.position, p = GameManager.Instance.PlayerT.position; p.y = a.y;
        if ((p - a).magnitude > sight) return false;
        // walls block sight; triggers are ignored
        if (Physics.Linecast(a, p, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore))
            return hit.collider.CompareTag("Player");
        return true;
    }

    int NextIndex()
    {
        if (mode == PatrolMode.Loop) return (index + 1) % points.Length;
        if (mode == PatrolMode.PingPong)
        {
            int ahead = index + step;
            if (ahead < 0 || ahead >= points.Length) step = -step;
            return index + step;
        }
        int next = Random.Range(0, points.Length);
        return next == index ? (next + 1) % points.Length : next;
    }

    int Nearest()
    {
        int best = 0; float bd = float.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            float d = (points[i] - transform.position).sqrMagnitude;
            if (d < bd) { bd = d; best = i; }
        }
        return best;
    }
}
