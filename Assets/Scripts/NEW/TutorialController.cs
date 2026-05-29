using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public void OnContinue()
    {
        SceneManager.LoadScene("GameScene");
    }
}
