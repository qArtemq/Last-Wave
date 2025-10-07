using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class HealSpawner : MonoBehaviour
{
    [Header("Heal kit Prefab")]
    [SerializeField] GameObject healPrefab;

    [Header("Heal kit limit")]
    [SerializeField] int maxHeals = 5;

    [Header("Spawn Points")]
    public Transform spawnPoints;
    public float spawnRadius = 0.25f;

    [Header("Spawn Time")]
    [SerializeField] float minSpawnTime = 5f;
    [SerializeField] float maxSpawnTime = 15f;

    List<GameObject> spawnedHeals = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            CleanupList();
            if (spawnedHeals.Count >= maxHeals)
                continue;

            Vector2 pos = (Vector2)spawnPoints.position + Random.insideUnitCircle * spawnRadius;

            GameObject heal = Instantiate(healPrefab, pos, Quaternion.identity);
            spawnedHeals.Add(heal);
        }
    }
    void CleanupList()
    {
        spawnedHeals.RemoveAll(item => item == null);
    }
    void OnDrawGizmosSelected()
    {
       Gizmos.DrawWireSphere(spawnPoints.position, Mathf.Max(0.05f, spawnRadius));
    }
}
