using UnityEngine;

public class StoneSkippingSpawner : MonoBehaviour
{
    [Tooltip("Prefab with Rigidbody + Grab + StoneBounce")]
    public GameObject stonePrefab;
    [Tooltip("How many stones to have initially")]
    public int initialCount = 20;
    [Tooltip("Width (X) and Depth (Z) of the spawn rectangle")]
    public Vector2 spawnAreaSize = new Vector2(10f, 5f);

    void OnEnable()
    {
        StoneBounce.onStoneDestroyed += OnStoneDestroyed;
    }

    void OnDisable()
    {
        StoneBounce.onStoneDestroyed -= OnStoneDestroyed;
    }

    void Start()
    {
        for (int i = 0; i < initialCount; i++)
            SpawnOne();
    }

    void OnStoneDestroyed(StoneBounce deadStone)
    {
        // spawn a replacement
        SpawnOne();
    }

    void SpawnOne()
    {
        if (!stonePrefab)
        {
            return;
        }

        // random XZ offset
        Vector3 offset = new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            0f,
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f)
        );
        Vector3 pos = transform.position + offset;

        // random yaw rotation
        Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        Instantiate(stonePrefab, pos, rot);
    }
}