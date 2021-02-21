using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedFlower : Flower
{
    public GameObject fireball;

    public SpriteRenderer renderer;

    public Sprite curAnim;
    public Sprite upAnim;
    public Sprite downAnim;

    private float shootTime = 0f;

    private void Awake()
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
        Action();
    }

    void Action()
    {
        if (State.UP != state)
        {
            shootTime = 0f;
            return;
        }

        curAnim = (transform.position.y < GameManager.instance.player.transform.position.y) ? upAnim : downAnim;
        shootTime += Time.deltaTime;

        if (4f < shootTime)
        {
            GameObject gameObject = Instantiate(fireball, transform.position, Quaternion.identity);
            gameObject.GetComponent<Fire>().SetTarget(transform.position, GameManager.instance.player.transform.position, GetComponent<BoxCollider2D>().size);

            shootTime = 0f;
        }

        renderer.sprite = curAnim;

        if (transform.position.x < GameManager.instance.player.transform.position.x)
            renderer.flipX = true;
        else
            renderer.flipX = false;
    }

    void GamePuase(bool IsPuase)
    {
        renderer.enabled = IsPuase;
    }
}
