using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Enemy Variables")]
    [Range(0f, 1000f)]
    public float WalkSpeed = 100f;
    [Range(0f, 1000f)]
    public float JumpForce = 300f;
    [Range(0f, 20f)]
    public float WalkDuration = 2.5f;
    public float CurDirection = -1f; // 1 0r -1 for right and left movement;
    [Range(0f, 5f)]
    public float SpeedMulti = 1f;
    [Range(0f, 20f)]
    public float MinJumpTime = 3f;
    [Range(0f, 50f)]
    public float MaxJumpTime = 10f;
    [Range(0f, 5f)]
    public float Damage = 1f;
    [Range(0f, 5f)]
    public float Health = 1f;

    //Private Variables
    private Rigidbody2D rig;
    private float curDuration;
    private float y;
    private float z;
    private bool _isDead;
    private bool _onGround;


    // Start is called before the first frame update
    void Start()
    {
        rig = gameObject.GetComponent<Rigidbody2D>();
        curDuration = 0f;
        y = transform.localScale.y;
        z = transform.localScale.z;
        _isDead = false;
        StartCoroutine(jump());
    }

    // Update is called once per frame
    void Update()
    {
        if (curDuration <= 0)
        {
            CurDirection *= -1;
            curDuration = WalkDuration;
            StartCoroutine(walking(curDuration));
        }
        rig.velocity = new Vector2(CurDirection * WalkSpeed * SpeedMulti, rig.velocity.y);
    }

    //Helps Switch direction of enemy after t seconds
    IEnumerator walking(float t)
    {
        while (t > 0)
        {
            yield return new WaitForSeconds(t);
            curDuration = 0;
            t = 0;
            float dir = transform.localScale.x * -1;
            gameObject.transform.localScale = new Vector3(dir, y, z);
        }
    }

    //Helps randomly jump if on ground;
    IEnumerator jump()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinJumpTime, MaxJumpTime));
            if (_isDead)
            {
                break;
            }
            else if (_onGround)
            {
                Vector2 jumping = new Vector2(0f, JumpForce);
                rig.AddForce(jumping);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _onGround = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Sword"))
        {
            Debug.Log("OWWW");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            decreaseHealth(player.GetComponent<Player>().Damage);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Miss me?");
        if (collision.gameObject.CompareTag("Ground"))
        {
            _onGround = false;
        }
    }

    private void decreaseHealth(float amount)
    {
        Health -= amount;
        if (Health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}