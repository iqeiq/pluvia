using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;


public class TitleManager : MonoBehaviour {

    public GameObject stage1;
    public GameObject stage2;
    
    public GameObject cursor;
    
    private int selected = 0;

	void Start () {

        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Cancel"))
            .Subscribe(_ => Application.Quit());

        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Attack"))
            .Take(1)
            .Subscribe(_ => {
                Debug.Log("Game Start");
                if(selected == 0)
                    SceneManager.LoadScene("sweet");
                else
                    SceneManager.LoadScene("bitter");
            });
        
        this.UpdateAsObservable()
            .Select(_ => Input.GetAxis("Select"))
            .Where(val => Mathf.Abs(val) > 0.01f)
            .ThrottleFirst(TimeSpan.FromMilliseconds(400f))
            .Select(val => (int)Mathf.Sign(val))
            .Subscribe(val => {
                selected += val;
                Debug.Log(selected + ", " + val);
                if(selected > 1) selected = 0;
                else if(selected < 0) selected = 1;
                var p = selected == 0 ? stage1.transform.position.y : stage2.transform.position.y;
                var cp = cursor.transform.position;
                cp.y = p;
                cursor.transform.position = cp;
            });
        
	}
    
}
