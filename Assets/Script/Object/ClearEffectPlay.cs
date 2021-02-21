using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearEffectPlay : MonoBehaviour
{
    public GameObject git;
    public GameObject[] explorer;

    public AudioSource audio;
    private Vector3 targetPos;

    public bool isPlay = false;
    public bool isEnd = false;
    private float overTIme = 5f;

    private void Awake()
    {
        targetPos = git.transform.position;
        targetPos.y += 0.18f;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (true == isPlay)
            Play();
        else
        {
            if (true == isEnd)
            {
                overTIme -= Time.deltaTime;

                if (overTIme <= 0f)
                {
                    isEnd = false;

                    GameManager.instance.Save();
                    SceneManager.LoadScene("gameover");
                }

            }
        }
    }

    public void SetPlay()
    {
        git.SetActive(true);
        isPlay = true;
    }

    void Play()
    {
        if (git.transform.position == targetPos)
        {
            for (int i = 0; i < explorer.Length; ++i)
            {
                StartCoroutine(ShowExplorerEffect(i));
            }

            isPlay = false;
            isEnd = true;
        }
        else
        {
            git.transform.position = Vector3.MoveTowards(git.transform.position, targetPos, Time.deltaTime * 0.25f);
        }
    }

    private IEnumerator ShowExplorerEffect(int index)
    {
        yield return new WaitForSeconds(0.5f + (index * 0.5f));

        audio.Play();
        explorer[index].SetActive(true);

    }
}
