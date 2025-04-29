using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float boundary = 7.5f;

    void Update()
    {
        float input = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(input, 0f, 0f) * moveSpeed * Time.deltaTime;

        transform.position += movement;

        float clampedX = Mathf.Clamp(transform.position.x, -boundary, boundary);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}
