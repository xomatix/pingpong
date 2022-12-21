using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    public float speed;
    private float width; 
    private float posX;
    private Vector2 newPosition;

    // Update is called once per frame
    void FixedUpdate()
    {
        width = GetComponent<SpriteRenderer>().bounds.size.x;
        newPosition = rb.position + new Vector2(Input.GetAxis("Horizontal") * speed, 0);
        if (newPosition.x + width/2 < 2.8 && newPosition.x - width / 2 > -2.8)
        {
            rb.position = newPosition;
        }
        //Debug.Log(width);
    }
}
