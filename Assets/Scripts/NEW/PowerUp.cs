using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Tooltip("Units/sec falling along -Z")]
    public float fallSpeed = 2f;
    [Tooltip("Deg/sec spin")]
    public float rotationSpeed = 90f;
    [Tooltip("Initial bubble size")]
    public Vector3 bubbleScale = Vector3.one;

    int spinDir;

    void Start()
    {
        // set size & random spin direction
        transform.localScale = bubbleScale;
        spinDir = (Random.value < 0.5f) ? 1 : -1;
    }

    void Update()
    {
        transform.Translate(Vector3.back * fallSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.up, spinDir * rotationSpeed * Time.deltaTime, Space.World);
    }

    void OnMouseDown()
    {
        // give human player one missile
        foreach (var car in FindObjectsOfType<CarController>())
            if (!car.isAI)
            {
                car.AddMissile();
                break;
            }

        Destroy(gameObject);
    }
}
