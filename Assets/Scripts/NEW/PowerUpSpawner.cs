// PowerUpSpawner.cs
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Tooltip("Prefab with BubblePowerUp.cs + trigger collider")]
    public GameObject powerUpPrefab;

    [Tooltip("Seconds between spawns")]
    public float spawnInterval = 5f;

    [Tooltip("World X min/max for random spawn across the track width")]
    public float xMin = -10f, xMax = 10f;

    [Tooltip("World Y height above the road")]
    public float spawnY = 1f;

    [Tooltip("World Z position at which power-ups appear")]
    public float spawnZ = 50f;

    float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;

        // Random X, fixed Y and Z
        float x = Random.Range(xMin, xMax);
        Vector3 pos = new Vector3(x, spawnY, spawnZ);

        Instantiate(powerUpPrefab, pos, Quaternion.identity);
    }
}
