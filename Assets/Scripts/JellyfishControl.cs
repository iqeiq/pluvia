using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class JellyfishControl : EnemyControl {

	// Use this for initialization
	new void Start () {
        base.Start();

        var sr = GetComponent<SpriteRenderer>();
        var player = GameObject.Find("player");
        var speed = 0.2f;
        var t = 0f;
        
        this.UpdateAsObservable()
            .Select(_ => player.transform.position - transform.position)
            .Where(dir => dir.magnitude < 5)
            .Subscribe(dir => {
                var mv = dir.normalized * (speed * Time.deltaTime) + (Mathf.Sin(t * Mathf.Deg2Rad) * 0.002f) * transform.up;
                transform.position += mv;
                t = (t + 2) % 360f;
                sr.flipX = mv.x > 0;
            });


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
