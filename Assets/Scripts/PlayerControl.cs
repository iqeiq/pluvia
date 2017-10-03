using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UniRx;
using UniRx.Triggers;
using Random = UnityEngine.Random;
　

public class PlayerControl : MonoBehaviour {

    public LayerMask groundLayer;
    public float jumpScale = 5.0f;
    public float moveScale = 6.0f;
    public float maxSpeed = 4.0f;
    public int hp = 5;

    private Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Transform hand;
    private Transform joint;
    private int inithp;
    private bool canJump = false;
    private bool canMove = false;
    private bool attack = false;
    private bool invincible = false;

    PostProcessingBehaviour ppb;
    PostProcessingProfile profile;


    void Init() {
        canJump = false;
        canMove = true;
        attack = false;
        hp = inithp;
        invincible = false;
        transform.position = GameObject.Find("start").transform.position;
        hand.gameObject.SetActive(false);
        Flip(false);
        GetComponent<BoxCollider2D>().enabled = true;
        sr.color = new Color(1, 1, 1, 1);
        rb.velocity = Vector2.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        ppb = GameObject.Find("Main Camera").GetComponent<PostProcessingBehaviour>();
        profile = Instantiate(ppb.profile);
        ppb.profile = profile;
    }

    void Reset()
    {
        // ステージのリセットをここに書くのはヤバイ
        GameObject.FindGameObjectsWithTag("Whale")
            .Select(g => g.GetComponent<WhaleControl>())
            .Where(w => w.isActiveAndEnabled)
            .ToList()
            .ForEach(w => w.Reset());
    }

