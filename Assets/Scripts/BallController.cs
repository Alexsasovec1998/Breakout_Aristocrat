using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float launchSpeed = 6f;
    private Rigidbody2D rigidBodyBall;
    private bool activeBall = false;
    private bool justBounced = false;

    public Transform paddle; 
    public bool isFirstBall = false; 

    // Start is called before the first frame update
    void Start()
    {
        rigidBodyBall = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!activeBall)
        {
            transform.position = paddle.position + new Vector3(0f, 0.3f, 0f);

            if (isFirstBall && Input.GetKeyDown(KeyCode.Space))
            {
                activeBall = true;
                rigidBodyBall.velocity = Vector2.up * launchSpeed;
            }
            else if (!isFirstBall)
            {
                activeBall = true;
                rigidBodyBall.velocity = Vector2.up * launchSpeed;
            }
        }

        if (transform.position.y < -6f)
        {
            if (isFirstBall)
            {
                GameController.Instance.RespawnBall();
            }

            Destroy(gameObject);
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick") && !justBounced)
        {
            BrickController brick = collision.gameObject.GetComponent<BrickController>();
            if (brick != null)
            {
                ScoreManager.instance.AddScore(brick.points);
            }
            brick.DestroySelf();

            justBounced = true;
        }
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            Vector3 paddlePos = collision.transform.position;
            float hitPoint = transform.position.x - paddlePos.x;
            float paddleWidth = collision.collider.bounds.size.x / 2f;

            float normalizedHitPoint = hitPoint / paddleWidth;

            Vector2 newDirection = new Vector2(normalizedHitPoint, 1f).normalized;

            rigidBodyBall.velocity = newDirection * launchSpeed;
        }
    }

    private void LateUpdate()
    {
        justBounced = false;
    }

    public void SetLaunchSpeed(float speed)
    {
        launchSpeed = speed;
    }
}
