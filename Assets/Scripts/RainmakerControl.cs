using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class RainmakerControl : EnemyControl  {

    Rigidbody2D rig;
    [SerializeField]
    private GameObject parent;


    new void Start()
    {
        base.Start();

        rig = GetComponent<Rigidbody2D>();
        
        StartCoroutine(LateStart());

        this.OnCollisionEnter2DAsObservable()
            .Where(col => IsTag(col.gameObject, "Whale", "Rainmaker"))
            .Take(1)
            .Subscribe(_ => {
                StartCoroutine(Die());
            });

    }

    IEnumerator LateStart() {
        var range = FindComponent<RangeListener>("range");
        
        yield return new WaitForSeconds(0.2f);

        yield return range.Listen()
            .Where(col => IsTag(col.gameObject, "Player", "Enemy"))
            .Take(1)
            .Do(_ => Emit())
            .ToYieldInstruction()
            .AddTo(range);
        
    }

    public void Emit() {
        rig.gravityScale = 1.0f;
        if (parent != null) {
            parent.GetComponent<JellyfishControl>().Gone();
            transform.parent = parent.transform.parent;
        }
            
    }
}
