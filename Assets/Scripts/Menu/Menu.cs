using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private InputActionReference menuAction;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Player_Movement player;
    [SerializeField] private Player_attack player_attack;
    [SerializeField] private Player_Xp xpSystem;
    [SerializeField] private WaveManager waveManager;

    [SerializeField] private TextMeshProUGUI waveStats;
    [SerializeField] private TextMeshProUGUI killStats;
    [SerializeField] private TextMeshProUGUI playerStats;

    private bool isOpen = false;

    private void Start()
    {
        if (menuPanel) menuPanel.SetActive(isOpen);

        if (player == null) player = FindAnyObjectByType<Player_Movement>();
        if (player_attack == null) player_attack = FindAnyObjectByType<Player_attack>();
        if (waveManager == null) waveManager = FindAnyObjectByType<WaveManager>();
        if (xpSystem == null) xpSystem = FindAnyObjectByType<Player_Xp>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (menuAction != null && menuAction.action.WasPerformedThisFrame() && player != null && !player.isDead)
        {
            isOpen = !isOpen;

            if (menuPanel) menuPanel.SetActive(isOpen);
            if (isOpen)
            {
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                InformationBoard();
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (isOpen) InformationBoard();
    }

    public void RestartGameButtom() 
    {
        Time.timeScale = 1f;
        isOpen = false;
        if (menuPanel) menuPanel.SetActive(isOpen);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void InformationBoard()
    {
        if (waveManager != null)
        {
            if (waveStats)
                waveStats.text =
                    $"Wave: {waveManager.CurrentWave}\n" +
                    $"Waves Completed: {waveManager.WavesCompleted}\n" +
                    $"Alive Enemies: {waveManager.AliveEnemies}";

            if (killStats)
                killStats.text =
                    $"Total Kills: {waveManager.TotalKills}";
        }
        else
        {
            if (waveStats) waveStats.text = "Wave: —\nWaves Completed: —\nAlive Enemies: —";
            if (killStats) killStats.text = "Total Kills: —\nBoss Kills: —";
        }

        if (playerStats)
        {
            float curHP = player != null ? player.health : 0f;
            float maxHP = player != null ? player.maxHealth : 0f;
            float moveSpeed = player != null ? player.moveSpeed : 0f;

            float dmg = player_attack != null ? player.damage : 0f;
            float atkPerSec = player_attack != null ? player_attack.shootInterval : 0f;

            playerStats.text =
                $"Health: {curHP:0}/{maxHP:0}\n" +
                $"Damage: {dmg:0}\n" +
                $"Attack Speed: {atkPerSec:0.##}/s\n" +
                $"Move Speed: {moveSpeed:0.##}\n" +
                $"XP Multiplier: x{xpSystem.XPMultiplier:0.00}";
        }
    }

    public void GoToLobbyButtom()
    {
        Time.timeScale = 1f;
        GameData.WavesCompleted = waveManager.WavesCompleted;
        SceneLoader.LoadScene("Menu");
    }
}
