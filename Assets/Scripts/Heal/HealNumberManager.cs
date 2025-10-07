using UnityEngine;

public class HealNumberManager : MonoBehaviour
{
    public static HealNumberManager I;
    [SerializeField] HealPopup popupPrefab;
    Camera cam;

    void Awake()
    {
        I = this;
        cam = Camera.main;
    }

    public void SpawnHeal(int amount, Vector3 worldPos)
    {
        if (!popupPrefab || amount <= 0) return;
        var p = Instantiate(popupPrefab);
        p.Init(amount, worldPos, cam);
    }
}
