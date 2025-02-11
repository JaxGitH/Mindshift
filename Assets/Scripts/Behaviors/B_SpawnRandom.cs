using UnityEngine;

public class B_SpawnRandom : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] spawnPrefabs;
    [SerializeField] private Transform spawnPoint;

    private bool hasSpawned = false; // Prevents double spawning

    private void Start()
    {
        if (spawnPoint == null)
            spawnPoint = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return; // Prevent multiple triggers

        if (other.CompareTag("Player"))
        {
            hasSpawned = true; // Mark as triggered
            SpawnRandomPrefab();
            Destroy(gameObject);
        }
    }

    private void SpawnRandomPrefab()
    {
        if (spawnPrefabs.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned to spawn!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPrefabs.Length);
        GameObject prefabToSpawn = spawnPrefabs[randomIndex];

        Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"Spawned {prefabToSpawn.name} at {spawnPoint.position}");
    }
}
