using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;


public class TitleManager : MonoBehaviour {

	void Start () {

        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Cancel"))
            .Subscribe(_ => Application.Quit());

        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Attack"))
            .Take(1)
            .Subscribe(_ => {
                Debug.Log("Game Start");
                SceneManager.LoadScene("stage1");
            });
        
	}
    
}
