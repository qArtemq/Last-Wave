using UnityEngine;

public class Player_attack : MonoBehaviour
{
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] public float shootInterval = 0.25f;
    string enemyTag = "Enemy";
    Player_Movement player;
    float _nextShotTime = 0f;

    Camera cam;

    void Awake()
    {
        player = GetComponent<Player_Movement>();
        cam = Camera.main;
    }

    void Update()
    {
        if (!player.isDead)
            TryShoot();
    }

    void TryShoot()
    {
        if (Time.time < _nextShotTime) return;

        Transform enemy = FindClosestEnemy();
        if (enemy == null) return;

        Vector2 dir = (enemy.position - firePoint.position).normalized;

        Shoot(dir);

        _nextShotTime = Time.time + shootInterval;
    }
    void Shoot(Vector2 dir)
    {
        if (!weaponPrefab || !firePoint) return;

        GameObject go = Instantiate(weaponPrefab, firePoint.position, Quaternion.identity);
        var w = go.GetComponent<Weapon>();
        if (w != null)
            w.Init(dir, player.GetDamage());
    }
    Transform FindClosestEnemy()
    {
        if (cam == null) return null;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var e in enemies)
        {
            Vector3 viewportPos = cam.WorldToViewportPoint(e.transform.position);
            bool isVisible = viewportPos.z > 0 &&
                             viewportPos.x > 0 && viewportPos.x < 1 &&
                             viewportPos.y > 0 && viewportPos.y < 1;

            if (!isVisible) continue;

            float dist = Vector2.Distance(firePoint.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = e.transform;
            }
        }

        return closest;
    }
}
