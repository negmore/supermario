using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    enum ANIM_STATE
    {
        STATE_STAND = 0,
        STATE_WALK,
        STATE_JUMP,
        STATE_FLIP,
        STATE_DOWN,
        STATE_SHOOT,
        STATE_DIE,
    };

    private ANIM_STATE state;

    public SpriteRenderer renderer;

    public Sprite[] stand;
    public Sprite[] walk;
    public Sprite[] jump;
    public Sprite[] flip;
    public Sprite[] down;
    public Sprite[] die;
    public Sprite[] shoot;

    public Sprite[] clear;

    public Sprite[] starWalk1;
    public Sprite[] starWalk2;
    public Sprite[] starWalk3;

    private Sprite[] curAnim;

    private float curTime = 0f;
    private float starTime = 0f;
    private float StarAniTime = 0;
    private float speed = 8f;
    private float deash = 1f;

    private int index = 0;
    private int starWalkIndex = -1;

    private bool isWalk = false;
    public bool isClear = false;

    private void Awake()
    {
        curAnim = stand;
        state = ANIM_STATE.STATE_STAND;

        renderer.sprite = curAnim[index];
    }

    public void ModeInitialized()
    {
        starTime = 0f;
        starWalkIndex = -1;
        StarAniTime = 0f;
    }

    void Update()
    {
        if (false == GameManager.instance.IsGameStatePlay())
        {
            if (GameManager.GameState.GAME_CLEAR != GameManager.instance.GetGameState())
                return;
            else if (GameManager.GameState.GAME_DIE == GameManager.instance.GetGameState())
                renderer.enabled = false;
        }

        curTime += Time.deltaTime * speed * deash;

        if (0f < starTime)
            StarAnimation();
        else
            NormalAnimation();

        renderer.sprite = curAnim[index];
    }

    void NormalAnimation()
    {
        if (1f < curTime)
        {
            curTime = 0f;

            if (true == isWalk)
                ++index;
            else
                index += 4;
        }

        if (curAnim.Length <= index || true == isClear)
            index = 0;
    }

    void StarAnimation()
    {
        if (true == isClear)
        {
            StarAniTime = 0;
            index = 0;
            return;
        }

        starTime -= Time.deltaTime;
        StarAniTime += Time.deltaTime;

        if (starTime <= 0f)
        {
            if (true == isWalk)
            {
                curAnim = walk;
                state = ANIM_STATE.STATE_WALK;
            }
            else
                index = 0;

            starTime = 0f;
            starWalkIndex = -1;

            return;
        }

        float changeTime = (1.5f < starTime) ? 0.1f : 0.5f;

        if (1f < curTime)
        {
            if (true == isWalk)
            {
                curTime = 0f;
                ++index;
            }
        }

        if (changeTime < StarAniTime)
        {
            StarAniTime = 0f;

            if (true == isWalk)
            {
                ++starWalkIndex;
                ChangeStarWalkAnimation();
            }
            else
                ++index;
        }

        if (curAnim.Length <= index)
        {
            index = (true == isWalk) ? 0 : 1;
        }
    }

    void ChangeStarWalkAnimation()
    {
        if (2 < starWalkIndex)
            starWalkIndex = 0;

        switch (starWalkIndex)
        {
            case 0:
                curAnim = starWalk1;
                break;
            case 1:
                curAnim = starWalk2;
                break;
            case 2:
                curAnim = starWalk3;
                break;
        }
    }
    //void Update()
    //{
    //    if (false == GameManager.instance.IsGameStatePlay())
    //    {
    //        if (GameManager.GameState.GAME_CLEAR != GameManager.instance.GetGameState())
    //            return;
    //        else if (GameManager.GameState.GAME_DIE == GameManager.instance.GetGameState())
    //            renderer.enabled = false;
    //    }

    //    curTime += Time.deltaTime * speed * deash;

    //    if (0f < starTime)
    //    {
    //        starTime -= Time.deltaTime;

    //        if (starTime < 2f)
    //            curTime *= 0.5f;

    //        if (starTime < 0f)
    //            starTime = 0f;
    //    }

    //    if (true == isWalk)
    //    {
    //        if (0f < starTime)
    //        {
    //            if (1f < curTime)
    //                curAnim = starWalk3;
    //            else if (0.5f < curTime)
    //                curAnim = starWalk2;
    //            else if (0f <= curTime)
    //                curAnim = starWalk1;
    //        }
    //        else
    //        {
    //            curAnim = walk;
    //        }

    //        if (1.5f < curTime)
    //        {
    //            curTime = 0f;
    //            ++index;
    //        }

    //        if (curAnim.Length <= index)
    //            index = 0;
    //    }
    //    else
    //    {
    //        if (1f < curTime)
    //        {
    //            curTime = 0f;

    //            if (0f < starTime)
    //            {
    //                ++index;
    //                index = (0 == index) ? 1 : index;
    //            }
    //            else
    //            {
    //                index += 4;
    //            }
    //        }

    //        if (true == isClear)
    //            index = 0;

    //        if (curAnim.Length <= index)
    //            index = (0f == starTime) ? 0 : index - curAnim.Length;
    //    }

    //    renderer.sprite = curAnim[index];
    //}

    void ClearAnimation()
    {
        curTime += Time.deltaTime * speed * deash;

        if (1f < curTime)
        {
            curTime = 0f;
            ++index;
        }

        if (curAnim.Length <= index)
            index = 0;

        renderer.sprite = curAnim[index];
    }

    void SetStandAnimation()
    {
        if (ANIM_STATE.STATE_STAND != state)
        {
            curAnim = stand;
            state = ANIM_STATE.STATE_STAND;
            InitAnimationValue();
        }
    }

    void SetWalkAnimation()
    {
        if (true == isWalk)
            return;

        state = ANIM_STATE.STATE_WALK;

        if (starTime <= 0f)
            curAnim = walk;
        else
            ChangeStarWalkAnimation();

        InitAnimationValue();

        isWalk = true;
    }

    void SetJumpAnimation()
    {
        curAnim = jump;

        if (ANIM_STATE.STATE_JUMP != state)
        {
            state = ANIM_STATE.STATE_JUMP;

            InitAnimationValue();
        }
    }

    void SetDownAnimation()
    {
        if (ANIM_STATE.STATE_DOWN != state)
        {
            curAnim = down;
            state = ANIM_STATE.STATE_DOWN;

            InitAnimationValue();
        }
    }

    void SetDeadAnimation()
    {
        InitAnimationValue();
        curAnim = die;
        state = ANIM_STATE.STATE_DIE;

    }
    void SetFlipAnimation()
    {
        if (ANIM_STATE.STATE_FLIP != state)
        {
            curAnim = flip;
            state = ANIM_STATE.STATE_FLIP;

            InitAnimationValue();
        }
    }

    void SetClearAnimation()
    {
        InitAnimationValue();
        curAnim = clear;
    }

    void ClearStandAnimation()
    {
        if (true == isClear)
            return;

        isClear = true;
    }

    void SetStartMode(float Time)
    {
        starTime = Time;

        if (-1 ==starWalkIndex)
            starWalkIndex = 0;

        curTime = 0f;
        ++index;
    }

    void InitAnimationValue()
    {
        curTime = 0f;
        index = (starTime <= 0f) ? 0 : 1;

        isWalk = false;
        isClear = false;
    }

    void SetDashSpeed(float DashSpeed)
    {
        deash = DashSpeed * 2f;
    }

    void SetShootAnimation()
    {
        InitAnimationValue();
        curAnim = shoot;
        state = ANIM_STATE.STATE_SHOOT;
    }

    public int GetAnimationIndex()
    {
        return index;
    }

    public int GetAnimationState()
    {
        return (int)state;
    }

    public void SetChangedState(int State, int Index, int StarIndex)
    {
        starWalkIndex = StarIndex;
        int index = (-1 == StarIndex) ? 0 : starWalkIndex + 1;

        switch ((ANIM_STATE)State)
        {
            case ANIM_STATE.STATE_WALK:
                {
                    if (-1 == StarIndex)
                        renderer.sprite = walk[Index];
                    else
                    {
                        ChangeStarWalkAnimation();
                        renderer.sprite = curAnim[Index];
                    }
                }
                break;
            case ANIM_STATE.STATE_STAND:
                renderer.sprite = stand[index];
                break;
            case ANIM_STATE.STATE_JUMP:
                renderer.sprite = jump[index];
                break;
            case ANIM_STATE.STATE_FLIP:
                renderer.sprite = flip[index];
                break;
            case ANIM_STATE.STATE_DOWN:
                renderer.sprite = down[index];
                break;
            case ANIM_STATE.STATE_SHOOT:
                renderer.sprite = shoot[index];
                break;
        }
    }

    public int GetStarModeInAnimationIndex()
    {
        if (0f < starTime)
        {
            return starWalkIndex;
        }

        return -1;
    }
}
