using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class JellyfishGenerator : EnemyControl {

		public GameObject prefab;
		public float interval = 3;
		
		new void Start () {
				base.Start();
				bool flip = GetComponent<SpriteRenderer>().flipX;
				Observable.Interval(TimeSpan.FromSeconds(interval))
					.Where(_ => !isDead)
					.Subscribe(_ => {
						var g = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
						if(flip) {
							g.GetComponent<JellyfishControl>().straightFlip = -1f;
						}
					})
					.AddTo(gameObject);
		}	

		protected override void OnDead() {
        
    }
	
}
