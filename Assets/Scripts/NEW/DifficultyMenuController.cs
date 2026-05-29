using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyMenuController : MonoBehaviour
{
    public void OnEasy()
    {
        GameSettings.SelectedDifficulty = GameSettings.Difficulty.Easy;
        GameSettings.AIMissileInterval = 12f;
        GameSettings.RecoveryThreshold = 0.1f;
        GameSettings.AIMaxSpeed = 80f;
        GameSettings.AIMeterDownRate = 25f;
        SceneManager.LoadScene("TutorialScene");
    }

    public void OnMedium()
    {
        GameSettings.SelectedDifficulty = GameSettings.Difficulty.Medium;
        GameSettings.AIMissileInterval = 10f;
        GameSettings.RecoveryThreshold = 0.2f;
        GameSettings.AIMaxSpeed = 90f;
        GameSettings.AIMeterDownRate = 35f;
        SceneManager.LoadScene("TutorialScene");
    }

    public void OnHard()
    {
        GameSettings.SelectedDifficulty = GameSettings.Difficulty.Hard;
        GameSettings.AIMissileInterval = 5f;
        GameSettings.RecoveryThreshold = 0.4f;
        GameSettings.AIMaxSpeed = 100f;
        GameSettings.AIMeterDownRate = 45f;
        SceneManager.LoadScene("TutorialScene");
    }
}
