using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class StoneBounce : MonoBehaviour
{
    public static event System.Action<StoneBounce> onStoneDestroyed;

    [Header("Bounce Settings")]
    [Range(0f, 1f)] public float bounciness = 0.7f;
    public float minBounceUp = 1f;

    [Header("Skip Count (Random Range)")]
    public int minSkips = 1;
    public int maxSkips = 7;

    [Header("Splash & Sink")]
    public GameObject splashPrefab;
    public float sinkDelay = 1f;
    public float destroyDelay = 3f;

    Rigidbody rb;
    int skipCount;
    int sinkAfter;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sinkAfter = Random.Range(minSkips, maxSkips + 1);
    }

    void OnCollisionEnter(Collision col)
    {
        if (!col.collider.CompareTag("Water")) return;

        // bounce
        Vector3 inVel = rb.velocity;
        Vector3 outVel = Vector3.Reflect(inVel, Vector3.up) * bounciness;
        outVel.y = Mathf.Max(outVel.y, minBounceUp);
        rb.velocity = outVel;

        // splash
        if (splashPrefab)
            Instantiate(splashPrefab, col.contacts[0].point, Quaternion.identity);

        // count & sink
        if (++skipCount >= sinkAfter)
            StartCoroutine(SinkAndDestroy());
    }

    IEnumerator SinkAndDestroy()
    {
        yield return new WaitForSeconds(sinkDelay);

        // disable further collisions so it falls through
        GetComponent<Collider>().enabled = false;
        rb.useGravity = true;

        yield return new WaitForSeconds(destroyDelay);

        // notify spawner
        onStoneDestroyed?.Invoke(this);

        Destroy(gameObject);
    }
}