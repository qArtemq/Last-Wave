using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Player_Movement player;

    private void Awake()
    {
        if (endPanel) endPanel.SetActive(false);
    }

    private void Start()
    {
        if (player == null)
            player = FindAnyObjectByType<Player_Movement>();
    }

    private void Update()
    {
        if (player != null && player.isDead)
        {
            ShowGameOver();
        }
    }

    private void ShowGameOver()
    {
        if (endPanel) endPanel.SetActive(true);

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartGameButton()
    {
        Time.timeScale = 1f;
        if (endPanel) endPanel.SetActive(false);
        player.isDead = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
