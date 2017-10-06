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

    public int vision = 5;
    public float speed = 0.2f;

    private Animator anim;
    
    new void Start () {
        
        base.Start();

        anim = GetComponent<Animator>();
        anim.speed = 0;

        var sr = GetComponent<SpriteRenderer>();
        var player = GameObject.Find("player");
        var t = 0f;
        
        this.UpdateAsObservable()
            .Where(_ => chase)
            .Select(_ => player.transform.position - transform.position)
            .Where(dir => (carrier ? Mathf.Abs(dir.x) : dir.magnitude) < vision)
            .Subscribe(dir => {
                var mv = (carrier ? Mathf.Sign(dir.x) * transform.right : dir.normalized) * (speed * Time.deltaTime);
                mv += (Mathf.Sin(t * Mathf.Deg2Rad) * 0.002f) * transform.up;
                transform.position += mv;
                t = (t + 2) % 360f;
                sr.flipX = mv.x > 0;
            });
	}

    protected override void OnDead() {
        anim.speed = 1;
        if (carrier) {
            var g = transform.Find("rainmaker");
            if(g != null)
                g.GetComponent<RainmakerControl>().Emit();
        }
    }

    public void Gone()
    {
        if (shift)
        {
            carrier = false;
        }
        else if (!straight)
        {
            StartCoroutine(Die());
        }
    }

   
}
