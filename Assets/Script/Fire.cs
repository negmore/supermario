using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Sprite[] anim;

    public float aniSpeed = 2f;
    public float moveSpeed = 0.2f;

    private float aniTime = 0f;
    private float dirHorizontal = 0f;
    private float dirVertical = 0f;

    private int index = 0;

    void Update()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;

        Animation();
        Move();
    }

    void Animation()
    {
        aniTime += Time.deltaTime * aniSpeed;

        if (1f < aniTime)
        {
            aniTime = 0f;
            ++index;
        }

        if (anim.Length <= index)
            index = 0;

        renderer.sprite = anim[index];
    }

    void Move()
    {
        float posX = dirHorizontal * Time.deltaTime * moveSpeed;
        float posY = dirVertical * Time.deltaTime * moveSpeed;

        transform.position += new Vector3(posX, posY, 0f);
    }

    public void SetTarget(Vector3 StartPos, Vector3 TargetPos, Vector2 Size)
    {
        dirHorizontal = (StartPos.x <= TargetPos.x) ? 1f : -1f;
        dirVertical = (StartPos.y <= TargetPos.y) ? 1f : -1f;

        transform.position = new Vector3(StartPos.x + (Size.x / 2 * dirHorizontal), StartPos.y + (Size.y / 4), 0f);

        Destroy(gameObject, 10f);

        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ("Player" == collision.transform.tag)
        {
            PlayerController player = collision.transform.GetComponent<PlayerController>();
            player.Dead();
        }
    }

    void GamePuase(bool IsPuase)
    {
        renderer.enabled = IsPuase;
    }
}
