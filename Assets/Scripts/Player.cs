using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{

    [Header("Controls")]
    public KeyCode Right = KeyCode.D;
    public KeyCode Left = KeyCode.A;
    public KeyCode Jump = KeyCode.W;
    public KeyCode Sprint = KeyCode.LeftShift;
    public KeyCode Attack = KeyCode.Mouse0;
    public KeyCode Shield = KeyCode.Mouse1;

    [Header("Player Mechs")]
    [Range(0f, 1000f)]
    public float WalkSpeed = 500f;
    [Range(0f, 3f)]
    public float SprintMulti = 1.25f;
    [Range(0f, 1f)]
    public float AirMulti = .75f;
    [Range(0f, 1200f)]
    public float JumpForce = 600f;
    [Range(0f, 5f)]
    public float VertOffest = .5f; // Vertical Offest
    [Range(0f, 15f)]
    public float m_Health = 3f;
    [Range(0f, 10f)]
    public float Damage = 1f;
    [Range(0f, 5f)]
    public float AttackTime = .2f;
    [Range(0f, 1f)]
    public float DamageReduce = .1f;
    [Range(0f, 1f)]
    public float MovePenalty = .1f; // Move slower when attacking
    public GameObject Ui;
    public Animator Anim;
    public GameObject Sword;
    public Camera cam;
    public bool mirror;

    //Private Variables
    private bool _onGround;
    private bool _canJump, _canWalk, _canDbljump;
    private float rot, _startScaleX, _startScaleY;
    private Rigidbody2D rig;
    private float c_Health;
    private float c_AttDelay;
    private bool _isShield;
    private bool _attacking;

    //Sets at the beginning
    void Start()
    {
        Sword.tag = "Item";
        rig = gameObject.GetComponent<Rigidbody2D>();
        _startScaleX = transform.localScale.x;
        _startScaleY = transform.localScale.y;
        c_Health = m_Health;
        c_AttDelay = 0;
    }

    void Update()
    {

        // Input Variables
        bool _xpos = Input.GetKey(Right);
        bool _xneg = Input.GetKey(Left);
        bool _ypos = Input.GetKeyDown(Jump);
        bool _sprint = Input.GetKey(Sprint);
        bool _attack = Input.GetKeyDown(Attack);
        bool _shield = Input.GetKey(Shield);
;
        //Checks if the Player is on the Ground
        if (_onGround)
        {
            Anim.SetBool("InAir", false);
            _canJump = true;
            _canWalk = true;
            _canDbljump = true;
        }

        //Horizontal Movement
        if (_xpos)
        {
            if (_sprint && _canWalk)
            {
                Anim.SetBool("Sprinting", true);
                moving(1f, SprintMulti);
            }
            else if (_canWalk)
            {
                Anim.SetBool("Sprinting", false);
                moving(1f, 1f);
            } else
            {
                Anim.SetBool("Sprinting", false);
                moving(1f, AirMulti);
            }

        }
        else if (_xneg)
        {
            if (_sprint && _canWalk)
            {
                Anim.SetBool("Sprinting", true);
                moving(-1f, SprintMulti);
            }
            else if (_canWalk)
            {
                Anim.SetBool("Sprinting", false);
                moving(-1f, 1f);
            } else
            {
                Anim.SetBool("Sprinting", false);
                moving(-1f, AirMulti);
            }
        }
        else
        {
            Anim.SetFloat("Speed", 0f);
            rig.velocity = new Vector2(0f, rig.velocity.y);
        }

        //Vertical Movement
        if (_ypos && (_canJump || _canDbljump))
        {
            Anim.SetBool("InAir", true);
            _canJump = false;
            _canWalk = false;
            jump();
        }

        if (_attack && c_AttDelay == 0 && !_isShield)
        {
            StartCoroutine(AttackDelay(AttackTime));
            c_AttDelay = AttackTime;
            Anim.SetBool("Swinging", true);
        }

        if (_shield)
        {
            _isShield = true;
            Anim.SetBool("Shielding", true);
        } else
        {
            _isShield = false;
            Anim.SetBool("Shielding", false);
        }
    }

    //Just helps with the sword position when or when not isMirror
    void FixedUpdate()
    {
        if (cam.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x + 0.2f)
        {
            transform.localScale = new Vector3(_startScaleX, _startScaleY, 1);
            mirror = false;
        }
        if (cam.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x - 0.2f)
        {
            transform.localScale = new Vector3(-_startScaleX, _startScaleY, 1);
            mirror = true;
        }
    }


    #region Helpers
    //Jump helper
    private void jump()
    {
        if (_canDbljump && _canJump)
        {
            _onGround = false;
            _canDbljump = true;
            rig.AddForce(new Vector2(0, JumpForce));
        }
        else if (_canDbljump && !_canJump)
        {
            rig.velocity = new Vector2(rig.velocity.x, 0f);
            _canDbljump = false;
            rig.AddForce(new Vector2(0, JumpForce));
        }
    }

    //Movement Helper with Direction(-1,1) for DIR and Sprint multiplier for MULTI
    private void moving(float dir, float multi)
    {
        float newx = (dir * WalkSpeed * Time.deltaTime);
        if (_attacking)
        {
            newx *= MovePenalty;
        }
        else
        {
            newx *= multi;
        }
        rig.velocity = new Vector2(newx, rig.velocity.y);
        Anim.SetFloat("Speed", Mathf.Abs(newx));
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") &&
            collision.GetContact(0).point.y <= rig.position.y - VertOffest)
        {
            _onGround = true;
        }
    }

    //The delay for the swinging attack animation
    IEnumerator AttackDelay(float time)
    {
        while (true)
        {
            _attacking = true;
            Sword.tag = "Sword";
            yield return new WaitForSeconds(time);
            break;
        }
        _attacking = false;
        c_AttDelay = 0;
        Sword.tag = "Item";
        Anim.SetBool("Swinging", false);
    }

    //Helps with collisions with enemies and damage caluclations with/without shield(mantle);
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            float contact_x = collision.GetContact(0).point.x - rig.position.x * transform.localScale.x;
            Debug.Log(contact_x);
            if (contact_x <= 0 || !_isShield)
            {
                decreaseHealth(collision.gameObject.GetComponent<EnemyScript>().Damage);
            } else
            {
                decreaseHealth(collision.gameObject.GetComponent<EnemyScript>().Damage * (1-DamageReduce));
            }
        }
    }

    //Helps see if the player left the ground
    private void OnCollisionExit2D(Collision2D collision)
    {
        _onGround = false;
    }

    //Checks if player is looking behind
    public bool isMirror()
    {
        return mirror;
    }

    //Function to decrease health and tell ui to update
    private void decreaseHealth(float amount)
    {
        c_Health -= amount;
        Debug.Log(c_Health);
        if (c_Health <= 0)
        {
            Ui.GetComponent<UI>().LoseHealth(0f);
            Debug.Log("GAMEOVER!!!!");
            Destroy(this.gameObject);
            // GAMEOVER STUFF;
        } else
        {
            Ui.GetComponent<UI>().LoseHealth(c_Health);
        }
    }
    #endregion
}