using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class LycorisControl :　BehaviourUtil {

	void Start () {
        this.OnTriggerEnter2DAsObservable()
            .Where(col => IsTag(col.gameObject, "Player"))
            .Subscribe(_ => {
                StartCoroutine(Die());
            });
    }

    public IEnumerator Die() {
        GetComponent<BoxCollider2D>().enabled = false;
        var sr = GetComponent<SpriteRenderer>();
        var halo = transform.Find("halo").GetComponent<Behaviour>();
        sr.sortingOrder = 5;

        var a = 1f;
        yield return this.UpdateAsObservable()
            .Select(_ => Time.deltaTime)
            .Do(t => a -= t * 0.75f)
            .TakeWhile(_ => a > 0f)
            .Select(_ => 1f - EaseOut(1f - a))
            .Do(t => {
                if(t < 0.75f) halo.enabled = false;
                SetColA(t);
                transform.position += transform.up * (1 - t * t) * 0.015f;
            })
            .ToYieldInstruction()
            .AddTo(this);

        Destroy(gameObject);
    }

}
