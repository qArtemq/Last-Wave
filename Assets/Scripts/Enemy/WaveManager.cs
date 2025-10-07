using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public int CurrentWave { get; private set; } = 0;
    public int WavesCompleted => Mathf.Max(0, CurrentWave - 1);
    public int AliveEnemies => aliveEnemies;
    public int TotalKills { get; private set; } = 0;

    public event Action<int, bool> OnWaveStarted;
    public event Action OnBossIncoming;

    [Header("All enemies for all waves")]
    public List<GameObject> allEnemies = new List<GameObject>();
    public List<GameObject> allBosses = new List<GameObject>();

    [Header("UI delays for banners")]
    [SerializeField] float uiIntroDelay = 1.2f;
    [SerializeField] float uiBossDelay = 1.2f;

    [Header("Wave (single settings)")]
    [SerializeField] int enemyCount = 5;
    [SerializeField] int extraEnemyCount = 5;
    [SerializeField] int enemyAddPerWave = 1;
    [SerializeField] int bossCount = 1;
    [SerializeField] float spawnInterval = 0.5f;
    [SerializeField] float extraSpawnInterval = 0.5f;
    [SerializeField] float delayAfterWave = 1.0f;

    [Header("The appearance of bosses")]
    [Min(1)] public int bossEvery = 5;
    public bool spawnBossAtStart = true;
    public bool bossAlsoWithEnemy = true;

    [Header("Increasing the difficulty OF KILLING")]
    public bool scaleOnKill = true;
    public bool scaleOnlyOnBossKill = true;

    [Header("Increasing difficulty")]
    [Range(0f, 500f)] public float enemyHealthPercentPerWave = 15f;
    [Range(0f, 500f)] public float enemyDamagePercentPerWave = 10f;
    [Range(0f, 500f)] public float bossHealthPercentPerWave = 20f;
    [Range(0f, 500f)] public float bossDamagePercentPerWave = 15f;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>();
    public float spawnRadius = 0.25f;

    [Header("Management")]
    public bool autoStart = true;
    public bool waitUntilAllDead = true;

    int aliveEnemies = 0;
    Coroutine routine;

    float enemyHealthMul = 1f;
    float enemyDamageMul = 1f;
    float bossHealthMul = 1f;
    float bossDamageMul = 1f;

    [SerializeField] Events eventsUI;

    void OnEnable() { Enemy.OnEnemyDied += OnEnemyDied; }
    void OnDisable() { Enemy.OnEnemyDied -= OnEnemyDied; }

    void Start()
    {
        if (autoStart) StartWaves();
    }

    public void StartWaves()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        if (allEnemies.Count == 0 && allBosses.Count == 0)
        {
            Debug.LogWarning("WaveManager: No enemies!");
            yield break;
        }

        CurrentWave = 0;

        while (true)
        {
            CurrentWave++;

            bool isBossWave = (bossEvery > 0) && (CurrentWave % bossEvery == 0);

            OnWaveStarted?.Invoke(CurrentWave, isBossWave);
            if (uiIntroDelay > 0f) yield return new WaitForSeconds(uiIntroDelay);

            if (isBossWave)
            {
                OnBossIncoming?.Invoke();
                eventsUI?.ShowBossIncoming();
                if (uiBossDelay > 0f) yield return new WaitForSeconds(uiBossDelay);
            }

            if (isBossWave && spawnBossAtStart)
            {
                for (int j = 0; j < Mathf.Max(1, bossCount); j++)
                {
                    SpawnRandomBoss();
                    yield return new WaitForSeconds(spawnInterval);
                }
            }

            if (!isBossWave || bossAlsoWithEnemy)
            {
                for (int j = 0; j < enemyCount; j++)
                {
                    SpawnRandomEnemy();
                    yield return new WaitForSeconds(spawnInterval);
                }

                if (waitUntilAllDead) while (AliveEnemies > 0) yield return null;

                for (int j = 0; j < extraEnemyCount; j++)
                {
                    SpawnRandomEnemy();
                    yield return new WaitForSeconds(extraSpawnInterval);
                }

                if (waitUntilAllDead) while (AliveEnemies > 0) yield return null;
            }

            if (isBossWave && !spawnBossAtStart)
            {
                for (int j = 0; j < Mathf.Max(1, bossCount); j++)
                {
                    SpawnRandomBoss();
                    yield return new WaitForSeconds(spawnInterval);
                }
                if (waitUntilAllDead) while (AliveEnemies > 0) yield return null;
            }
            if (delayAfterWave > 0) yield return new WaitForSeconds(delayAfterWave);

            float eH = 1f + enemyHealthPercentPerWave / 100f;
            float eD = 1f + enemyDamagePercentPerWave / 100f;
            enemyHealthMul *= eH;
            enemyDamageMul *= eD;

            float bH = (bossHealthPercentPerWave > 0f) ? (1f + bossHealthPercentPerWave / 100f) : eH;
            float bD = (bossDamagePercentPerWave > 0f) ? (1f + bossDamagePercentPerWave / 100f) : eD;
            bossHealthMul *= bH;
            bossDamageMul *= bD;

            enemyCount += enemyAddPerWave;
            if (CurrentWave % 3 == 0) extraEnemyCount += enemyAddPerWave;
        }
    }

    void SpawnRandomEnemy()
    {
        if (spawnPoints.Count == 0 || allEnemies.Count == 0) return;
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject prefab = allEnemies[Random.Range(0, allEnemies.Count)];
        Vector2 pos = (Vector2)point.position + Random.insideUnitCircle * spawnRadius;
        var go = Instantiate(prefab, pos, Quaternion.identity);

        var enemy = go.GetComponent<Enemy>();
        if (enemy != null) enemy.ApplyScaling(enemyHealthMul, enemyDamageMul);
        aliveEnemies++;
    }

    void SpawnRandomBoss()
    {
        if (spawnPoints.Count == 0 || allBosses.Count == 0) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject prefab = allBosses[Random.Range(0, allBosses.Count)];
        Vector2 pos = (Vector2)point.position + Random.insideUnitCircle * spawnRadius;

        var go = Instantiate(prefab, pos, Quaternion.identity);

        if (go.GetComponent<BossTag>() == null) go.AddComponent<BossTag>();

        string bossName = go.GetComponent<BossMeta>() ? go.GetComponent<BossMeta>().displayName : prefab.name;

        var enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.ApplyScaling(bossHealthMul, bossDamageMul);
            enemy.SetBossUI(eventsUI, bossName);
            eventsUI?.ShowBoss(bossName, 1f);
        }
        aliveEnemies++;
    }

    void OnEnemyDied(Enemy e)
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);

        bool isBoss = e != null && e.GetComponent<BossTag>() != null;
        TotalKills++;

        if (!scaleOnKill) return;
        if (scaleOnlyOnBossKill && !isBoss) return;

        enemyHealthMul *= (1f + enemyHealthPercentPerWave / 100f);
        enemyDamageMul *= (1f + enemyDamagePercentPerWave / 100f);

        float bH = (bossHealthPercentPerWave > 0f) ? (1f + bossHealthPercentPerWave / 100f) : (1f + enemyHealthPercentPerWave / 100f);
        float bD = (bossDamagePercentPerWave > 0f) ? (1f + bossDamagePercentPerWave / 100f) : (1f + enemyDamagePercentPerWave / 100f);
        bossHealthMul *= bH;
        bossDamageMul *= bD;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        foreach (var t in spawnPoints)
            if (t) Gizmos.DrawWireSphere(t.position, Mathf.Max(0.05f, spawnRadius));
    }
#endif

    public class BossTag : MonoBehaviour { }
    public class BossMeta : MonoBehaviour { public string displayName = "Boss"; }

}
