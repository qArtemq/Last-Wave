using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image imageFilled;
    [SerializeField] private bool hideWhenFull = false;

    public void SetHealth(float normalized)
    {
        normalized = Mathf.Clamp01(normalized);
        if (imageFilled) imageFilled.fillAmount = normalized;
        if (hideWhenFull) imageFilled.transform.parent.gameObject.SetActive(normalized < 0.999f);
    }
}
