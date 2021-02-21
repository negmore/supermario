using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nokonoko : Monster
{
    public enum State
    {
        SHELL = 0,
        NORMAL,
        WING,
    };

    public Rigidbody2D rb;
    public SpriteRenderer renderer;

    public Sprite[] animWing;
    public Sprite[] animNormal;
    public Sprite[] animDie;
    public Sprite[] animShell;
    public Sprite[] animCurrent;

    public State state;

    private bool isCatch = false;

    private void Awake()
    {
        aniSpeed = 5f;
        speed = 1f;
        jumpHeight = 3f;

        if (null != GameManager.instance)
            direction = (transform.position.x <= GameManager.instance.GetPlayerPosition().x) ? 1f : -1f;
        else
            direction = -1f;

        ChangeState();
    }

    void FixedUpdate()
    {
        if (false == GameManager.instance.IsGameStatePlay())
            return;

        CollisionCheck();
        MoveDir();
        Animation();

        if (true == isShell && 15f < shellTime)
        {
            if (true == isShellMoving)
                shellTime = 0f;

            state = State.NORMAL;
            ChangeState();
        }
    }

    private void MoveDir()
    {
        if (true == isDead)
            return;

        if (true == isShell)
        {
            if (true == isShellMoving)
            {
                rb.velocity = new Vector2(direction * 2f, rb.velocity.y);
            }

        }
        else
        {
            float moveX = (direction * speed) * Time.fixedDeltaTime;
            moveX /= 10;
            transform.position += new Vector3(moveX, 0f);

            if (true == isGround && State.WING == state)
            {
                rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                isGround = false;
            }
        }
    }

    private void Animation()
    {
        curTIme += Time.fixedDeltaTime * aniSpeed;

        if (1f < curTIme)
        {
            curTIme = 0f;
            ++index;
        }

        if (true == isShell)
        {
            if (false == isShellMoving)
                index = 0;

        }

        if (animNormal.Length <= index)
            index = 0;

        renderer.sprite = animCurrent[index];
    }

    void CollisionCheck()
    {
        Vector3 pos = transform.position;
        pos.y -= 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.down, 0.1f);
        Debug.DrawRay(pos, Vector3.down * 0.1f, Color.red);

        if (false == hit)
            return;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (true == isDead)
            return;

        if ("Player" == collision.transform.tag)
        {
            if (collision.contacts[0].normal.y <= -0.7f)
            {
                if (State.SHELL == state)
                {
                    if (true == isShellMoving)
                    {
                        ShellMoveDirection(false);
                    }
                    else
                    {
                        ShellMoveDirection();
                    }
                }
                else
                {
                    state = (State.WING == state) ? State.NORMAL : State.SHELL;
                    ChangeState();
                }
            }

            if (true == isShell)
            {
                if (0.7 <= collision.contacts[0].normal.x || collision.contacts[0].normal.x <= -0.7f)
                {
                    if (true == isShellMoving)
                        GameManager.instance.player.GetComponent<PlayerController>().Dead();
                    else
                    {
                        ShellMoveDirection();
                    }
                }
            }
        }

        if ("Ground" == collision.transform.tag || "monster" == collision.transform.tag)
        {
            if (0.7f < collision.contacts[0].normal.y)
            {
                isGround = true;
            }

            if (collision.contacts[0].normal.x <= -0.8f || 0.8f <= collision.contacts[0].normal.x)
                direction *= -1;
        }

        if ("0bject" == collision.transform.tag && "DeadZone" == collision.transform.name)
            Die(false);
    }

    private void Die(bool PumpDie)
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
        rb.simulated = false;

        isDead = true;

        float destroyTime = 5f;

        if (true == PumpDie)
        {
            renderer.sprite = animDie[0];
            destroyTime = 2f;
        }

        Vector3 pos = transform.position + (Vector3.up * transform.GetComponent<BoxCollider2D>().size.y);
        GameManager.instance.SetKillScore(pos);

        Destroy(gameObject, destroyTime);
    }

    private void ShellMoveDirection(bool move = true)
    {
        if (true == move)
        {
            direction = (transform.position.x <= GameManager.instance.player.transform.position.x) ? -1f : 1f;

            Vector3 movePos = Vector3.zero;
            movePos.x = 0.08f * direction;

            transform.position += movePos;
        }
        else
            direction = 0f;

        isShellMoving = move;

    }

    private void ChangeState()
    {
        index = 0;
        curTIme = 0f;
        speed = 1.5f;

        if (State.SHELL != state)
        {
            isShell = false;
            isShellMoving = false;
            shellTime = 0f;
            rb.gravityScale = 1f;
        }
        else
        {
            rb.gravityScale = 0.7f;
        }

        switch(state)
        {
            case State.WING:
                {
                    animCurrent = animWing;
                    speed = 2f;
                }
                break;
            case State.NORMAL:
                {
                    animCurrent = animNormal;
                }
                break;
            case State.SHELL:
                {
                    animCurrent = animShell;

                    isShell = true;
                    shellTime = Time.fixedDeltaTime;
                    direction = 0f;
                }
                break;
        }

        renderer.sprite = animCurrent[index];
    }

    public void SetCatchMode(bool IsCatch)
    {
        if (true == isCatch && false == IsCatch)
            ShellMoveDirection();

        isCatch = IsCatch;
    }

    void GamePuase(bool IsPuase)
    {
        renderer.enabled = IsPuase;
        rb.simulated = !IsPuase;
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
