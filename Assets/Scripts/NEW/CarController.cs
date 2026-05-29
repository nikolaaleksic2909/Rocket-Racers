using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CarController : MonoBehaviour
{
    // once any car wins, stop all further win-checks
    public static bool GlobalRaceOver = false;

    [Header("Movement")]
    public float maxSpeed = 100f;
    public float accelRate = 30f;
    public float decelRate = 80f;

    [Header("Overheat Meter")]
    public float meterMax = 100f;
    public float meterUpRate = 15f;
    public float meterDownRate = 40f;
    [Range(0, 1)]
    public float recoveryThreshold = 0.5f;

    [Header("Waypoint & Rocket")]
    public WaypointFollower follower;
    public GameObject rocketPrefab;
    public Transform rocketSpawnPoint;

    [Header("UI (drag in)")]
    public TMP_Text speedText;
    public Slider meterSlider;
    public TMP_Text rocketsText;
    public TMP_Text lapText;
    public TMP_Text resultText;

    [Header("Laps")]
    public int maxLaps = 5;
    public float lapCooldown = 1f;

    [Header("AI Settings")]
    public bool isAI = false;
    public float aiMissileInterval = 10f;
    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty aiDifficulty = Difficulty.Medium;

    // internal state
    int missileCount = 0;
    int currentLap = 0;
    bool hasStartedLap;
    float lastLapTime = -Mathf.Infinity;

    float currentSpeed = 0f;
    float currentMeter = 0f;
    bool canAccel = true;

    float aiTimer = 0f;
    float fireTimer = 0f;
    const float fireCooldown = 1f;

    Image meterFill;

    void Start()
    {
        GlobalRaceOver = false;

        if (isAI)
        {
            aiMissileInterval = GameSettings.AIMissileInterval;
            recoveryThreshold = GameSettings.RecoveryThreshold;
            maxSpeed = GameSettings.AIMaxSpeed;
            meterDownRate = GameSettings.AIMeterDownRate;
        }

        if (follower == null)
            follower = GetComponent<WaypointFollower>();

        if (meterSlider != null && meterSlider.fillRect != null)
            meterFill = meterSlider.fillRect.GetComponent<Image>();

        if (meterSlider != null)
        {
            meterSlider.interactable = false;
            meterSlider.direction = Slider.Direction.BottomToTop;
            meterSlider.minValue = 0f;
            meterSlider.maxValue = 1f;
            meterSlider.value = 0f;
        }

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (GlobalRaceOver) return;

        // firing cooldown
        if (fireTimer > 0f) fireTimer -= Time.deltaTime;

        // AI missile gain
        if (isAI)
        {
            aiTimer += Time.deltaTime;
            if (aiTimer >= aiMissileInterval)
            {
                aiTimer = 0f;
                missileCount++;
            }
        }

        // decide whether to throttle
        bool wantsAccel = isAI
            ? (currentMeter < meterMax && canAccel)
            : (Input.GetKey(KeyCode.W) && canAccel);

        HeatAndCool(wantsAccel);

        if (follower != null)
            follower.SetSpeed(currentSpeed);

        if (!isAI) HandleRocketFire();

        if (isAI && missileCount > 0 && fireTimer <= 0f)
        {
            missileCount--;
            FireRocket();
            fireTimer = fireCooldown;
        }

        UpdateUI();
    }

    void HeatAndCool(bool accelerating)
    {
        float recoveryLevel = recoveryThreshold * meterMax;

        if (accelerating)
        {
            currentSpeed = Mathf.Min(currentSpeed + accelRate * Time.deltaTime, maxSpeed);
            currentMeter += meterUpRate * Time.deltaTime;
            if (currentMeter >= meterMax)
            {
                currentMeter = meterMax;
                canAccel = false;
            }
        }
        else
        {
            currentSpeed = Mathf.Max(currentSpeed - decelRate * Time.deltaTime, 0f);
            if (currentMeter > 0f)
            {
                currentMeter -= meterDownRate * Time.deltaTime;
                if (!canAccel && currentMeter <= recoveryLevel)
                    canAccel = true;
                currentMeter = Mathf.Max(currentMeter, 0f);
            }
        }
    }

    void HandleRocketFire()
    {
        if (Input.GetKeyDown(KeyCode.E) &&
            missileCount > 0 &&
            fireTimer <= 0f)
        {
            missileCount--;
            FireRocket();
            fireTimer = fireCooldown;
        }
    }

    void FireRocket()
    {
        if (rocketPrefab == null || rocketSpawnPoint == null) return;
        var go = Instantiate(rocketPrefab, rocketSpawnPoint.position, Quaternion.identity);
        var r = go.GetComponent<Rocket>();
        if (r != null)
        {
            foreach (var c in FindObjectsOfType<CarController>())
                if (c != this)
                {
                    r.Init(c.transform, this);
                    break;
                }
        }
    }

    void UpdateUI()
    {
        if (speedText != null) speedText.text = Mathf.RoundToInt(currentSpeed).ToString();
        if (meterSlider != null)
        {
            float n = currentMeter / meterMax;
            meterSlider.value = n;
            if (meterFill != null)
                meterFill.color = Color.Lerp(
                    new Color(0.4f, 0.8f, 1f, 1f), Color.red, n
                );
        }
        if (rocketsText != null) rocketsText.text = $"Rockets: {missileCount}";
        if (lapText != null) lapText.text = $"LAPS: {currentLap}/{maxLaps}";
    }

    void OnTriggerEnter(Collider other)
    {
        if (GlobalRaceOver) return;
        if (!other.CompareTag("Finish")) return;

        if (Time.time - lastLapTime < lapCooldown) return;
        lastLapTime = Time.time;

        if (!hasStartedLap)
        {
            hasStartedLap = true;
            Debug.Log($"{name} armed lap counter");
            return;
        }

        currentLap++;
        Debug.Log($"{name} crossed finish: lap {currentLap}/{maxLaps}");

        if (currentLap > maxLaps)
        {
            GlobalRaceOver = true;
            currentSpeed = 0f;
            canAccel = false;
            if (resultText != null)
            {
                resultText.text = isAI ? "AI WON" : "PLAYER WON";
                resultText.gameObject.SetActive(true);
            }
            StartCoroutine(EndGameThenReturnToMenu());
        }
    }

    IEnumerator EndGameThenReturnToMenu()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }

    public void AddMissile() => missileCount++;
    public void Stun(float s) => StartCoroutine(StunRoutine(s));

    IEnumerator StunRoutine(float secs)
    {
        float pre = currentSpeed;
        currentSpeed = 0f;
        canAccel = false;
        yield return new WaitForSeconds(secs);
        currentSpeed = pre * 0.5f;
        canAccel = true;
    }
}
