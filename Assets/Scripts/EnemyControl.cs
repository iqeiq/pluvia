using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class EnemyControl : BehaviourUtil {

    [SerializeField]
    public int hp {get; protected set;} = 1;
    protected bool isDead = false;

    public BoxCollider2D trigger;


	public void Start () {
        this.OnTriggerEnter2DAsObservable()
            .Where(col => IsTag(col.gameObject, "Attack"))
            .Do(_ => --hp)
            .Where(_ => hp == 0)
            .Subscribe(_ => {
                isDead = true;
                OnDead();
                StartCoroutine(Die());
            });
	}

    protected virtual void OnDead() {

	}
	
	public IEnumerator Die () {
        if (trigger == null)
            trigger = GetComponent<BoxCollider2D>();
        
        trigger.enabled = false;

        var a = 1f;
        
		yield return this.UpdateAsObservable()
            .Select(_ => Time.deltaTime)
            .Do(t => a -= t)
			.TakeWhile(_ => a > 0f)
            .Do(_ => SetColA(a))
            .ToYieldInstruction()
            .AddTo(this);

		Destroy(gameObject);
    }
}
