using UnityEngine;

public class PingPongMovement : MonoBehaviour
{
    [SerializeField] private float speed = 250.0f;

    [SerializeField] private float maxDistance = 100.0f;

    private void Update()
    {
        float value = Mathf.PingPong(Time.time * speed, 2 * maxDistance) - maxDistance;
        transform.localPosition = new Vector3(value, transform.localPosition.y,
            transform.localPosition.z);
    }
}
