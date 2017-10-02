using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
　

public class WhaleControl : MonoBehaviour {

    public int type = 0;
    public float d = 10.0f;
    public float speed = 1.0f;
    public bool moveOnStay = false;

    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private bool stay;
    private Vector2 initPosition;
    private Quaternion initRotation;
    private IEnumerator c;


    void Start () {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        initPosition = transform.position;
        initRotation = transform.rotation;
        

        switch (type)
        {
            case 0: StartCoroutine(c = Move()); break;
            case 1: StartCoroutine(c = Move1()); break;
            case 2: StartCoroutine(c = Move2()); break;
            default: StartCoroutine(c = Move()); break;
        }
    }

    public void Reset()
    {
        transform.position = initPosition;
        transform.rotation = initRotation;
        if(c != null)
        {
            StopCoroutine(c);
            StartCoroutine(c);
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Player")) stay = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Player")) stay = false;
    }

    IEnumerator _move(Vector2 dir, float max) {
        float t = 0.0f;
        float prev = 0.0f;

        sp.flipX = dir.x > 0;

        while (t < 1.0f)
        {
            if(!moveOnStay || stay)
            {
                float diff = speed * Time.deltaTime;
                t += diff;
                float val = (t * t) * (3f - (2f * t)) * max;
                rb.MovePosition(rb.position + dir * (val - prev));
                prev = val;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Move() {
        while(true) {
            yield return _move(new Vector2(1, 0), d);
            yield return new WaitForSeconds(1);
            yield return _move(new Vector2(0, 1), d);
            yield return new WaitForSeconds(1);
            yield return _move(new Vector2(-1, 0), d);
            yield return new WaitForSeconds(1);
            yield return _move(new Vector2(0, -1), d);
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator Move1()
    {
        while (true)
        {
            yield return _move(new Vector2(1, 0), d);
            yield return new WaitForSeconds(1);
            yield return _move(new Vector2(-1, 0), d);
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator Move2()
    {
        while (true)
        {
            yield return _move(new Vector2(0, 1), d);
            yield return new WaitForSeconds(1);
            yield return _move(new Vector2(0, -1), d);
            yield return new WaitForSeconds(1);
        }
    }

}
