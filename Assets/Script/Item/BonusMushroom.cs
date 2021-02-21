using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusMushroom : Mushroom
{
    private void Awake()
    {
        movePos = transform.position;
        movePos.y += 0.16f;

        if (null == rb)
            rb = transform.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;

        if (true == isSpwan)
            SpwanAction();
        else
            Move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ("Player" == collision.transform.tag)
        {
            Vector3 pos = transform.position + (Vector3.up * transform.GetComponent<BoxCollider2D>().size.y);
            GameManager.instance.SetLife(pos, 1);

            Destroy(gameObject);
        }

        if ("Ground" == collision.transform.tag)
        {
            if (0.7f < collision.contacts[0].normal.x || collision.contacts[0].normal.x < -0.7f)
                direction *= -1f;
        }
    }

    void GamePuase(bool IsPuase)
    {
        //renderer.enabled = IsPuase;
        rb.simulated = !IsPuase;
    }

    void BlockHitAction(float CenterPos)
    {
        float forceX = (transform.position.x - CenterPos) / 0.08f;
        float forceY = Mathf.Abs(CenterPos - transform.position.x) / 0.08f;

        rb.AddForce(new Vector2(1f * forceX * direction, 1f * forceY), ForceMode2D.Impulse);

        if ((forceX < 0f && direction < 0f) || (0f < forceX && 0f < direction))
            direction *= -1f;
    }

}
