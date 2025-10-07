using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Ability
{
    public string name;

    public enum AbilityType
    {
        IncreaseDamage,
        IncreaseMaxHealth,
        IncreaseRegeneration,
        MoveSpeedUp,
        AttackSpeedUp,
        XpMultiplier
    }

    public AbilityType type;
    public int valuePerPick = 1;
    [TextArea] public string description;
}

[Serializable]
public class Panel
{
    public string name;
    public GameObject panel;
    public Button[] buttons;
}

public class ChoiceSkill : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private Panel[] allPanels;

    [Header("Abilities Catalog")]
    [SerializeField] private Ability[] allAbilities;

    [Header("Caps / Limits")]
    [SerializeField] private int damageCap = 999;
    [SerializeField] private int maxHealthCap = 999;
    [SerializeField] private int regenerationCap = 100;
    [SerializeField] private float moveSpeedCap = 20f;
    [SerializeField] private float attackSpeedCap = 0.1f;
    [SerializeField] private float xpMultiplierCap = 5f;

    private Player_Movement player;
    private Player_attack playerAttack;
    private Player_Xp player_Xp;

    private System.Random rng = new System.Random();

    private void Start()
    {
        canvas.SetActive(false);
        foreach (var p in allPanels) p.panel.SetActive(false);

        player = FindAnyObjectByType<Player_Movement>();
        playerAttack = FindAnyObjectByType<Player_attack>();
        player_Xp = FindAnyObjectByType<Player_Xp>();
    }

    public void ShowMenu()
    {
        List<Ability> available = BuildAvailableAbilities();

        if (available.Count <= 0)
        {
            return;
        }

        Time.timeScale = 0f;
        canvas.SetActive(true);

        foreach (var p in allPanels) p.panel.SetActive(false);

        int countToShow = available.Count >= 3 ? 3 : available.Count;

        Panel target = GetPanelByButtonsCount(countToShow);
        if (target == null)
        {
            Debug.LogWarning("The panel with the required number of buttons was not found.");
            CloseMenu();
            return;
        }

        List<Ability> pick = PickRandom(available, countToShow);

        SetupButtons(target, pick);

        target.panel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseMenu()
    {
        foreach (var p in allPanels) p.panel.SetActive(false);
        canvas.SetActive(false);
        Time.timeScale = 1f;
    }


    private List<Ability> BuildAvailableAbilities()
    {
        var list = new List<Ability>();
        foreach (var ab in allAbilities)
        {
            if (IsAbilityAtCap(ab)) continue;
            list.Add(ab);
        }
        return list;
    }

    private bool IsAbilityAtCap(Ability ab)
    {
        switch (ab.type)
        {
            case Ability.AbilityType.IncreaseDamage:
                return player.damage >= damageCap;

            case Ability.AbilityType.IncreaseMaxHealth:
                return player.maxHealth >= maxHealthCap;

            case Ability.AbilityType.IncreaseRegeneration:
                return player.regenerationHealth >= regenerationCap;

            case Ability.AbilityType.MoveSpeedUp:
                return player.moveSpeed >= moveSpeedCap;

            case Ability.AbilityType.AttackSpeedUp:
                return playerAttack.shootInterval <= attackSpeedCap;

            case Ability.AbilityType.XpMultiplier:
                return player_Xp.XPMultiplier >= xpMultiplierCap;

            default:
                return true;
        }
    }

    private Panel GetPanelByButtonsCount(int count)
    {
        foreach (var p in allPanels)
        {
            if (p.buttons != null && p.buttons.Length == count)
                return p;
        }
        return null;
    }

    private List<Ability> PickRandom(List<Ability> input, int count)
    {
        var arr = new List<Ability>(input);
        for (int i = arr.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        if (arr.Count > count) arr.RemoveRange(count, arr.Count - count);
        return arr;
    }

    private void SetupButtons(Panel target, List<Ability> abilities)
    {
        for (int i = 0; i < target.buttons.Length; i++)
        {
            var btn = target.buttons[i];
            if (i >= abilities.Count)
            {
                btn.gameObject.SetActive(false);
                continue;
            }

            btn.gameObject.SetActive(true);

            Ability ab = abilities[i];

            var texts = btn.GetComponentsInChildren<TextMeshProUGUI>(true);
            TextMeshProUGUI title = null;
            TextMeshProUGUI desc = null;

            foreach (var t in texts)
            {
                if (t.name.Contains("Title", StringComparison.OrdinalIgnoreCase))
                    title = t;
                else if (t.name.Contains("Description", StringComparison.OrdinalIgnoreCase))
                    desc = t;
            }

            if (title != null)
                title.text = ab.name;

            if (desc != null)
                desc.text = ab.description;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                ApplyAbility(ab);
                CloseMenu();
            });
        }
    }


    private void ApplyAbility(Ability ab)
    {
        switch (ab.type)
        {
            case Ability.AbilityType.IncreaseDamage:
                player.damage = Mathf.Min(player.damage + ab.valuePerPick, damageCap);
                break;

            case Ability.AbilityType.IncreaseMaxHealth:
                player.maxHealth = Mathf.Min(player.maxHealth + ab.valuePerPick, maxHealthCap);
                break;

            case Ability.AbilityType.IncreaseRegeneration:
                player.regenerationHealth = Mathf.Min(player.regenerationHealth + ab.valuePerPick, regenerationCap);
                player.isRegen = true;
                break;

            case Ability.AbilityType.MoveSpeedUp:
                player.moveSpeed = Mathf.Min(player.moveSpeed + ab.valuePerPick * 0.1f, moveSpeedCap);
                break;

            case Ability.AbilityType.AttackSpeedUp:
                float newInterval = playerAttack.shootInterval - ab.valuePerPick * 0.01f;
                playerAttack.shootInterval = Mathf.Max(newInterval, attackSpeedCap);
                break;

            case Ability.AbilityType.XpMultiplier:
                float add = Mathf.Max(0f, ab.valuePerPick * 0.01f);
                float allowed = xpMultiplierCap - player_Xp.XPMultiplier;
                if (allowed <= 0f) break;
                float toAdd = Mathf.Min(add, allowed);
                player_Xp.BoostXPMultiplier(toAdd);
                break;
        }
    }
}
