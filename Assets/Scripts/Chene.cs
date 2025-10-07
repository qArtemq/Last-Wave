using TMPro;
using UnityEngine;

public class Chene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveComplete;
    [SerializeField] private TextMeshProUGUI record;

    private void Awake()
    {
        int savedRecord = PlayerPrefs.GetInt("Record", 0);

        if (GameData.WavesCompleted > savedRecord)
        {
            savedRecord = GameData.WavesCompleted;
            PlayerPrefs.SetInt("Record", savedRecord);
            PlayerPrefs.Save();

            record.text = "New Record";
        }
        else
        {
            record.text = "Your Record";
        }
        GameData.WavesCompleted = savedRecord;
        waveComplete.text = $"Waves: {GameData.WavesCompleted}";
    }
    public void ButtomGame() 
    {
        SceneLoader.LoadScene("Game");
    }
    public void ButtomExit()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }
    public void ButtonResetRecord()
    {
        PlayerPrefs.DeleteKey("Record");
        PlayerPrefs.Save();
        record.text = "Record cleared!";
        waveComplete.text = "Waves: 0";
        GameData.WavesCompleted = 0;
    }
}
public static class GameData
{
    public static int WavesCompleted;
}

