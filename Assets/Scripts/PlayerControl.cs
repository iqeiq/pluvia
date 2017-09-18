using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class PlayerControl : MonoBehaviour {

    public LayerMask groundLayer;
    public float jumpScale = 5.0f;
    public float moveScale = 6.0f;
    public float maxSpeed = 4.0f;

    private Animator anim;
    private SpriteRenderer sr;
    private Transform hand;
    private Transform joint;
    private SpriteRenderer srh;
    private bool canJump = false;
    private bool attack = false;


    void Start () {

        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        joint = transform.Find("joint");
        hand = joint.Find("hand");
        hand.gameObject.SetActive(false);
        srh = hand.Find("umbrella").GetComponent<SpriteRenderer>();

        var rb = GetComponent<Rigidbody2D>();
        
        Transform gcl = transform.Find("groundcheck_left");
        Transform gcr = transform.Find("groundcheck_right");


        // move
        this.FixedUpdateAsObservable()
            .Select(_ => Math.Sign(Input.GetAxis("Move")))
            .Subscribe(val => {
                if (Math.Abs(val) > 0.1f) {
                    rb.AddForce(transform.right * val * moveScale, ForceMode2D.Impulse);
                    if (Math.Abs(rb.velocity.x) > maxSpeed)
                    {
                        rb.velocity = new Vector2(maxSpeed * Math.Sign(rb.velocity.x), rb.velocity.y);
                    }
                    anim.speed = canJump || attack ? 1 : 0;
                    Flip(Math.Sign(val) < 0);
                }
                else {
                    if(!attack) anim.speed = 0;
                }

            });

        // jump
        this.FixedUpdateAsObservable()
            .Where(_ => canJump && Input.GetButton("Jump"))
            .Subscribe(_ => {
                canJump = false;
                rb.AddForce(transform.up * jumpScale, ForceMode2D.Impulse);
                Debug.Log("jump");
            });

        // grounded?
        this.UpdateAsObservable()
            .Select(_ => 
                Physics2D.Linecast(transform.position, gcl.position, groundLayer) ||
                Physics2D.Linecast(transform.position, gcr.position, groundLayer)
            )
            .DistinctUntilChanged()
            .Subscribe(val => {
                canJump = val;
                if (val)
                {
                    rb.velocity = transform.right * rb.velocity.x;
                    Debug.Log("canJump");
                }
            });

        // attack
        this.UpdateAsObservable()
            .Where(_ => !attack && Input.GetButton("Attack"))
            .Do(_ => Debug.Log("Attack"))
            .Subscribe(_ => StartCoroutine("Attack"));

        this.OnCollisionEnter2DAsObservable()
            .Select(col => col.gameObject)
            .Subscribe(obj => Debug.Log(obj.tag));


        // dead
        this.OnCollisionEnter2DAsObservable()
            .Select(col => col.gameObject.tag)
            .Where(tag => tag.Equals("Dead"))
            .Subscribe(_ => {
                Debug.Log("Dead");

                transform.position = GameObject.Find("start").transform.position;
                Flip(false);
            });
    }

    void Flip(bool left) {
        sr.flipX = left;
        //srh.flipX = left;
        // TODO:
        joint.rotation = Quaternion.Euler(0, left ? 180 : 0, 0);
    }

    float _currentAnimatorFrame() {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    IEnumerator Attack() {
        attack = true;
        
        anim.SetInteger("attack", 1);
        // for sync walk motion
        anim.Play("player3", 0, _currentAnimatorFrame());
        hand.gameObject.SetActive(true);
        hand.transform.localRotation = Quaternion.Euler(0, 0, 60);
        yield return new WaitForSeconds(0.2f);
       

        anim.SetInteger("attack", 2);
        // for sync walk motion
        anim.Play("player2", 0, _currentAnimatorFrame());
        hand.transform.localRotation = Quaternion.Euler(0, 0, 0);
        yield return new WaitForSeconds(0.2f);
        
        anim.SetInteger("attack", 0);
        // for sync walk motion
        anim.Play("player", 0, _currentAnimatorFrame());
        hand.gameObject.SetActive(false);
        attack = false;
        
    }

}
