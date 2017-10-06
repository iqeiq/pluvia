using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;

public class GameManager : BehaviourUtil {

    void Start () {
        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Cancel"))
            .Subscribe(_ => Application.Quit());
	}

    // TODO: 雑
    public void UpdateHP(int hp) {
        var c = GameObject.Find("UI-HP").transform;
        while (true) {
            ++hp;
            var h = c.Find("HP-" + hp);
            if (h == null) break;
            h.GetComponent<Image>().color = new Color(0.1f, 0f, 0f, 0.2f);
        }
    }

    // TODO: 雑
    public void UpdateLycoris(int ly) {
        var c = GameObject.Find("UI-Lycoris").transform;
        while (true)
        {
            var h = c.Find("ly-" + ly);
            if (h == null) break;
            var i = h.GetComponent<Image>();
            var ic = i.color;
            ic.r = 0.5f;
            ic.g = 0.5f;
            i.color = ic;
            --ly;
        }
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
            .Where(_ => Input.GetButtonDown("Attack"))
            .Take(1)
            .Subscribe(_ => {
                Debug.Log("Game Clear");
                SceneManager.LoadScene("title");
            });
    }
}
