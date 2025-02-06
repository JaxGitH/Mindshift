using Unity.VisualScripting;
using UnityEngine;

public class B_SpawnRandom : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] spawnPrefabs; // Array of prefabs to spawn
    [SerializeField] private Transform spawnPoint; // Spawn location (defaults to spawner's position)

    private void Start()
    {
        if (spawnPoint == null)
            spawnPoint = transform; // Default to this GameObject's position
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure only the player triggers the spawn
        {
            SpawnRandomPrefab();
            Destroy(gameObject); // Destroy the spawner after triggering
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
