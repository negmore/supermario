using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlower : IItem
{
    public SpriteRenderer renderer;
    public Sprite[] anim;

    public float aniSpeed = 4f;

    private float curTime = 0f;
    private int index = 0;

    private void Awake()
    {
        transform.position += new Vector3(0f, 0.16f);
       
    }
    void Update()
    {
        Animation();
    }

    private void Animation()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;

        curTime += Time.deltaTime * aniSpeed;

        if (1f < curTime)
        {
            curTime = 0f;
            ++index;
        }

        if (anim.Length <= index)
            index = 0;

        renderer.sprite = anim[index];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ("Player" == collision.transform.tag)
        {
            PlayerController player = GameManager.instance.player.GetComponent<PlayerController>();

            if (null != player)
                player.ChageState(PlayerController.State.STATE_FLOWER);

            Vector3 pos = transform.position + (Vector3.up * transform.GetComponent<BoxCollider2D>().size.y);
            GameManager.instance.SetScore(pos, 1000);

            Destroy(gameObject);
        }
    }

    void GamePuase(bool IsPuase)
    {
        renderer.enabled = IsPuase;
    }
}
