using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;

public class GameManager : BehaviourUtil {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Clear() {
        Debug.Log("Clear");
        SceneManager.LoadScene("clear", LoadSceneMode.Additive);
        StartCoroutine(OnGameOver());
    }

    public void GameOver() {
        Debug.Log("GameOver");
        SceneManager.LoadScene("gameover", LoadSceneMode.Additive);
        StartCoroutine(OnGameOver());
    }

    
    IEnumerator OnGameOver() {
        var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        var c = camera.backgroundColor.r;
        var tt = 1f;
        yield return this.UpdateAsObservable()
            .Select(_ => Time.deltaTime)
            .Do(t => tt -= t)
            .TakeWhile(_ => tt > 0f)
            .Select(t => EaseInOut(tt) * c)
            .Do(t => {
                camera.backgroundColor = new Color(t, t, t, 1f);
            })
            .ToYieldInstruction()
            .AddTo(this);

        GameObject.Find("GuideText").GetComponent<Text>().enabled = true;
        this.UpdateAsObservable()
            .Select(_ => Input.GetButtonDown("Attack"))
            .Where(b => b)
            .Take(1)
            .Subscribe(_ => {
                Debug.Log("Game Clear");
                SceneManager.LoadScene("title");
            });
    }
}
