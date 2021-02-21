using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinEffect : Coin
{
    private Vector3 upMove, downMove;
    private bool action = true;

    private void Awake()
    {
        upMove = downMove = transform.position;
        upMove.y += 0.56f;
        downMove.y += 0.32f;

        aniSpeed *= 12f;

        isUse = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;

        Animation();
        EffectAnimation();
    }

    private void EffectAnimation()
    {
        Vector3 targetPos = (true == action) ? upMove : downMove;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 2 * Time.deltaTime);

        if (true == action && upMove == transform.position)
            action = false;
        else if (false == action && downMove == transform.position)
        {
            GameManager.instance.SetCoin(transform.position);

            Destroy(gameObject);
        }
    }

    void GamePuase(bool IsPuase)
    {
        renderer.enabled = IsPuase;
    }
}
