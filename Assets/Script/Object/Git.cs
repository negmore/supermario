using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Git : MonoBehaviour
{
    public Rigidbody2D rb;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.GameState.GAME_CLEAR == GameManager.instance.GetGameState())
        {
            rb.gravityScale = 1f;
        }

        if (rb.velocity.y < -0.5f)
            rb.velocity = new Vector2(0f, -0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ("Object" == collision.transform.tag)
        {
            GameManager.instance.StartBgm(2);
            GameManager.instance.player.GetComponent<PlayerController>().SetDirection(1f);
            GameManager.instance.player.GetComponent<PlayerController>().SetPlayerAnimation("SetStandAnimation");
        }
    }
}