    void Start () {

        inithp = hp;
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        joint = transform.Find("joint");
        hand = joint.Find("hand");
        
        Transform gcl = transform.Find("groundcheck_left");
        Transform gcr = transform.Find("groundcheck_right");

        Init();
        
        // ha?

        // move
        this.FixedUpdateAsObservable()
            .Where(_ => canMove)
            .Select(_ => Math.Sign(Input.GetAxis("Move")))
            .Subscribe(val => {
                if (Math.Abs(val) > 0.1f) {
                    rb.AddForce(transform.right * val * moveScale, ForceMode2D.Impulse);
                    if (Math.Abs(rb.velocity.x) > maxSpeed)
                    {
                        rb.velocity = new Vector2(maxSpeed * Math.Sign(rb.velocity.x), rb.velocity.y);
                    }
                    anim.speed = canJump || attack ? 1 : 0;
                    Flip(Math.Sign(val) < 0);
                }
                else {
                    if(!attack) anim.speed = 0;
                }

            });

        // jump
        this.FixedUpdateAsObservable()
            .Where(_ => canJump && canMove && Input.GetButton("Jump"))
            .Subscribe(_ => {
                canJump = false;
                rb.AddForce(transform.up * jumpScale, ForceMode2D.Impulse);
                //Debug.Log("jump");
            });

        // grounded?
        this.UpdateAsObservable()
            .Select(_ => 
                Physics2D.Linecast(transform.position, gcl.position, groundLayer) ||
                Physics2D.Linecast(transform.position, gcr.position, groundLayer)
            )
            .DistinctUntilChanged()
            .Subscribe(val => {
                canJump = val;
                if (val)
                {
                    rb.velocity = transform.right * rb.velocity.x;
                    Debug.Log("canJump");
                }
            });

        // attack
        this.UpdateAsObservable()
            .Where(_ => !attack && Input.GetButtonDown("Attack"))
            //.Do(_ => Debug.Log("Attack"))
            .Subscribe(_ => StartCoroutine("Attack"));

        /*this.OnCollisionEnter2DAsObservable()
            .Select(col => col.gameObject)
            .Subscribe(obj => Debug.Log(obj.tag));
        */

        // dead
        this.OnCollisionEnter2DAsObservable()
            .Select(col => col.gameObject.tag)
            .Where(tag => tag.Equals("Dead"))
            .Subscribe(_ => {
                Debug.Log("Dead");
                Reset();
                Init();
            });

        this.OnTriggerEnter2DAsObservable()
            .Merge(this.OnTriggerStay2DAsObservable())
            .Where(col => !invincible && col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            .Do(col => StartCoroutine(Damage()))
            .Where(_ => hp == 0)
            .Subscribe(_ => {
                StartCoroutine("Die");
            });
            

    }
    

    Vector2 _detectHitSide() {
        RaycastHit2D hit;
        if (hit = Physics2D.Raycast(transform.Find("raycheck").position, transform.up * -1.0f, Mathf.Infinity, 1 << LayerMask.NameToLayer("EnemyHit"))) {
            Debug.Log(hit.normal);
            return hit.normal;
        }
        // 要るのか？？
        if (hit = Physics2D.Raycast(transform.Find("raycheck").position, transform.right * -1.0f, Mathf.Infinity, 1 << LayerMask.NameToLayer("EnemyHit")))
        {
            Debug.Log(hit.normal);
            return hit.normal;
        }
        return Vector2.right;
    }

    IEnumerator Damage() {
        invincible = true;
        canMove = false;
        hp--;
        Debug.Log("hp: " + hp);
        var c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, 0.7f);
        var norm = _detectHitSide();
        rb.velocity = Vector2.Scale(Vector2.Reflect(rb.velocity, norm), new Vector2(1.0f, 1.1f));
        if (rb.velocity.y < 0.001f)
            rb.velocity += new Vector2(0.0f, 1.0f) * 3.0f;

        var frame = 0;
        // grain周り移動したい
        var grain = profile.grain.settings;
        while (frame < 90)
        {
            if (frame == 30) canMove = true;
            sr.color = new Color(c.r, Random.Range(0.5f, c.g), Random.Range(0.5f, c.b), Random.Range(0.0f, 1.0f));
            if (frame <= 30) {
                var t = (15 - Math.Abs(frame - 15)) / 15.0f;
                grain.intensity = 0.5f + (t * t) * (3f - (2f * t)) * 0.5f;
                profile.grain.settings = grain;
            }
            yield return new WaitForFixedUpdate();
            ++frame;
        }
        grain.intensity = 0.0f;
        profile.grain.settings = grain;

        sr.color = new Color(c.r, c.g, c.b, 1.0f);
        invincible = false;

        yield return new WaitForSeconds(0.5f);
        canMove = true;
        yield return new WaitForSeconds(1.5f);
        sr.color = new Color(c.r, c.g, c.b, 1.0f);
        
    }

 
    void Flip(bool left) {
        sr.flipX = left;
        joint.rotation = Quaternion.Euler(0, left ? 180 : 0, 0);
    }

    IEnumerator Die() {
        Debug.Log("Die");
        canMove = false;
        GetComponent<BoxCollider2D>().enabled = false;
        var a = 1.0f;
        while (a > 0.0f)
        {
            a -= 0.5f * Time.deltaTime;
            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, a);
            transform.rotation = Quaternion.Euler(0, 0, (1.0f - a) * 90);
            yield return new WaitForFixedUpdate();
        }
        Reset();
        Init();
        Debug.Log("Dead");
    }

    float _currentAnimatorFrame() {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    IEnumerator Attack() {
        attack = true;
        
        anim.SetInteger("attack", 1);
        // for sync walk motion
        anim.Play("player3", 0, _currentAnimatorFrame());
        hand.gameObject.SetActive(true);
        hand.transform.localRotation = Quaternion.Euler(0, 0, 60);
        yield return new WaitForSeconds(0.15f);
       

        anim.SetInteger("attack", 2);
        // for sync walk motion
        anim.Play("player2", 0, _currentAnimatorFrame());
        hand.transform.localRotation = Quaternion.Euler(0, 0, 0);
        yield return new WaitForSeconds(0.1f);
        
        anim.SetInteger("attack", 0);
        // for sync walk motion
        anim.Play("player", 0, _currentAnimatorFrame());
        hand.gameObject.SetActive(false);
        attack = false;
        
    }

}
