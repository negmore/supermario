using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : IItem
{
    public Rigidbody2D rb;

    protected Vector3 movePos;

    protected float direction = 0;
    public float speed = 3f;

    protected bool isSpwan = false;

    private void Awake()
    {
        movePos = transform.position;
        movePos.y += 0.16f;

        direction = 1f;
        rb.simulated = true;

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

    void SetSpwan()
    {
        isSpwan = true;
        rb.simulated = false;
    }

    protected void SpwanAction()
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

    protected void Move()
    {
        transform.position += new Vector3(direction * speed * Time.deltaTime * 0.1f, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ("Player" == collision.transform.tag)
        {
            PlayerController player = GameManager.instance.player.GetComponent<PlayerController>();

            if (PlayerController.State.STATE_NORMAL == player.GetState())
                player.ChageState(PlayerController.State.STATE_BIG);

            Vector3 pos = transform.position + (Vector3.up * transform.GetComponent<BoxCollider2D>().size.y);
            GameManager.instance.SetScore(pos, 1000);

            Destroy(gameObject);
        }

        if ("Ground" == collision.transform.tag)
        {
            if (0.7f < collision.contacts[0].normal.x || collision.contacts[0].normal.x < -0.7f)
                direction *= -1f;
        }
    }

    public void Use()
    {

    }

    void GamePuase(bool IsPuase)
    {
        gameObject.SetActive(!IsPuase);
        rb.simulated = !IsPuase;
    }

    void BlockHitAction(float CenterPos)
    {
        float forceX = (CenterPos - transform.position.x) / 0.08f;
        float forceY = Mathf.Abs(CenterPos - transform.position.x) / 0.08f;

        rb.AddForce(new Vector2(1f * forceX * direction, 1f * forceY), ForceMode2D.Impulse);

        if ((forceX < 0f && direction < 0f) || (0f < forceX && 0f < direction))
            direction *= -1f;
    }
}
