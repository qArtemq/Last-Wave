using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TakeXp : MonoBehaviour
{
    [Serializable]
    public class ColorOption
    {
        public string name = "Low";
        public float weight = 1f;
        public int xpAmount = 10;
        public Sprite spriteOverride;
    }

    [Header("Stone Options")]
    public List<ColorOption> options = new List<ColorOption>
    {
        new ColorOption{ name="Low",  weight=60, xpAmount=10},
        new ColorOption{ name="Mid",   weight=30, xpAmount=25},
        new ColorOption{ name="High", weight=10, xpAmount=50}
    };

    [Header("Settings")]
    public string playerTag = "Player";
    public bool destroyOnPickup = true;

    private int _xpToGive;
    private SpriteRenderer _rend;

    void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();

        ApplyRandomAppearanceAndXP();

        var col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    private void ApplyRandomAppearanceAndXP()
    {
        if (options == null || options.Count == 0) return;

        float total = 0f;
        foreach (var o in options) total += Mathf.Max(0, o.weight);
        float r = Random.value * total;

        ColorOption picked = options[0];
        float cum = 0f;
        foreach (var o in options)
        {
            cum += Mathf.Max(0, o.weight);
            if (r <= cum) { picked = o; break; }
        }
        _rend.sprite = picked.spriteOverride;
        _xpToGive = picked.xpAmount;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        var xp = other.GetComponentInParent<Player_Xp>();
        if (xp != null)
        {
            xp.AddXP(_xpToGive);
        }

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}
