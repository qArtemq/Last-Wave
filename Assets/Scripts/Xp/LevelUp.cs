using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    [Header("Links to UI")]
    [SerializeField] private Image imageFilled;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Data source")]
    [SerializeField] private Player_Xp playerXp;

    [Header("Skill menu")]
    [SerializeField] private ChoiceSkill choiceSkill;

    private void OnEnable()
    {
        if (!playerXp) playerXp = FindAnyObjectByType<Player_Xp>();
        if (!choiceSkill) choiceSkill = FindAnyObjectByType<ChoiceSkill>();

        if (playerXp != null)
        {
            playerXp.OnXPChanged += HandleXPChanged;
            playerXp.OnLevelUp += HandleLevelUp;

            HandleXPChanged(playerXp.CurrentXP, playerXp.RequiredXP, playerXp.Level);
        }
    }

    private void OnDisable()
    {
        if (playerXp != null)
        {
            playerXp.OnXPChanged -= HandleXPChanged;
            playerXp.OnLevelUp -= HandleLevelUp;
        }
    }

    private void HandleXPChanged(int current, int required, int level)
    {
        float normalized = required <= 0 ? 0f : (float)current / required;
        if (imageFilled) imageFilled.fillAmount = Mathf.Clamp01(normalized);

        if (levelText) levelText.text = $"{level} Level";
    }

    private void HandleLevelUp(int newLevel)
    {
        Debug.Log($"Level Up! New level: {newLevel}");
        choiceSkill.ShowMenu();
    }
}
