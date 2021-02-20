using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource bgm;
    public AudioSource effect;

    public AudioClip[] bgmList;
    public AudioClip[] EffectList;

    private float playTime = 0;

    void Awake()
    {
        bgm.clip = bgmList[0];
        bgm.Play();
    }

    private void Update()
    {
        if (0f == playTime)
            return;

       if (playTime <= bgm.time)
        {
            effect.Stop();

            playTime = 0f;
            PlayBGM(0);
        }
    }

    public void PlayBGM(int Index)
    {
        bgm.clip = bgmList[Index];

        if (4 == Index)
        {
            bgm.time = 1f;
            playTime = 11f;
        }

        bgm.Play();

        if (Index < 2 || 5 <= Index)
            bgm.loop = true;
        else
            bgm.loop = false;
    }

    public void PlayEffect(int Index)
    {
        effect.clip = EffectList[Index];
        effect.Play();
    }

    public void StopBGM()
    {
        bgm.Stop();
    }
}
