using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class RainmakerControl : EnemyControl  {

    new void Start()
    {
        base.Start();

        var rig = GetComponent<Rigidbody2D>();
        var eff = GetComponent<PlatformEffector2D>();
        var sr = GetComponent<SpriteRenderer>();

        var range = FindComponent<RangeListener>("range");
        
        range.Listen()
            .Where(col => IsTag(col.gameObject, "Player", "Enemy"))
            .Take(1)
            .Subscribe(_ => {
                rig.gravityScale = 1.0f;
            });

        this.OnCollisionEnter2DAsObservable()
            .Where(col => IsTag(col.gameObject, "Whale", "Rainmaker"))
            .Take(1)
            .Subscribe(_ => {
                StartCoroutine(Die());
            });

    }
}
