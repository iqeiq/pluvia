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

    private bool canJump = false;
    

    void Start () {

        var rb = GetComponent<Rigidbody2D>();
        var anim = GetComponent<Animator>();
        var sr = GetComponent<SpriteRenderer>();

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
                    anim.speed = canJump ? 1 : 0;
                    sr.flipX = Math.Sign(val) < 0;
                }
                else {
                    anim.speed = 0;
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

        this.OnCollisionEnter2DAsObservable()
            .Select(col => col.gameObject)
            .Subscribe(obj => Debug.Log(obj.tag));


        this.UpdateAsObservable()
            .Select(_ => transform.position)
            .Where(p => p.y < -10)
            .Subscribe(_ => {
                Debug.Log("Dead");

                transform.position = GameObject.Find("start").transform.position;
                sr.flipX = false;
            });
    }
	
	void Update () {


	}

    void FixedUpdate()
    {
          

    }

}
