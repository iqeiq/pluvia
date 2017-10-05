using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class RangeListener : MonoBehaviour {

    public IObservable<Collider2D> Listen() {
      return this.OnTriggerEnter2DAsObservable();
    }
}
