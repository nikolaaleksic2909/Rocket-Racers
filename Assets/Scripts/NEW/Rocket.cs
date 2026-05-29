// Rocket.cs
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Tooltip("Units/sec")]
    public float speed = 20f;
    [Tooltip("Euler offset to make your model point forward")]
    public Vector3 rotationOffset = new Vector3(90f, 0f, 0f);

    Transform target;
    CarController shooter;

    public void Init(Transform targetTransform, CarController shooterController)
    {
        target = targetTransform;
        shooter = shooterController;
        gameObject.layer = LayerMask.NameToLayer("Rocket");
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(rotationOffset);
        transform.position += dir * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // ignore collisions with other rockets
        if (other.gameObject.layer == LayerMask.NameToLayer("Rocket"))
            return;

        var cc = other.GetComponent<CarController>();
        if (cc != null && cc != shooter)
        {
            cc.Stun(1f);
            Destroy(gameObject);
        }
    }
}
