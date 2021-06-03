using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    const int playerSpeed = 7;
    const int playerJumpHeight = 5;
    public int score;

    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        SendInputToServer();
    //     if(Input.GetKey(KeyCode.D))
    //         rb.velocity = new Vector2(playerSpeed, rb.velocity.y);
    //     else if(Input.GetKey(KeyCode.A))
    //         rb.velocity = new Vector2(-playerSpeed, rb.velocity.y);
    //     SendLocationToServer();
    }

    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.Space),
            Input.GetKey(KeyCode.Mouse1),
            Input.GetKey(KeyCode.Mouse0),
            Input.GetKey(KeyCode.LeftShift)
        };

        ClientSend.PlayerMovement(_inputs);
    }

    // private void SendLocationToServer()
    // {
    //     ClientSend.PlayerMovement(transform.position);
    // }
}
