using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimation : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Rigidbody2D rb;

    public Sprite[] anim;

    public float aniSpeed;
    public float delTime = 0f;

    private float time = 0f;
    private float delCurTime = 0f;

    public int loopCount = 1;
    public bool isLoop = false;

    private int index = 0;

    private void Awake()
    {
        if (0 == loopCount)
            isLoop = true;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * aniSpeed;
        delCurTime += Time.deltaTime;

        if (1f < time)
        {
            time = 0f;
            ++index;
        }

        if (anim.Length <= index)
        {
            index = 0;

            if (false == isLoop)
            {
                --loopCount;

                if (0 == loopCount)
                    Destroy(gameObject);
            }
        }

        renderer.sprite = anim[index];

        if (0 != delTime && delTime < delCurTime)
            Destroy(gameObject);
    }
}
