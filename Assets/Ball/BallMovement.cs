using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    public float speed = 1;
    private Vector2 direction;
    private float width;
    private bool destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        width = GetComponent<SpriteRenderer>().bounds.size.x;
        int Y = Random.Range(0,2) % 2;
        float[] vectorsY = { -1, 1 };
        int X = Random.Range(0, 2) % 2;
        float[] vectorsX = { -0.5f, 0.5f };
        direction = new Vector2(vectorsX[X], vectorsY[Y]);

        Manager.current.onBallPointTriggerEnterEnemy += OnBallPointDeleteEnemy;
        Manager.current.onBallPointTriggerEnterPlayer += OnBallPointDeletePlayer;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        direction = new Vector2(direction.x, -direction.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.position.x + width/2 > 2.8)
        {
            direction = new Vector2(-direction.x, direction.y);
        }
        if (rb.position.x - width / 2 < -2.8)
        {
            direction = new Vector2(-direction.x, direction.y);
        }
        if (rb.position.y > 6 && !destroyed)
        {
            destroyed = true;
            OnBallPointDeleteEnemy();
        }
        if (rb.position.y < -6 && !destroyed)
        {
            destroyed = true;
            OnBallPointDeletePlayer();
        }
        if ( !destroyed)
        {
            rb.position += direction * speed;
        }
    }

    private void OnBallPointDeletePlayer()
    {
        Manager.current.BallPointTriggerEnterPlayer();
        Destroy(this.gameObject);
    }
    private void OnBallPointDeleteEnemy()
    {
        Manager.current.BallPointTriggerEnterEnemy();
        Destroy(this.gameObject);
    }
}
