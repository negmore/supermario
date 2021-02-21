using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public enum STATE
    {
        STATE_START = 0,
        STATE_TIMEUP,
        STATE_GAMEOVER,
    }

    public GameObject background;

    public Sprite[] numbers;

    public Image[] scores;
    public Image[] coins;
    public Image[] lifes;

    public STATE state;

    private int score;
    private int coin;
    private int life;

    private float curTime = 0f;
    private float aniTime = 0f;

    private int aniIndex = 0;

    // Start is called before the first frame update
    void Awake()
    {
        score = PlayerPrefs.GetInt("score", 0);
        coin = PlayerPrefs.GetInt("coin", 0);
        life = PlayerPrefs.GetInt("life", 0);

        SetNumberImage(score, ref scores);
        SetNumberImage(coin, ref coins);
        SetNumberImage(life, ref lifes);

        if (state == STATE.STATE_GAMEOVER)
        {
            int hiScore = PlayerPrefs.GetInt("hiscore", 0);

            if (hiScore < score)
            {
                PlayerPrefs.SetInt("hiscore", score);
                PlayerPrefs.Save();
            }
        }
    }

    private void Update()
    {
        curTime += Time.deltaTime;

        if (3f < curTime)
        {
            if (state == STATE.STATE_START)
                SceneManager.LoadScene("level1");
            else if (state == STATE.STATE_TIMEUP)
                SceneManager.LoadScene("intro");
            else if (state == STATE.STATE_GAMEOVER && 13f < curTime)
            {
                SceneManager.LoadScene("Title");
            }

        }
    }

    void SetNumberImage(int Number, ref Image[] sprites)
    {
        if (null == sprites[0] || Number < 0)
            return;

        for (int i = 0; i < sprites.Length; ++i)
        {
            int index = Number % 10;
            sprites[i].sprite = numbers[index];

            Number /= 10;
        }
    }
}
