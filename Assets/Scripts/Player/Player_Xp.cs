using System;
using UnityEngine;

public class Player_Xp : MonoBehaviour
{
    [Header("Current values")]
    [SerializeField] private int level = 1;
    [SerializeField] private int currentXP = 0;
    [SerializeField] private int requiredXP = 0;

    [Header("Complexity Growth Parameters")]
    [Tooltip("The base of the complexity formula. The more, the more XP you need at the beginning.")]
    [SerializeField] private int baseXP = 100;
    [Tooltip("The exponent of growth. 1.0 = linear, 1.5–2.5 = exponential/more complex.")]
    [SerializeField] private float exponent = 1.5f;

    [Header("XP Gain Multiplier")]
    [Tooltip("How much faster XP is gained. Example: 1.5 = +50% faster XP.")]
    [SerializeField] private float xpMultiplier = 1.0f;

    [Tooltip("How much to increase the XP multiplier after each level up (optional).")]
    [SerializeField] private float multiplierPerLevel = 0.05f;

    public event Action<int, int, int> OnXPChanged;
    public event Action<int> OnLevelUp;

    public int Level => level;
    public int CurrentXP => currentXP;
    public int RequiredXP => requiredXP;
    public float XPMultiplier => xpMultiplier;

    private void Awake()
    {
        requiredXP = CalcRequiredXP(level);
        RaiseXPChanged();
    }

    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        int finalXP = Mathf.RoundToInt(amount * xpMultiplier);
        currentXP += finalXP;

        while (currentXP >= requiredXP)
        {
            currentXP -= requiredXP;
            level++;
            requiredXP = CalcRequiredXP(level);
            xpMultiplier += multiplierPerLevel;
            OnLevelUp?.Invoke(level);
        }

        RaiseXPChanged();
    }

    private int CalcRequiredXP(int lvl)
    {
        float v = baseXP * Mathf.Pow(lvl, exponent) * lvl;
        return Mathf.Max(1, Mathf.RoundToInt(v));
    }

    private void RaiseXPChanged()
    {
        OnXPChanged?.Invoke(currentXP, requiredXP, level);
    }
    public void BoostXPMultiplier(float amount)
    {
        xpMultiplier += amount;
        xpMultiplier = Mathf.Max(0.1f, xpMultiplier);
    }
}
