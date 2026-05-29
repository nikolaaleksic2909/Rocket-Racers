using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [Tooltip("Parent containing Node 0, Node 1, ... in order")]
    public Transform pathParent;

    [Tooltip("Meters per second—driven by CarController")]
    [HideInInspector] public float speed = 0f;

    [Tooltip("Lateral shift from the center nodes (half-lane width). Positive = right lane, negative = left.")]
    public float lateralOffset = 1.5f;

    List<Transform> nodes;
    int currentIndex = 0;

    void Start()
    {
        // gather nodes in order
        nodes = new List<Transform>();
        foreach (Transform t in pathParent) nodes.Add(t);
    }

    void Update()
    {
        if (nodes.Count == 0) return;

        // where on the path we’re headed
        Transform node = nodes[currentIndex];
        int nextIndex = (currentIndex + 1) % nodes.Count;
        Transform nextNode = nodes[nextIndex];

        // compute tangent & right-vector
        Vector3 tangent = (nextNode.position - node.position).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;

        // target position = center node + lateral shift
        Vector3 targetPos = node.position + right * lateralOffset;

        // move and rotate
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);

        // advance when close
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            currentIndex = nextIndex;
    }

    // called by CarController each frame
    public void SetSpeed(float s)
    {
        speed = s;
    }
}
