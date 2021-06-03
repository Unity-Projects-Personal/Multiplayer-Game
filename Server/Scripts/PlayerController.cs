using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    int inputXNonZero; // made for setting the way you are facing
    bool grounded;
    public LayerMask ground;
    float radius = 0.05f;
    Rigidbody2D rb;
    public string playerUsername;
    public int myId;

    public int animId; // num of the animIdation

    #region General Movement
    float airAccel = 1f;
    float acceleration = 2f;
    float inputX;
    float moveSpeed;

    float fallMult = 2.5f;
    float lowJumpMult = 2;
    bool jumping = false;
    #endregion

    #region Dashing
    float mindashLength = 8;
    float maxDashLength = 25;
    float dashChargeTime = 0.85f;
    float dashChargeTimer = 0.0f;
    float dashingForce = 2f;
    bool dashing;
    float dashingHitForceX;

    int lastHitById = -1;

    #endregion

    #region Uppercut

    float uppercutJumpForce = 15; //uppercutJumpForce * 15
    float uppercutHitForceY = 700;
    bool isUppercutting = false;
    #endregion

    #region Special Checks
    public LayerMask playerLayerMask;
    bool isDashing; // in the process of dashing
    float specialCollisionCheckTime = 0.3f; // checks for collision for only 0.1 seconds
    float specialCollisionCheckTimer;
    bool isStunned;
    float stunnedTime = 0.4f;
    float stunnedTimer = 0;


    float coolDownDashTime = 1;
    float coolDownDashTimer = 0.0f;
    bool coolingDownDash;
    float coolDownUppercutTime = 0.75f;
    float coolDownUppercutTimer = 0.0f;
    bool coolingDownUppercut;
    #endregion


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myId = GetComponent<Player>().id;
    }

    public bool isGrounded(bool __jump)
    {
        jumping = __jump;
        grounded = false;
        grounded = Physics2D.CircleCast(new Vector2(transform.position.x + (transform.localScale.x / 2), transform.position.y - (transform.localScale.y / 2)), radius, Vector2.down, radius * 2, ground); // checks 0.5,-0.5, 0 for collision
        if (grounded)
            return true;
        else
        {
            grounded = Physics2D.CircleCast(new Vector2(transform.position.x - (transform.localScale.x / 2), transform.position.y - (transform.localScale.y / 2)), radius, Vector2.down, radius * 2, ground); // checks -0.5,-0.5, 0 for collision
            return grounded;
        }
    }

    public void Move(int moveDirection, float _moveSpeed)
    {
        if (!isStunned)
        {
            moveSpeed = _moveSpeed;
            inputX = moveDirection;
            inputXNonZero = moveDirection != 0 ? moveDirection : (inputXNonZero != 0 ? inputXNonZero : 1);
            if (!isDashing && !isUppercutting && !jumping && inputX != 0)
                SendAnimationAndSet(1);
        }
    }

    public void Jump(float _jumpForce)
    {
        if (!isStunned)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpForce * 23);
        }
    }

    #region Special Abailaties
    public void Dash(int _facing, bool _dashing, bool _cancelDash)
    {
        if (!isStunned && !coolingDownDash)
        {
            if (_cancelDash == true && dashing == true && _dashing == true)
            {
                dashChargeTimer = 0.0f;
                dashing = false;
                return;
            }
            // player stoped charging and didnt click cancel dash
            if (_dashing == false && dashing == true && _cancelDash == false)
            {
                coolingDownDash = true;
                isDashing = true;
                print("dashing");
                SendAnimationAndSet(5);
                rb.velocity = new Vector2(transform.localScale.x * dashingForce * 2 * (mindashLength + (maxDashLength * dashChargeTimer)), 0);
                dashChargeTimer = 0.0f;
                dashing = false;
                return;
            }
            if (dashChargeTimer >= dashChargeTime && dashing == true && _cancelDash == false)
            {
                coolingDownDash = true;
                isDashing = true;
                print("dashing");
                SendAnimationAndSet(5);
                rb.velocity = new Vector2(transform.localScale.x * dashingForce * 2 * (mindashLength + (maxDashLength * dashChargeTimer)), 0);
                dashChargeTimer = 0.0f;
                dashing = false;
                return;
            }
            dashing = _dashing;
        }
    }

    public void Uppercut(bool _isUppercutting)
    {
        if (_isUppercutting && !isStunned && !coolingDownUppercut)
        {
            isUppercutting = _isUppercutting;
            coolingDownUppercut = true;
            print("Uppercutting");
            SendAnimationAndSet(6);
            rb.velocity = new Vector2(rb.velocity.x, uppercutJumpForce);
        }
    }

    public void AddKnockBack(float _forceX, float _forceY)
    {
        rb.AddForce(new Vector2(inputX * _forceX, _forceY / 8));
    }

    public void Stun()
    {
        isStunned = true;
    }

    public void SpecialCollisionCheck(bool _isDashing, bool _isUppercutting)
    {
        RaycastHit2D[] hit2DLeft = Physics2D.CircleCastAll(new Vector2(gameObject.transform.position.x - (transform.localScale.x / 2), gameObject.transform.position.y), 0.5f, Vector2.left, playerLayerMask);
        RaycastHit2D[] hit2DRight = Physics2D.CircleCastAll(new Vector2(gameObject.transform.position.x + (transform.localScale.x / 2), gameObject.transform.position.y), 0.5f, Vector2.right, playerLayerMask);

        if (isUppercutting)
            SendAnimationAndSet(6);
        else if (isDashing)
            SendAnimationAndSet(5);

        if (hit2DLeft != null)
        {
            for (int i = 0; i < hit2DLeft.Length; i++)
            {
                if (hit2DLeft[i].collider.gameObject.GetComponent<PlayerController>() != null)
                {
                    PlayerController enemyPlayer = hit2DLeft[i].collider.gameObject.GetComponent<PlayerController>();
                    if (enemyPlayer.playerUsername != playerUsername)
                    {
                        if (_isDashing)
                        {
                            enemyPlayer.AddKnockBack(dashingHitForceX, 5);
                            enemyPlayer.SetHitBy(myId);
                        }
                        else if (_isUppercutting)
                        {
                            if (Vector3.Distance(enemyPlayer.transform.position, transform.position) < 3.3f)
                            {
                                enemyPlayer.AddKnockBack(25, uppercutHitForceY);
                                enemyPlayer.SetHitBy(myId);
                            }
                        }
                        enemyPlayer.Stun();
                        print(enemyPlayer.playerUsername + " Is now stunned and now knocked back");
                    }
                }
            }
        }
        if (hit2DRight != null)
        {
            for (int i = 0; i < hit2DRight.Length; i++)
            {
                if (hit2DRight[i].collider.gameObject.GetComponent<PlayerController>() != null)
                {
                    PlayerController enemyPlayer = hit2DRight[i].collider.gameObject.GetComponent<PlayerController>();
                    if (enemyPlayer.playerUsername != playerUsername)
                    {
                        if (_isDashing)
                        {
                            enemyPlayer.AddKnockBack(dashingHitForceX, 5);
                            enemyPlayer.SetHitBy(myId);
                        }
                        else if (_isUppercutting)
                        {
                            if (Vector3.Distance(enemyPlayer.transform.position, transform.position) < 3.3f)
                            {
                                enemyPlayer.AddKnockBack(25, uppercutHitForceY);
                                enemyPlayer.SetHitBy(myId);
                            }
                        }
                        enemyPlayer.Stun();
                        print(enemyPlayer.playerUsername + " Is now stunned and now knocked back");
                    }
                }
            }
        }

        return;
    }
    public void SetHitBy(int _id)
    {
        lastHitById = _id;
    }
    public int GetHitBy()
    {
        return lastHitById;
    }
    #endregion
    float lastY;// see if y pos has changed
    void FixedUpdate()
    {
        #region Better Fall

        if (rb.velocity.y < 0 && lastY != transform.position.y && !isUppercutting && !dashing)
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMult - 1) * Time.deltaTime;
            SendAnimationAndSet(3);
        }
        else if (rb.velocity.y > 0 && !jumping && lastY != transform.position.y && !isUppercutting && !dashing)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMult - 1) * Time.deltaTime;
            SendAnimationAndSet(3);
        }
        else if (rb.velocity.y < 0 && lastY != transform.position.y && !isUppercutting)
        {
            rb.velocity += Vector2.up * Physics.gravity.y / 2 * (fallMult - 1) * Time.deltaTime;
            SendAnimationAndSet(3);
        }
        else if (rb.velocity.y > 0 && !jumping && lastY != transform.position.y && !isUppercutting)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y / 2 * (lowJumpMult - 1) * Time.deltaTime;
            SendAnimationAndSet(3);
        }
        lastY = transform.position.y;
        #endregion
        if (!isStunned && !dashing)
            rb.AddForce(new Vector2(((inputX * moveSpeed * 15) - rb.velocity.x) * (grounded ? acceleration : airAccel), 0));
        else if (dashing && !isStunned)
            rb.AddForce(new Vector2(((inputX * moveSpeed / 2 * 7) - rb.velocity.x) * (grounded ? acceleration : airAccel), 0));

        #region facing
        transform.localScale = new Vector2(inputXNonZero, 1);
        #endregion
        if (inputX == 0 && rb.velocity.y == 0 && !isUppercutting && !isDashing && !jumping && !dashing)
            SendAnimationAndSet(0);
        if (rb.velocity.y > 0 && !isUppercutting && !grounded)
        {
            SendAnimationAndSet(2);
        }

        #region Specials
        if (dashing)
        {
            SendAnimationAndSet(4);
            dashChargeTimer += 1 * Time.deltaTime;
        }
        if (isStunned)
        {
            stunnedTimer += 1 * Time.deltaTime;
            if (stunnedTimer >= stunnedTime)
            {
                isStunned = false;
                stunnedTimer = 0;
            }
        }
        if (isDashing || isUppercutting)
        {
            SpecialCollisionCheck(isDashing, isUppercutting);
            specialCollisionCheckTimer += 1 * Time.deltaTime;
            if (specialCollisionCheckTimer >= specialCollisionCheckTime)
            {
                isDashing = false;
                isUppercutting = false;
                specialCollisionCheckTimer = 0;
            }
        }

        if (coolingDownDash)
        {
            isDashing = false;
            coolDownDashTimer += Time.deltaTime;
            if (coolDownDashTimer >= coolDownDashTime)
            {
                coolDownDashTimer = 0;
                coolingDownDash = false;
            }
        }

        if (coolingDownUppercut)
        {
            isUppercutting = false;
            coolDownUppercutTimer += Time.deltaTime;
            if (coolDownUppercutTimer >= coolDownDashTime)
            {
                coolDownUppercutTimer = 0;
                coolingDownUppercut = false;
            }
        }
        SendAnimationAndSet(animId);

        #endregion
    }
    int previousAnimationId = 7;
    float previousAnimationTimeId = 0;
    public void SendAnimationAndSet(int _animId)
    {
        if (previousAnimationId != _animId)
        {
            if (previousAnimationTimeId + 0.25f <= Time.time)
            {
                previousAnimationTimeId = Time.time;
                animId = _animId;
                ServerSend.PlayerAnimation(gameObject.GetComponent<Player>(), animId);
                previousAnimationId = _animId;
            }
        }
    }
}