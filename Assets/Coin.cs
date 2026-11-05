using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static readonly List<Coin> All = new List<Coin>();
    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.AddScore(1);
        Destroy(gameObject);
    }
}
