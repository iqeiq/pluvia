using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class WhaleControl : MonoBehaviour {

    public float d = 10.0f;
    public float speed = 1.0f;

    private Rigidbody2D rb;


    void Start () {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine("Move");
    }

    IEnumerator _move(Vector2 dir, float max) {
        float current = 0.0f;
        while (current < max)
        {
            float diff = speed * Time.deltaTime;
            current += diff;
            rb.MovePosition(rb.position + dir * diff);
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
	
	   
}
