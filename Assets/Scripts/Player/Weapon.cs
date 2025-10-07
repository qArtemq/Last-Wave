using UnityEngine;

public class Weapon : MonoBehaviour 
{ 
    [SerializeField] float speed = 12f; 
    [SerializeField] float lifeTime = 3f; 
    
    Vector2 _dir = Vector2.right; 
    int damage; 
    public void Init(Vector2 direction, int dmg) 
    { 
        damage = dmg; 
        _dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right; 
        
        float angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg; 
        transform.rotation = Quaternion.Euler(0, 0, angle); 
        
        Destroy(gameObject, lifeTime); 
    } 
    void FixedUpdate() 
    { 
        transform.position += (Vector3)(_dir * speed * Time.deltaTime); 
    } 
    
    private void OnTriggerEnter2D(Collider2D other) 
    { 
        if (!other.CompareTag("Enemy")) return; 
        
        var enemy = other.GetComponent<Enemy>(); 
        if (enemy != null) 
        { 
            enemy.TakeDamage(damage); 
            Destroy(gameObject); 
        } 
    } 
}