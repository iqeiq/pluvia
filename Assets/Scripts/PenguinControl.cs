using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class PenguinControl : MonoBehaviour {

    private Animator anim;
    private Transform hand;
    private Transform joint;
    [SerializeField]
    public bool flip = true;

    void Start () {
        anim = GetComponent<Animator>();
        joint = transform.Find("joint");
        hand = joint.Find("hand");
        anim.speed = 0;
        if (flip) joint.rotation = Quaternion.Euler(0, 180, 0);
        StartCoroutine(Attack());

    }

    IEnumerator Attack()
    {
        anim.speed = 1;
        while (true)
        {
            for(int i = 0; i < 12; ++i)
            {
                hand.transform.localRotation = Quaternion.Euler(0, 0, 360f - i * 360f / 12);
                yield return new WaitForSeconds(0.01f);
            }
           
        }
    }

}
