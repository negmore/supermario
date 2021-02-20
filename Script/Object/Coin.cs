using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : IItem
{
    public SpriteRenderer renderer;
    public Sprite[] sprite;

    public float curTime = 0f;
    public float aniSpeed = 4f;

    private int index = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;

        Animation();
    }

    public void Animation()
    {
        float speed = aniSpeed;
        curTime += Time.deltaTime * speed;

        if (1f < curTime)
        {
            curTime = 0f;
            ++index;
        }

        if (sprite.Length <= index)
            index = 0;

        renderer.sprite = sprite[index];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ("Player" == collision.transform.tag)
        {
            Vector3 pos = transform.position + (Vector3.up * transform.GetComponent<BoxCollider2D>().size.y);
            GameManager.instance.SetCoin(pos, true);

            Destroy(gameObject);
        }
    }

    void GamePuase(bool IsPuase)
    {
        renderer.enabled = IsPuase;
    }
}
