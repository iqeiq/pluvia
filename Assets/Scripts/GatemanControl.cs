using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class GatemanControl : BehaviourUtil {

		public int th = 0;
		public Transform message;
		private bool closed = true;

		void Start () {
				var range = FindComponent<RangeListener>("range");
        
      	range.Listen()
          	.Where(col => IsTag(col.gameObject, "Player") && closed)
						.Select(col => col.gameObject.GetComponent<PlayerControl>())
          	.Subscribe(p => {
								if(p.lycoris >= th) {
										Debug.Log("unlock!");
										StartCoroutine(Die());
								} else {
										Debug.Log("locked!");
										StartCoroutine(Dialog());
								}
						});	
		}

		public IEnumerator Dialog() {
				closed = false;
				var t = message.Find("Text").GetComponent<Text>();
				var im = message.Find("Image").GetComponent<Image>();
				t.enabled = true;
				im.enabled = true;
				t.text = "x" + th;
				message.position = transform.position + transform.up * 1.2f;
				yield return new WaitForSeconds(3f);
				t.enabled = false;
				im.enabled = false;
				closed = true;
		}

		public IEnumerator Die () {
    		GetComponent<BoxCollider2D>().enabled = false;

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
