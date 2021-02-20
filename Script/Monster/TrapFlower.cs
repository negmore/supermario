using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapFlower : Flower
{
    public SpriteRenderer renderer;

    public Sprite[] anim;

    public float aniSpeed = 2f;

    private float time = 0f;
    private int index = 0;

    void Awake()
    {
        upPos = downPos = transform.position;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        upPos.y += collider.size.y;
    }

    void Update()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;

        UpAndDownAction();
        Animation();
    }

    private void Animation()
    {
        time += Time.deltaTime * aniSpeed;

        if (1f < time)
        {
            time = 0f;
            ++index;
        }

        if (anim.Length <= index)
            index = 0;

        renderer.sprite = anim[index];
    }

    void GamePuase(bool IsPuase)
    {
        renderer.enabled = IsPuase;
    }
}
