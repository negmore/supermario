using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleControll : MonoBehaviour
{
    public AudioSource audio;
    public GameObject target;
    public Image[] hiscores;

    private bool isBlink = false;
    private bool blink = false;
    private float curTime = 0f;

    private void Awake()
    {
        SetHiscore(PlayerPrefs.GetInt("hiscore"));
    }

    void Update()
    {
        if (true == Input.GetKeyDown(KeyCode.X) && false == isBlink)
        {
            audio.Play();
            StartCoroutine(Play());
        }

        if (true == isBlink)
        {
            curTime += Time.deltaTime;

            if (0.25f < curTime)
            {
                curTime = 0f;
                target.SetActive(blink);
                blink = !blink;
            }
        }
    }

    IEnumerator Play()
    {
        isBlink = true;

        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("coin", 0);
        PlayerPrefs.SetInt("life", 3);

        PlayerPrefs.Save();

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("intro");
    }

    void SetHiscore(int Hiscore)
    {
        for (int i = 0; i < hiscores.Length; ++i)
        {
            hiscores[i].sprite = GameManager.instance.GetNumberSprite(Hiscore % 10);
            Hiscore /= 10;
        }
    }
}
