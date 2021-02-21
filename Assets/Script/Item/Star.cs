using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : IItem
{
    public SpriteRenderer renderer;
    public Rigidbody2D rb;

    public Sprite[] anim;

    private float direction = 1f;
    private float curTime = 0f;

    private int index = 0;

    public float speed = 30f;
    public float aniSpeed = 2f;

    private bool isSpwan = true;

    private Vector3 movePos;

    private void Awake()
    {
        if (null == rb)
            rb = transform.GetComponent<Rigidbody2D>();

        movePos = new Vector3(transform.position.x, transform.position.y + 0.16f);
        rb.simulated = (true == isSpwan) ? false : true;
    }

    void Update()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;

        if (true == isSpwan)
        {
            SpwanAction();
        }
        else
        {
            Move();
        }

        Animation();
    }

    private void Animation()
    {
        curTime += Time.fixedDeltaTime * aniSpeed;

        if (1f < curTime)
        {
            curTime = 0f;
            ++index;
        }

        if (anim.Length <= index)
            index = 0;

        renderer.sprite = anim[index];
    }

    private void Move()
    {
        float moveX = direction * speed * Time.fixedDeltaTime * Time.fixedDeltaTime;
        transform.position += new Vector3(moveX, 0f, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (true == isSpwan)
            return;

        if ("Ground" == collision.transform.tag || "Object" == collision.transform.tag)
        {
            if (0.7 < collision.contacts[0].normal.y)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(rb.velocity.x, 3f), ForceMode2D.Impulse);
            }

            if (0.7f < collision.contacts[0].normal.x || collision.contacts[0].normal.x < -0.7f)
                direction *= -1f;
        }

        if ("Player" == collision.transform.tag)
        {
            PlayerController player = GameManager.instance.player.GetComponent<PlayerController>();
            player.SetStarMode(true);

            Vector3 pos = transform.position + (Vector3.up * transform.GetComponent<BoxCollider2D>().size.y);
            GameManager.instance.SetScore(pos, 1000);

            Destroy(gameObject);
        }
    }

    void SpwanAction()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePos, Time.fixedDeltaTime * 0.2f);

        if (movePos == transform.position)
        {
            isSpwan = false;
            direction = GameManager.instance.player.GetComponent<PlayerController>().GetDirection() * -1f;

            if (0 == direction)
                direction = 1f;

            rb.simulated = true;
        }

    }

    void GamePuase(bool IsPuase)
    {
        renderer.enabled = IsPuase;
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
