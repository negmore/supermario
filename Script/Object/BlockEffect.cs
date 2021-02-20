using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEffect : MonoBehaviour
{
    public GameObject effect;

    // Update is called once per frame
    void Start()
    {
        PlayEffect();
    }

    private void Update()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;
    }

    void PlayEffect()
    {
        CreateEffect(new Vector2(-0.48f, 0.48f));
        CreateEffect(new Vector2(0.48f, 0.48f));
        CreateEffect(new Vector2(-0.16f, 0.16f));
        CreateEffect(new Vector2(0.16f, 0.16f));

        Destroy(gameObject, 4f);

    }

    void CreateEffect(Vector2 Position)
    {
        GameObject gameObj = Instantiate(effect, transform.position, Quaternion.identity);
        Rigidbody2D rigid = gameObj.GetComponent<Rigidbody2D>();
        rigid.AddForce(Position, ForceMode2D.Impulse);

        Destroy(gameObj, 4f);
    }
}
