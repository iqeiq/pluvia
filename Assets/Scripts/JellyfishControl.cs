using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class JellyfishControl : EnemyControl {

    [SerializeField]
    public bool carrier = false;
    [SerializeField]
    public bool chase = true;
    [SerializeField]
    public bool shift = true;
    [SerializeField]
    public bool straight = false;
    public float straightFlip = 1f;

    public int vision = 5;
    public float speed = 0.2f;
    public float d = 3;

    private Animator anim;
    private SpriteRenderer sr;
    
    new void Start () {
        
        base.Start();

        anim = GetComponent<Animator>();
        anim.speed = 0;

        sr = GetComponent<SpriteRenderer>();
        var player = GameObject.Find("player");
        var t = 0f;
        
        this.UpdateAsObservable()
            .Where(_ => !straight && chase)
            .Where(_ => !isDead)
            .Select(_ => player.transform.position - transform.position)
            .Where(dir => (carrier ? Mathf.Abs(dir.x) : dir.magnitude) < vision)
            .Subscribe(dir => {
                var mv = (carrier ? Mathf.Sign(dir.x) * transform.right : dir.normalized) * (speed * Time.deltaTime);
                mv += (Mathf.Sin(t * Mathf.Deg2Rad) * 0.002f) * transform.up;
                transform.position += mv;
                t = (t + 2) % 360f;
                if(mv.magnitude > 0.05f)
                    sr.flipX = mv.x > 0;
            });
        
        if(straight) {
            // TODO; (-_-;)
            shift = false;
            chase = false;
            StartCoroutine(Move());
        }

	}

    protected override void OnDead() {
        anim.speed = 1;
        if (carrier) {
            var g = transform.Find("rainmaker");
            if(g != null)
                g.GetComponent<RainmakerControl>().Emit();
        }
    }

    public void Gone() {
        if (shift) {
            carrier = false;
        } else if (!straight) {
            StartCoroutine(Die());
        }
    }

    IEnumerator _move(Vector2 dir, float max) {
        float t = 0.0f;
        sr.flipX = dir.x > 0;
        
        while (t < 1.0f && !isDead) {
            var diff = speed * Time.deltaTime;
            t += diff;
            Vector3 temp = dir * (diff * max);
            transform.position += temp;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Move() {
        while (true) {
            yield return _move(new Vector2(-1 * straightFlip, 0), d);
            yield return new WaitForSeconds(1);
            yield return _move(new Vector2(1 * straightFlip, 0), d);
            yield return new WaitForSeconds(1);
        }
    }
   
}
