using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;


public class TitleManager : MonoBehaviour {

	void Start () {

        this.UpdateAsObservable()
            .Select(_ => Input.GetButtonDown("Attack"))
            .Where(b => b)
            .Take(1)
            .Subscribe(_ => {
                Debug.Log("Game Start");
                SceneManager.LoadScene("stage1");
            });
        
	}
    
}
