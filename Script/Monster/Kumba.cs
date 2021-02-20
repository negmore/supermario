using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kumba : Monster
{
    public Rigidbody2D rb;
    public SpriteRenderer renderer;

    public AudioSource audio;
    public AudioClip[] list;

    public Sprite[] moveAnim;
    public Sprite[] dieAnim;

    private void Awake()
    {
        if (null != GameManager.instance)
            direction = (transform.position.x <= GameManager.instance.GetPlayerPosition().x) ? 1f : -1f;
        else
            direction = -1f;

        aniSpeed = 5f;
        speed = 2f;

        curTIme = 0f;
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (false == GameManager.instance.IsGameStatePlay())
            return;

        if (false == InCameraView())
            return;

        MoveDir();

        if (false == isDead)
            Animation();
    }

    private void MoveDir()
    {
        if (true == isDead)
            return;

        float moveX = (direction * speed) * Time.deltaTime;
        moveX /= 10;
        transform.position += new Vector3(moveX, 0f, 0f);
        
    }

    private void Animation()
    {
        curTIme += Time.deltaTime * aniSpeed;

        if (1f < curTIme)
        {
            curTIme = 0f;
            ++index;
        }

        if (moveAnim.Length <= index)
            index = 0;

        renderer.sprite = moveAnim[index];
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (true == isDead)
            return;

        if ("Player" == collision.transform.tag)
        {
            if (true == collision.transform.GetComponent<PlayerController>().IsStarMode())
                DeadAction(false);

            if (collision.contacts[0].normal.y <= -0.7f)
            {
                collision.transform.GetComponent<PlayerController>().SetPumpJumpTime();
                DeadAction(true);
            }
        }

        if ("Ground" == collision.transform.tag || "Monster" == collision.transform.tag)
        {
            if (collision.contacts[0].normal.x <= -0.8f || 0.8f <= collision.contacts[0].normal.x)
                direction *= -1;
        }

        if ("0bject" == collision.transform.tag && "DeadZone" == collision.transform.name)
            DeadAction(true);
    }

    public override void SetDead(bool Die = true)
    {
        isDead = Die;
        DeadAction(false);
    }

    public override void SetPumpDead(bool isPump = true)
    {
        isDead = isPump;
        DeadAction(true);
    }


    private void DeadAction(bool isPump = false)
    {
        audio.clip = (true == isPump) ? list[0] : list[1];
        audio.Play();

        isDead = true;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;

        float destroyTime = 10f;

        if (true == isPump)
        {
            rb.simulated = false;
            destroyTime = 2f;
            renderer.sprite = dieAnim[0];
        }
        else
        {
            rb.simulated = true;
            renderer.flipY = true;

            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(0.75f, 2f), ForceMode2D.Impulse);
        }

        Vector3 pos = transform.position + (Vector3.up * transform.GetComponent<BoxCollider2D>().size.y);
        GameManager.instance.SetKillScore(pos);

        Destroy(gameObject, destroyTime);
    }

    void GamePuase(bool IsPuase)
    {
        renderer.enabled = IsPuase;
        rb.simulated = !IsPuase;
    }

    private void BlockHitAction(float CenterPos)
    {
        SetDead(true);

        float forceX = (transform.position.x - CenterPos) / 0.08f;
        float forceY = Mathf.Abs(CenterPos - transform.position.x) / 0.08f;

        rb.AddForce(new Vector2(1f * forceX, 1f * forceY), ForceMode2D.Impulse);
    }

    private bool InCameraView()
    {
        float cameraMax = FindObjectOfType<Camera>().transform.position.x + 2f;

        float sizeX = transform.GetComponent<BoxCollider2D>().size.x / 2f;

        if (transform.position.x - sizeX < cameraMax)
            return true;

        return false;
    }
}
