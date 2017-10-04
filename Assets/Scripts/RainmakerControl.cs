using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class RainmakerControl : MonoBehaviour {

    public int hp = 1;

    void Start()
    {
        
        var rig = GetComponent<Rigidbody2D>();
        var eff = GetComponent<PlatformEffector2D>();
        var sr = GetComponent<SpriteRenderer>();

        var range = transform.Find("range").GetComponent<RangeListener>();

        range.OnTriggerEnter2DAsObservable()
           .Where(col => col.gameObject.tag.Equals("Player") || col.gameObject.tag.Equals("Enemy"))
           .First()
           .Subscribe(_ => {
               rig.gravityScale = 1.0f;
               Debug.Log("fall");
           })
           .AddTo(gameObject);

        this.OnTriggerEnter2DAsObservable()
            .Where(col => col.gameObject.tag.Equals("Attack"))
            .Do(_ => hp--)
            .Where(_ => hp == 0)
            .Subscribe(_ => {
                StartCoroutine("Die");
            });

        this.OnCollisionEnter2DAsObservable()
            .Where(col => col.gameObject.tag.Equals("Whale") || col.gameObject.tag.Equals("Rainmaker"))
            .First()
            .Subscribe(_ => {
                gameObject.layer = LayerMask.NameToLayer("Player");
                //eff.colliderMask += (1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("Enemy"));
                Debug.Log("fix");
                var c = sr.color;
                c.a = 0.5f;
                sr.color = c;
            });

    }

    IEnumerator Die()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        var sr = GetComponent<SpriteRenderer>();
        var a = 1.0f;
        while (a > 0.0f)
        {
            a -= Time.deltaTime;
            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, a);
            yield return new WaitForFixedUpdate();
        }
    }
}
