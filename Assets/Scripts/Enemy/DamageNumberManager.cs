using UnityEngine;

public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager I;
    [SerializeField] DamagePopup popupPrefab;
    Camera cam;

    void Awake()
    {
        I = this;
        cam = Camera.main;
    }

    public void SpawnDamage(int amount, Vector3 worldPos)
    {
        if (!popupPrefab) return;
        var p = Instantiate(popupPrefab);
        p.Init(amount, worldPos, cam);
    }
}
