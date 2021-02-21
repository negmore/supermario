using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        STATE_NORMAL = 0,
        STATE_BIG,
        STATE_FLOWER,
    };

    public SpriteRenderer renderer;
    private Rigidbody2D playerBody;
    private Animator animator;

    public GameObject[] animState;
    public GameObject[] animStar;
    public GameObject fireBall;

    public AudioSource audio;
    public AudioClip[] soundList;

    public State state = State.STATE_NORMAL;

    private float direction = 0f;

    // 기본
    public float moveSpeed = 1f;

    private float Velocity;

    private bool isDead = false;
    private bool isPushCollision = false;
    
    // 점프
    // 점프
    public float jumpHeight = 3.8f;
    private float jumpLimitTime = 2f;
    private float jumpStartTime = 0f;
    private float pumpJump = 0.1f;

    private bool isGround;
    private bool isJump;
    private bool isJumping;
    private bool isDown;

    // 대쉬
    private float dashTime = 0f;
    private float dashUseTime = 2f;
    private bool isDash;

    // 불꽃 관리 변수
    public int fireCount = 2;
    public float shootCoolTIme = 0.5f;

    private float shootTime = 0f;

    // 무적
    public bool isStarMode = false;
    public float starTime = 0f;

    // 사망
    private bool isDeadAction = false;

    // 파이프 이동
    private bool isWarpGate = false;
    private bool isWarpOut = false;

    private Vector3 deadPos;
    private Vector3 warpInPos, warpOutStay, warpOutPos;

    // 무적 판정
    private float setInvTime = 3f;
    private float curInvTime = 0f;
    private float blinkTime = 0f;
    private float colorAlpha = 0f;

    public int audioIndex = 0;
    public bool MoveCalState = false;
    
    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        isDash = isJump = false;

        ChageState(state, true);
    }

    private void Update()
    {
        if (GameManager.instance.GetGameState() == GameManager.GameState.GAME_PLAY)
        {
            KeyInput();
        }
    }

    void FixedUpdate()
    {
        switch (GameManager.instance.GetGameState())
        {
            case GameManager.GameState.GAME_READY:
                break;
            case GameManager.GameState.GAME_PLAY:
                    UpdateGamePlay();
                break;
            case GameManager.GameState.GAME_PUASE:
                    UpdateGamePuase();
                return;
            case GameManager.GameState.GAME_CLEAR:
                    UpdateGameClear();
                break;
            case GameManager.GameState.GAME_DIE:
                    UpdateGameDead();
                break;
        }
    }

    void UpdateGamePlay()
    {
        if (true == isDead)
        {
            DeadAction();
            return;
        }

        if (true == isWarpGate)
        {
            WarpAction();
            return;
        }

        Move();

        if (true == isStarMode)
        {
            starTime -= Time.fixedDeltaTime;

            if (starTime <= 0f)
                SetStarMode(false);
        }

        if (0f != shootTime)
        {
            shootTime -= Time.fixedDeltaTime;

            if (shootTime < 0f)
                shootTime = 0f;
        }

        InvincibilityTime();
        TimeCalculator();

    }

    void TimeCalculator()
    {
        if (0f < pumpJump)
            pumpJump -= Time.fixedDeltaTime;
    }

    void UpdateGameDead()
    {
        shootCoolTIme += Time.fixedDeltaTime;

        if (4f < shootCoolTIme)
            GameManager.instance.PlayerDead();
    }

    void UpdateGamePuase()
    {
        DeadAction();
    }

    void UpdateGameClear()
    {
        Move();

        if (playerBody.velocity.y < -0.5f)
        {
            playerBody.velocity = new Vector2(0f, -0.5f);
        }
    }

    void KeyInput()
    {
        float prevDir = direction;

        if (true == Input.GetKey(KeyCode.RightArrow))
            direction = 1f;
        else if (true == Input.GetKey(KeyCode.LeftArrow))
            direction = -1f;
        else if (false == Input.GetKey(KeyCode.RightArrow) && false == Input.GetKey(KeyCode.LeftArrow))
            direction = 0f;

        if (true == Input.GetKey(KeyCode.DownArrow))
        {
            if (true == isGround && State.STATE_NORMAL != state)
            {
                if (true == isGround)
                {
                    isDown = true;
                    direction = 0f;
                }
            }
        }
        else if (true == Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (State.STATE_NORMAL != state)
                isDown = false;
        }

        if (true == Input.GetKeyDown(KeyCode.X))
        {
            if ((true == isGround && false == isJump) || 0f < pumpJump)
            {
                if (0f < pumpJump)
                    playerBody.velocity = Vector3.zero;

                float jumpPower = (State.STATE_NORMAL == state) ? 3.75f : 4f;
                jumpHeight *= Time.fixedDeltaTime;
                playerBody.velocity = new Vector2(playerBody.velocity.x, jumpPower);

                //playerBody.AddForce(Vector2.up * (jumpHeight), ForceMode2D.Impulse);

                isDown = false;
                isJump = true;
                isGround = false;
                pumpJump = 0f;

                animState[(int)state].SendMessage("SetJumpAnimation");
                SoundPlay(0);
            }
        }
        else if (true == Input.GetKeyUp(KeyCode.X))
        {
            if (false == isGround && true == isJump && 0f < playerBody.velocity.y)
            {
                playerBody.velocity = new Vector2(playerBody.velocity.x, 0f);
            }
        }

        if (true == Input.GetKeyDown(KeyCode.Z))
        {
            if (State.STATE_FLOWER == state && 0f == shootTime && 0 < fireCount)
            {
                FireBallShoot();
            }
        }
        else if (true == Input.GetKey(KeyCode.Z))
        {
            isDash = true;
            dashTime += Time.fixedDeltaTime;
        }
        else if (true == Input.GetKeyUp(KeyCode.Z))
        {
            isDash = false;
            dashTime = 0f;           
        }
    }
    void Move()
    {
        float speed = (0 == direction) ? moveSpeed * 4f : moveSpeed;
        float mul = 0f;

        if (true == isDash)
        {
            if (dashUseTime < dashTime)
                dashTime = dashUseTime;

            mul = dashTime * Time.fixedDeltaTime * 12.5f;
        }

        if (GameManager.GameState.GAME_CLEAR == GameManager.instance.GetGameState())
            mul /= 2f;

        animState[(int)state].SendMessage("SetDashSpeed", 1 + (mul * 1.5f));

        Velocity = Mathf.Lerp(Velocity, direction, (speed + mul) * Time.fixedDeltaTime);

        if (false == MoveCalState)
        {
            var pos = transform.position;
            pos.x += (Velocity * (moveSpeed + mul)) * Time.fixedDeltaTime;

            float cameraPosX = FindObjectOfType<Camera>().transform.position.x;
            float playerSizeX = GetComponent<BoxCollider2D>().size.x / 2f;

            if (pos.x - playerSizeX < cameraPosX - 2f)
                pos.x = (cameraPosX - 2f) + playerSizeX;

            transform.position = pos;
        }
        else
        {
            playerBody.velocity = new Vector2((Velocity * (moveSpeed + mul)), playerBody.velocity.y);
        }

        if (1 == direction)
            renderer.flipX = false;
        else if (-1 == direction)
            renderer.flipX = true;

        if (false == isGround)
            return;

        bool isFlip = (true == isGround && ((Velocity < 0f && 0f < direction) || (0f < Velocity && direction < 0f)));

        if (true == isFlip)
        {
            animState[(int)state].SendMessage("SetFlipAnimation");
            SoundPlay(1);
        }
        else
            SetMoveAnimation();
    }

    public void Dead()
    {
        GameManager.instance.StartBgm(3);

        isDeadAction = isDead = true;
        animState[(int)state].SendMessage("SetDeadAnimation");

        deadPos = transform.position + (Vector3.up * 0.8f);

        playerBody.velocity = Vector3.zero;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ("Ground" == collision.transform.tag || "Object" == collision.transform.tag)
        {
            if ("DeadZone" == collision.transform.name)
                Dead();

            for (int i = 0; i < collision.contacts.Length; ++i)
            {
                if (0.7f < collision.contacts[i].normal.y)
                {
                    if ("Goal_Block" == collision.transform.name && GameManager.GameState.GAME_CLEAR == GameManager.instance.GetGameState())
                        return;
                    else if (GameManager.GameState.GAME_CLEAR == GameManager.instance.GetGameState())
                    {
                        renderer.sortingLayerName = "middle";
                    }

                    isGround = true;
                    isJumping = false;
                    isJump = false;
                    isPushCollision = false;

                    GameManager.instance.InitComboKillScore();
                }
            }
        }

        for (int i = 0; i < collision.contacts.Length; ++i)
        {
            if (collision.contacts[i].normal.x < -0.7f || 0.7f < collision.contacts[i].normal.x)
            {
                if ("Ground" == collision.transform.tag)
                {
                    playerBody.velocity = new Vector2(0f, playerBody.velocity.y);
                }
            }

            if ("Monster" == collision.transform.tag)
            {
                if (0.7f <= collision.contacts[0].normal.y)
                {
                    playerBody.velocity = new Vector2(playerBody.velocity.x, playerBody.velocity.y + 0.4f);
                }
                else
                {
                    if (true == isStarMode)
                        return;

                    if (true == UnityEngine.Physics2D.GetIgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monster")))
                        return;

                    DownState();
                }
            }
        }

        // clear
        if ("Clear" == collision.transform.tag && "Goal" == collision.transform.name)
        {
            GameManager.instance.StopBgm();
            SoundPlay(4);
            GameManager.instance.SetGameState(GameManager.GameState.GAME_CLEAR);

            direction = 0f;
            playerBody.velocity = Vector2.zero;

            animState[(int)state].SendMessage("SetClearAnimation");

            Destroy(collision.transform.gameObject, 1f);
        }

        if (GameManager.GameState.GAME_CLEAR != GameManager.instance.GetGameState())
            return;

        if ("Object" == collision.transform.tag)
        {
            if ("Goal_Check" == collision.transform.name)
            {
                direction = 0f;
                GameManager.instance.StageClear();
            }
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if ("Ground" == collision.transform.tag || "Object" == collision.transform.tag)
        {
            if (0 != playerBody.velocity.y)
            {
                isGround = false;
                isJumping = true;

                if (false == isDead)
                    animState[(int)state].SendMessage("SetJumpAnimation");
            }
        }
    }

    public float GetDirection()
    {
        return direction;
    }

    public float GetSpeed()
    {
        return dashTime;
    }

    private void DeadAction()
    {
        if (false == isDead)
            return;

        if (true == isDeadAction)
        {
            transform.position = Vector3.MoveTowards(transform.position, deadPos, 3 * Time.fixedDeltaTime);

            if (transform.position == deadPos)
                isDeadAction = false;
        }
        else
        {
            jumpStartTime += Time.fixedDeltaTime;
        }
    }

    public bool GetFilpX()
    {
        return renderer.flipX;
    }

    public void DownState()
    {
        if (State.STATE_NORMAL == state)
            Dead();
        else
        {
            BoxCollider2D collider = transform.GetComponent<BoxCollider2D>();

            collider.size = new Vector2(collider.size.x, 0.14f);

            transform.position = new Vector3(transform.position.x, transform.position.y - 0.08f);
            jumpHeight = 3.8f;

            animState[(int)state].SetActive(false);
            state = State.STATE_NORMAL;
            animState[(int)state].SetActive(true);

            setInvTime = 3f;
            SetInvincibility(true);
        }
    }
    public void ChageState(State Change, bool SetInit = false)
    {
        if (false == SetInit)
        {
            SoundPlay(2);
        }

        if (State.STATE_NORMAL == state && State.STATE_FLOWER == Change)
        {
            ChageState(State.STATE_BIG);
            return;
        }
        else
        {
            animState[(int)state].SetActive(false);
            animState[(int)Change].SetActive(true);
        }

        BoxCollider2D collider = animState[(int)Change].GetComponent<BoxCollider2D>();
        transform.GetComponent<BoxCollider2D>().offset = collider.offset;
        transform.GetComponent<BoxCollider2D>().size = collider.size;

        if (true == SetInit)
        {
            collider.size = new Vector2(collider.size.x, 0.30f);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.08f);
        }

        if (state != Change)
        {
            GameManager.instance.SetGameState(GameManager.GameState.GAME_CHANGE);
            StartCoroutine(ChagneAnimation((int)state, (int)Change));
        }

        jumpHeight = 4.05f;
        state = Change;
    }

    IEnumerator ChagneAnimation(int Prev, int Next)
    {
        int count = 0;

        playerBody.simulated = false;
        transform.GetComponent<BoxCollider2D>().isTrigger = true;

        string state = animState[Prev].GetComponent<PlayAnimation>().GetAnimationState();
        int index = animState[Prev].GetComponent<PlayAnimation>().GetAnimationIndex();

        while (count < 4)
        {
            animState[Prev].SetActive(false);
            animState[Next].SetActive(true);

            animState[Next].GetComponent<PlayAnimation>().SetChangedState(state, index);

            if (0 == Prev)
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.08f);

            yield return new WaitForSeconds(0.125f);

            animState[Next].SetActive(false);
            animState[Prev].SetActive(true);

            animState[Prev].GetComponent<PlayAnimation>().SetChangedState(state, index);

            if (0 == Prev)
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.08f);

            yield return new WaitForSeconds(0.125f);

            ++count;
        }

        animState[Prev].SetActive(false);
        animState[Next].SetActive(true);

        animState[Next].GetComponent<PlayAnimation>().SetChangedState(state, index);

        playerBody.simulated = true;
        transform.GetComponent<BoxCollider2D>().isTrigger = false;

        GameManager.instance.SetGameState(GameManager.GameState.GAME_PLAY);

    }

    public void SetStarMode(bool StarMode)
    {
        if (true == StarMode)
            SetInvincibility(false);

        isStarMode = StarMode;

        if (true == StarMode)
        {
            GameManager.instance.StartBgm(4);
            SoundPlay(2);
            starTime = 12f;
            animState[(int)state].SendMessage("SetStartMode", starTime);
        }
    }

    public void SetWarpPosition(Vector3 Position, Vector3 dirIn, Vector3 outDir)
    {
        SoundPlay(3);

        isWarpGate = true;

        warpInPos = transform.position;
        warpInPos += dirIn * 0.48f;

        warpOutStay = Position + (outDir * -0.48f);

        float outPos = (0f != outDir.x) ? transform.GetComponent<BoxCollider2D>().size.x / 2f : transform.GetComponent<BoxCollider2D>().size.y / 2f;
        warpOutPos = Position + (outDir * outPos);

        transform.GetComponent<BoxCollider2D>().enabled = false;
        playerBody.simulated = false;

        renderer.sortingLayerName = "middle";

        if (0 != direction)
            animState[(int)state].SendMessage("SetWalkAnimation");
        else
            animState[(int)state].SendMessage("SetStandAnimation");
    }

    private void WarpAction()
    {
        if (false == isWarpOut)
        {
            transform.position = Vector3.MoveTowards(transform.position, warpInPos, Time.fixedDeltaTime * 0.5f);

            if (transform.position == warpInPos)
            {
                GameManager.instance.SetFadeInOut(true);
                new WaitForSeconds(1f);

                transform.position = warpOutStay;

                isWarpOut = true;

                GameManager.instance.SetFadeOff();

                int bgmIndex = (transform.position.y <= -1f) ? 5 : 0;
                GameManager.instance.StartBgm(bgmIndex);

                SoundPlay(3, true);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, warpOutPos, Time.fixedDeltaTime * 0.5f);

            if (transform.position == warpOutPos)
            {
                isWarpOut = false;
                isWarpGate = false;

                transform.GetComponent<BoxCollider2D>().enabled = true;
                playerBody.simulated = true;

                renderer.sortingLayerName = "forward";
            }
        }
    }

    void FireBallShoot()
    {
        if (null == fireBall)
            return; 

        float sizeX = (transform.GetComponent<BoxCollider2D>().size.x / 2) * ((false == renderer.flipX) ? 1f : -1f);
        float sizeY = (transform.GetComponent<BoxCollider2D>().size.y / 2) - 0.03f;

        Vector3 pos = new Vector3(transform.position.x + sizeX, transform.position.y + sizeY);
        Instantiate(fireBall, pos, Quaternion.identity);

        shootTime = shootCoolTIme;
        animState[(int)state].SendMessage("SetShootAnimation");

        --fireCount;
    }

    public void DestroyFireBall()
    {
        ++fireCount;
    }

    public void SetPushCollision()
    {
        isPushCollision = true;
    }

    public bool GetPushCollision()
    {
        return isPushCollision;
    }

    public State GetState()
    {
        return state;
    }
    void InvincibilityTime()
    {
        if (curInvTime <= 0f)
            return;

        CheckInviTimeCollision();

        curInvTime -= Time.fixedDeltaTime;
        blinkTime += Time.fixedDeltaTime;

        if (0.05f < blinkTime)
        {
            blinkTime = 0f;

            colorAlpha = (0f == colorAlpha) ? 255f : 0f;
            SetBlink();
        }

        if (curInvTime <= 0f)
        {
            SetInvincibility(false);

            colorAlpha = 255f;
            SetBlink();
            colorAlpha = 0f;
        }
    }

    void SetInvincibility(bool Switch)
    {
        curInvTime = (true == Switch) ? setInvTime : 0f;

        UnityEngine.Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monster"), Switch);
    }

    void SetBlink()
    {
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, colorAlpha);
    }

    void GamePuase(bool IsPuase)
    {
        renderer.enabled = IsPuase;
        playerBody.simulated = !IsPuase;
    }

    public void SetDirection(float Dir)
    {
        direction = Dir;
    }

    public void SetPumpJumpTime()
    {
        pumpJump = 0.1f;
    }

    public bool IsStarMode()
    {
        return isStarMode;
    }

    void CheckInviTimeCollision()
    {
        Vector3 pos = transform.position + (Vector3.down * (transform.GetComponent<BoxCollider2D>().size.y / 2));

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 0.1f);

        if (false == hit)
            return;

        if ("Monster" == hit.transform.tag)
        {
            Monster monster = hit.transform.GetComponent<Monster>();
            monster.SetPumpDead(true);

            playerBody.velocity = new Vector2(playerBody.velocity.x, playerBody.velocity.y + 0.4f);
            SetPumpJumpTime();
        }
    }

    void SetMoveAnimation()
    {
        if (0f != direction)
            animState[(int)state].SendMessage("SetWalkAnimation");
        else
        {
            if (shootTime <= 0)
            {
                if (true == isJump)
                    animState[(int)state].SendMessage("SetJumpAnimation");
                else
                {
                    if (true == isDown)
                        animState[(int)state].SendMessage("SetDownAnimation");
                    else
                        animState[(int)state].SendMessage("SetStandAnimation");
                }
            }
        }
    }

    public bool IsGround()
    {
        return isGround;
    }

    private void OnBecameInvisible()
    {
        if (true == isDead)
        {
            playerBody.simulated = false;
            shootCoolTIme = 0f;
            GameManager.instance.SetGameState(GameManager.GameState.GAME_DIE);
        }
    }

    void SoundPlay(int Index, bool Repeat = false)
    {
        if (false == Repeat && 0 != Index)
        {
            if (true == audio.isPlaying && audioIndex == Index)
                return;
        }

        audio.time = 0f;
        audioIndex = Index;

        audio.clip = soundList[audioIndex];
        audio.Play();
    }

    public void SetPlayerAnimation(string Animation)
    {
        if ("SetStandAnimation" == Animation)
            isGround = true;

        animState[(int)state].SendMessage(Animation);
    }
}