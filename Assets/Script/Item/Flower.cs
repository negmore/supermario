using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public enum State
    {
        DOWN = 0,
        UP,
    }

    protected Vector3 downPos;
    protected Vector3 upPos;

    protected State state = State.DOWN;

    protected float waitTime = 10f;
    protected float curAnimTime = 0f;
    protected float curWaitTime = 0f;

    protected bool isMove = true;
    protected bool die = false;

    private void Update()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;
    }

    protected void UpAndDownAction()
    {
        if (true == isMove)
        {
            Vector3 targetPos = (State.DOWN == state) ? upPos : downPos;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.fixedDeltaTime * 0.5f);

            if (transform.position == targetPos)
            {
                state = (State.DOWN == state) ? State.UP : State.DOWN;
                isMove = false;
            }
        }
        else
        {
            curWaitTime += Time.fixedDeltaTime;

            if (waitTime < curWaitTime)
            {
                isMove = true;
                curWaitTime = 0f;
            }
        }
    }

    protected void Die()
    {
        Destroy(gameObject);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if ("Player" == collision.transform.tag)
        {
            GameManager.instance.player.GetComponent<PlayerController>().Dead();
        }
    }

    void GamePuase(bool IsPuase)
    {
        gameObject.SetActive(!IsPuase);
    }

    void SetSpwan()
    {

    }
}
