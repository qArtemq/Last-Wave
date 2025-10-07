using TMPro;
using UnityEngine;

public class HealPopup : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Canvas canvas;
    [SerializeField] TMP_Text text;

    [Header("FX")]
    [SerializeField] float lifetime = 0.6f;
    [SerializeField] Vector2 movePerSec = new Vector2(0.0f, 1.8f);
    [SerializeField] float spawnSpread = 0.25f;
    [SerializeField] float startYOffset = 0.8f;

    float t;
    Color baseColor;
    Camera cam;

    public void Init(int amount, Vector3 worldPos, Camera worldCamera)
    {
        cam = worldCamera != null ? worldCamera : Camera.main;
        if (canvas) canvas.worldCamera = cam;

        Vector2 random = Random.insideUnitCircle * spawnSpread;
        transform.position = worldPos + Vector3.up * startYOffset + new Vector3(random.x, random.y, 0f);

        text.text = $"+{amount}";
        t = 0f;
    }

    void Update()
    {
        t += Time.deltaTime;

        transform.position += (Vector3)(movePerSec * Time.deltaTime);

        float a = 1f - (t / lifetime);
        text.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);

        if (t >= lifetime)
            Destroy(gameObject);
    }
}
