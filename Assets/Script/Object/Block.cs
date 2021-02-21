using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public SpriteRenderer renderer;

    public GameObject effect;
    public AudioSource audio;

    private float curTime = 0f;

    public bool actived = true;
    private bool action = false;
    private bool finishAction = false;

    private Vector3 startPos, actionPos;

    void Awake()
    {
        actionPos = startPos = transform.position;
        actionPos.y += 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;

        PlayAction();
    }

    private void PlayAction()
    {
        if (false == action)
            return;

        Vector3 targetPos = (false == finishAction) ? actionPos : startPos;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 2 * Time.deltaTime);

        if (actionPos == transform.position)
        {
            finishAction = true;
        }
        else if (transform.position == startPos)
        {
            action = false;
            finishAction = false;
            curTime = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (true == action)
            return;

        if ("Player" == collision.transform.tag)
        {
            if (0.7f < collision.contacts[0].normal.y)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();

                if (true == player.GetPushCollision())
                    return;

                player.SetPushCollision();

                if (PlayerController.State.STATE_NORMAL == player.state)
                {
                    SetActived(false);
                    action = true;

                    audio.Play();

                    CheckUpperIsDynamicObject();
                }
                else
                {
                    CheckUpperIsDynamicObject();

                    PlayEffect();
                    Destroy(gameObject);
                }
            }
        }
    }

    public void SetActived(bool Enable)
    {
        actived = Enable;

        action = false;
        finishAction = false;

        transform.position = startPos;
    }

    private void CheckUpperIsDynamicObject()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y + 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.up, 10f);

        if (true == hit)
        {
            if ("Monster" == hit.transform.tag)
            {
                Monster monster = hit.transform.GetComponent<Monster>();
                monster.SendMessage("BlockHitAction", transform.position.x);
            }
            else if ("Item" == hit.transform.tag)
            {
                IItem item = hit.transform.GetComponent<IItem>();
                item.SendMessage("BlockHitAction", transform.position.x);
            }
        }
    }

    void PlayEffect()
    {
        for (int i = 0; i < 4; ++i)
        {
            GameObject blokenEffect = Instantiate(effect, transform.position, Quaternion.identity);
            Rigidbody2D rigid = blokenEffect.transform.GetComponent<Rigidbody2D>();

            float dirX = (0 == i % 2) ? 1f : -1f;
            float dirY = (1 < i) ? 2.5f : 1f;

            rigid.velocity = Vector2.zero;
            rigid.AddForce(new Vector2(dirX, dirY), ForceMode2D.Impulse);
        }
    }
}
