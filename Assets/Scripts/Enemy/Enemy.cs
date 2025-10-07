using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform target;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 2.2f;
    [SerializeField] float stopDistance = 1.0f;

    [Header("Combat")]
    [SerializeField] int health = 100;
    [SerializeField] int damage = 5;
    [SerializeField] float contactDamageCooldown = 0.6f;
    [SerializeField] float attackRange = 1.15f;

    [Header("Drops")]
    [SerializeField] GameObject xpPref;
    [SerializeField] int bossRockCount = 10;
    [SerializeField] float rockSpawnRadius = 0.7f;

    [Header("Type")]
    [SerializeField] bool isBoss = false;

    [Header("UI BOSS")]
    Events eventsUI;
    bool bossUiBound;
    int maxHealthOnSpawn;
    string bossDisplayName;


    public static event Action<Enemy> OnEnemyDied;

    float _lastHitTime = -999f;
    Animator animator;
    bool isDead;

    NavMeshAgent agent;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Collider2D cols;

    string playerTag = "Player";

    int baseHealth;
    int baseDamage;

    public int Damage => damage;
    [SerializeField] float faceDeadZone = 0.08f;
    int lastFacing = 1;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        cols = GetComponent<Collider2D>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = false;
        agent.speed = moveSpeed;
        agent.stoppingDistance = stopDistance;

        animator.Play("Walk", 0, 0f);

        baseHealth = health;
        baseDamage = damage;

        TrySetTargetOnce();
    }

    void FixedUpdate()
    {
        if (isDead || !target) return;

        agent.SetDestination(target.position);

        if (!agent.pathPending && agent.remainingDistance <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            Vector2 to = ((Vector2)agent.nextPosition - rb.position).normalized;
            rb.linearVelocity = to * moveSpeed;
        }

        if (rb.linearVelocity.x > 0.01f) sr.flipX = true;
        else if (rb.linearVelocity.x < -0.01f) sr.flipX = false;
        else sr.flipX = (target.position.x > transform.position.x);
        UpdateFacing();
        TryContactHit();
    }

    void UpdateFacing()
    {
        if (!target) return;
        float dx = target.position.x - transform.position.x;

        if (dx > faceDeadZone) lastFacing = 1;
        else if (dx < -faceDeadZone) lastFacing = -1;

        sr.flipX = (lastFacing == 1);
    }

    public void ApplyScaling(float healthMultiplier, float damageMultiplier)
    {
        health = Mathf.Max(1, Mathf.CeilToInt(baseHealth * healthMultiplier));
        damage = Mathf.Max(1, Mathf.CeilToInt(baseDamage * damageMultiplier));
    }
    void TrySetTargetOnce()
    {
        if (target != null) return;

        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go != null) { target = go.transform; return; }
    }
    void TryContactHit()
    {
        if (!target) return;
        if (Time.time - _lastHitTime < contactDamageCooldown) return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= attackRange)
        {
            var player = target.GetComponent<Player_Movement>();
            if (player != null)
            {
                player.TakeDamage(damage);
                _lastHitTime = Time.time;
            }
        }
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (damage > 0)
            DamageNumberManager.I?.SpawnDamage(damage, transform.position);

        health = Mathf.Max(0, health - damage);

        if (isBoss && bossUiBound)
            eventsUI?.BossHP(health, Mathf.Max(1, maxHealthOnSpawn));

        if (health == 0 && !isDead)
            StartCoroutine(DieRoutine());
    }
    public void SetBossUI(Events ui, string name)
    {
        if (!isBoss) return;
        eventsUI = ui;
        bossDisplayName = name;
        maxHealthOnSpawn = health;
        bossUiBound = (eventsUI != null);
        eventsUI?.BossHP(health, maxHealthOnSpawn);
    }


    IEnumerator DieRoutine()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        agent.ResetPath();

        cols.enabled = false;

        animator.Play("Death", 0, 0f);

        yield return new WaitForSeconds(1.5f);

        OnEnemyDied?.Invoke(this);

        Vector2 pos = transform.position;

        if (xpPref) Instantiate(xpPref, pos, Quaternion.identity);

        if (isBoss)
        {
            for (int i = 0; i < bossRockCount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * rockSpawnRadius;
                Instantiate(xpPref, pos + offset, Quaternion.identity);
            }
        }
        if (isBoss && bossUiBound)
            eventsUI?.BossDead();
        Destroy(gameObject);
        enabled = false;
    }
} 
