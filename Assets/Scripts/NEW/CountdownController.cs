using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownController : MonoBehaviour
{
    public TMP_Text countdownText;
    public float startDelay = 1f;  // time before 3 appears
    public float between = 1f;     // seconds per count

    void Start()
    {
        // freeze time
        Time.timeScale = 0f;
        StartCoroutine(DoCountdown());
    }

    IEnumerator DoCountdown()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(between);
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(between);

        countdownText.text = "";
        Time.timeScale = 1f;  // unfreeze
    }
}
