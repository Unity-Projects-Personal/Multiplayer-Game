using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public int id;
    public string username;

    int team;
    public float time; // time for sending packet

    public PlayerController controller;
    public Respawn respawn;
    private float moveSpeed = 80f;
    public float jumpSpeed = 10f;

    private bool[] inputs;


    private void Start()
    {
        controller.playerUsername = username;
        //gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        inputs = new bool[6];
    }

    public void FixedUpdate()
    {
        int _inputDirection = 0;
        bool jump = false;
        bool dash = false;
        bool cancelDash = false;
        bool upperCut = false;
        bool slam = false;
        bool ultimate = false;
        if (inputs[0])
        {
            _inputDirection += 1;
        }
        if(inputs[1])
        {
            _inputDirection = -1;
        }
        if(inputs[2])
        {
            jump = true;
        }
        if(inputs[3])
        {
            dash = true;
        }
        if(inputs[4])
        {
            cancelDash = true;
        }
        if(inputs[5])
        {
            upperCut = true;
        }
        // if(inputs[6])
        // {
        //     slam = true;
        // }
        // if(inputs[7])
        // {
        //     ultimate = true;
        // }
        Move(_inputDirection, jump, dash, upperCut, slam, ultimate, cancelDash);
    }
    private void Move(int _inputDirection, bool _jump, bool _dash, bool _upperCut, bool _slam, bool _ultimate, bool _cancelDash)
    {
#region Ground Detection
        if(controller.isGrounded(_jump) && _jump)
        {
            controller.Jump(jumpSpeed);
        }
#endregion

#region General Movement
        controller.Move(_inputDirection, moveSpeed);
#endregion

#region Special Abilities
        controller.Dash(_inputDirection, _dash, _cancelDash);
        controller.Uppercut(_upperCut);
#endregion
        ServerSend.PlayerPosition(this);

        //ServerSend.PlayerRotation(this); Dont really want to use this again so yea
    }

    public void SetInput(bool[] _inputs, float _time)
    {
        inputs = _inputs;
        time = _time;
    }
    public void PlayerTeam(int _team, Player _player)
    {
        if(_team == team)
        {
            Physics2D.IgnoreCollision(_player.gameObject.GetComponent<Collider2D>(), _player.GetComponent<Collider2D>());
        }
    }
    public void SetTeam(int _team)
    {
        team = _team;
    }
    public int GetTeam()
    {
        return team;
    }

}
