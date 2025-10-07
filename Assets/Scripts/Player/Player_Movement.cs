using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player_Movement : MonoBehaviour
{
    Animator animator;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    public float moveSpeed = 5f;
    private int currentHash = 0;
    [SerializeField] InputActionReference move;

    [Header("UI")]
    [SerializeField] private HealthBarUI playerHealthBarPrefab;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0f, 1.2f, 0f);
    private HealthBarUI playerHealthBar;

    [Header("Health")]
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int regenerationHealth = 2;
    [SerializeField] public int health = 100;
    [SerializeField] public int damage = 20;
    [SerializeField] float hurtIFrames = 0.4f;
    [SerializeField] float hurtLock = 0.33f;

    public bool isDead = false;
    bool canBeHit = true;
    bool isHurting = false;
    [SerializeField] public bool isRegen = false;
    private Coroutine regenCoroutine;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        gameObject.tag = "Player";

        health = Mathf.Clamp(health, 0, maxHealth);

        if (playerHealthBarPrefab)
        {
            playerHealthBar = Instantiate(playerHealthBarPrefab, transform);
            playerHealthBar.transform.localPosition = healthBarOffset;

            UpdateHealthUI();
        }
    }

    void Update()
    {
        moveInput = move.action.ReadValue<Vector2>();
        if (!isHurting)
            CheckAnimaiton();

        if (isRegen && regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegerationHealth());
        }

        if (!isRegen && regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    IEnumerator HurtRoutine()
    {
        canBeHit = false;
        isHurting = true;

        ChangeAnimation("Player_Hurt");

        yield return new WaitForSeconds(hurtLock);

        isHurting = false;

        float rest = Mathf.Max(0f, hurtIFrames - hurtLock);
        if (rest > 0f) yield return new WaitForSeconds(rest);

        canBeHit = true;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || !canBeHit) return;

        health = Mathf.Max(0, health - damage);
        UpdateHealthUI();

        StartCoroutine(HurtRoutine());
        if (health <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;

        int before = health;
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        int gained = health - before;

        if (gained > 0)
        {
            Vector3 popupPos = transform.position;
            HealNumberManager.I?.SpawnHeal(gained, popupPos);
        }

        UpdateHealthUI();
    }


    IEnumerator RegerationHealth() 
    {
        while (isRegen) 
        {
            Heal(regenerationHealth);
            yield return new WaitForSeconds(5f);

            if (isDead) 
            {
                isRegen = false;
            }
        }
    }

    private void UpdateHealthUI()
    {
        if (playerHealthBar != null)
            playerHealthBar.SetHealth((float)health / maxHealth);
    }

    public void Die()
    {
        if (isDead) return;
        ChangeAnimation("Player_Death");
        rb.linearVelocity = Vector2.zero;
        isDead = true;
        this.enabled = false;
    }

    public int GetDamage()
    {
    return damage;
    }

    private void CheckAnimaiton() 
    {
        if (moveInput.x > 0.01f)
        {
            ChangeAnimation("Player_Run_R");
        }
        else if (moveInput.x < -0.01f)
        {
            ChangeAnimation("Player_Run_L");
        }
        else if (moveInput != Vector2.zero)
        {
            ChangeAnimation("Player_Run");
        }
        else
        {
            ChangeAnimation("Player_idle");
        }
    }
    public void ChangeAnimation(string animation)
    {
        int hash = Animator.StringToHash(animation);

        if (currentHash == hash) return;

        currentHash = hash;
        
        animator.Play(hash, 0, 0f);
    }
}
