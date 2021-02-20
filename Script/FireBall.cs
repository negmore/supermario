using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Rigidbody2D rb;
    public GameObject effect;

    public Sprite[] anim;

    public float aniSpeed = 6;

    private float curTime = 0f;
    private float speed = 6f;
    private float direction = 1f;

    private int index = 0;

    private void Awake()
    {
        direction = (false == GameManager.instance.player.GetComponent<PlayerController>().GetFilpX()) ? 1f : -1f;

        rb.AddForce(new Vector2(1.5f * direction, 0f), ForceMode2D.Impulse);
    }

    void Update()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;

        Animation();
    }

    void Animation()
    {
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
        if ("Monster" == collision.transform.tag)
        {
            Monster monster = collision.transform.GetComponent<Monster>();

            if (null == monster)
                return;

            CreateEffect();
            monster.SetDead();
        }
        else if ("Ground" == collision.transform.tag || "Object" == collision.transform.tag)
        {
            if (0.7f < collision.contacts[0].normal.y)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(1.5f * direction, 1.5f), ForceMode2D.Impulse);
            }
            else
            {
                CreateEffect();
            }
        }
    }

    private void CreateEffect()
    {
        GameManager.instance.player.GetComponent<PlayerController>().DestroyFireBall();

        Instantiate(effect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void GamePuase(bool IsPuase)
    {
        renderer.enabled = IsPuase;
        rb.simulated = !IsPuase;
    }
}
