using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBlock : MonoBehaviour
{
    public SpriteRenderer renderer;
    public GameObject item;
    public GameObject item_2;
    public AudioSource audio;

    public Sprite[] ActiveAnim;
    public Sprite[] DeActiveAnim;

    private Sprite[] curAnim;

    private int curIndex = 0;

    public float aniSpeed = 8f;

    private float curTime = 0f;
    private float comboTime = 0f;

    public bool actived;
    public bool isCombo = false;
    public bool isHide = false;

    private bool action = false;
    private bool finishAction = false;

    private Vector3 startPos, actionPos;

    // Start is called before the first frame update
    void Awake()
    {
        curAnim = (true == actived) ? ActiveAnim : DeActiveAnim;
        actionPos = startPos = transform.position;
        actionPos.y += 0.2f;

        if (true == isHide)
        {
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GameState.GAME_PLAY != GameManager.instance.GetGameState())
            return;

        Animation();
        PlayAction();

        if (true == isCombo && 0f < comboTime)
        {
            comboTime -= Time.deltaTime;

            if (comboTime <= 0f)
                isCombo = false;
        }
    }

    private void Animation()
    {
        if (true == actived)
        {
            curTime += Time.deltaTime * aniSpeed;

            if (1f < curTime)
            {
                ++curIndex;
                curTime = 0f;
            }
        }

        if (curAnim.Length <= curIndex)
            curIndex = 0;

        renderer.sprite = curAnim[curIndex];
    }

    private void PlayAction()
    {
        if (false == action)
            return;

        Vector3 targetPos = (false == finishAction) ? actionPos : startPos;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 2 * Time.deltaTime);

        if (actionPos == transform.position)
            finishAction = true;
        else if (transform.position == startPos)
        {
            action = false;

            if (true == isCombo && 0f < comboTime)
                finishAction = false;

            if (null != item_2)
            {
                if (PlayerController.State.STATE_NORMAL == GameManager.instance.player.GetComponent<PlayerController>().GetState())
                    Instantiate(item_2, transform.position, Quaternion.identity);
                else
                    Instantiate(item, transform.position, Quaternion.identity);
            }
            else
            {
                GameObject objItem = Instantiate(item, transform.position, Quaternion.identity);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (false == actived)
            return;

        if ("Player" == collision.transform.tag)
        {
            if (0.7f < collision.contacts[0].normal.y)
            {
                if (true == GameManager.instance.player.GetComponent<PlayerController>().GetPushCollision())
                    return;

                GameManager.instance.player.GetComponent<PlayerController>().SetPushCollision();

                if (false == isCombo)
                    SetActived(false);

                if (true == isCombo && 0f == comboTime)
                    comboTime = Random.Range(4, 10);

                action = true;
                audio.Play();

                collision.transform.position -= new Vector3(0f, 0.12f, 0f);

                CheckUpperIsDynamicObject();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (false == actived)
            return;

        if ("Player" == collision.transform.tag)
        {
            Vector3 pos = transform.position;
            pos.y -= (gameObject.GetComponent<BoxCollider2D>().size.y / 2f) + 0.02f;

            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 0.2f);

            if (false == hit || "Player" != hit.transform.tag)
                return;

            if (true == GameManager.instance.player.GetComponent<PlayerController>().GetPushCollision())
                return;

            GameManager.instance.player.GetComponent<PlayerController>().SetPushCollision();
            gameObject.GetComponent<BoxCollider2D>().isTrigger = false;

            if (false == isCombo)
                SetActived(false);

            if (true == isCombo && 0f == comboTime)
                comboTime = Random.Range(4, 10);

            action = true;
            audio.Play();

            collision.transform.position -= new Vector3(0f, 0.12f, 0f);

            CheckUpperIsDynamicObject();
        }
    }

    public void SetActived(bool Enable)
    {
        actived = Enable;

        action = false;
        finishAction = false;

        transform.position = startPos;

        curAnim = (true == Enable) ? ActiveAnim : DeActiveAnim;
        renderer.sprite = curAnim[0];
    }

    void CheckUpperIsDynamicObject()
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
}
