using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
　


public class EnemyControl : MonoBehaviour {

    public int hp = 1;

    
	public void Start () {
        var anim = GetComponent<Animator>();
        anim.speed = 0;

        this.OnTriggerEnter2DAsObservable()
            .Where(col => col.gameObject.tag.Equals("Attack"))
            .Do(_ => hp--)
            .Where(_ => hp == 0)
            .Subscribe(_ => {
                anim.speed = 1;
                StartCoroutine("Die");
            });
    
	}
	
	IEnumerator Die () {
        GetComponent<BoxCollider2D>().enabled = false;
        var sr = GetComponent<SpriteRenderer>();
        var a = 1.0f;
        while (a > 0.0f) {
            a -= Time.deltaTime;
            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, a);
            yield return new WaitForFixedUpdate();
        }
    }
}
