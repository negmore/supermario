using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour
{
    public Image image;
    public Sprite[] sprites;

    public float speed = 2f;
    private float curTime = 0f;

    private int index = 0;
    void Update()
    {
        curTime += Time.deltaTime * speed;

        if (1f < curTime)
        {
            curTime = 0f;
            ++index;
        }

        if (sprites.Length <= index)
            index = 0;

        image.sprite = sprites[index];
    }
}
