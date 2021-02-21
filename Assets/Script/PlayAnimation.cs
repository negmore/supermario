using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
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
    private float speed = 8f;
    private float deash = 1f;

    private int index = 0;

    private bool isWalk = false;
    public bool isClear = false;

    private void Awake()
    {
        curAnim = stand;
        renderer.sprite = curAnim[index];
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
        {
            starTime -= Time.deltaTime;

            if (starTime < 2f)
                curTime *= 0.5f;

            if (starTime < 0f)
                starTime = 0f;
        }

        if (true == isWalk)
        {
            if (0f < starTime)
            {
                if (1f < curTime)
                    curAnim = starWalk3;
                else if (0.5f < curTime)
                    curAnim = starWalk2;
                else if (0f <= curTime)
                    curAnim = starWalk1;
            }
            else
            {
                curAnim = walk;
            }

            if (1.5f < curTime)
            {
                curTime = 0f;
                ++index;
            }

            if (curAnim.Length <= index)
                index = 0;
        }
        else
        {
            if (1f < curTime)
            {
                curTime = 0f;

                if (0f < starTime)
                {
                    index += (0 == (index + 1) % 4) ? 2 : 1;
                }
                else
                {
                    if (curAnim.Length < 4)
                        ++index;
                    else
                        index += 4;
                }
            }

            if (true == isClear)
                index = 0;

            if (curAnim.Length <= index)
                index = (0f == starTime) ? 0 : index - curAnim.Length;
        }

        renderer.sprite = curAnim[index];
    }

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
        curAnim = stand;

        InitAnimationValue();
    }

    void SetWalkAnimation()
    {
        if (true == isWalk)
            return;

        InitAnimationValue();

        curAnim = walk;
        isWalk = true;
    }

    void SetJumpAnimation()
    {
        curAnim = jump;

        InitAnimationValue();

    }

    void SetDownAnimation()
    {
        if (0 == down.Length)
            return;

        curAnim = down;

        InitAnimationValue();
    }

    void SetDeadAnimation()
    {
        InitAnimationValue();
        curAnim = die;
    }
    void SetFlipAnimation()
    {
        curAnim = flip;

        InitAnimationValue();
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

        curTime = 0f;
        ++index;
    }

    void InitAnimationValue()
    {
        curTime = 0f;
        index = (0f == starTime) ? 0 : 1;

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
    }

    public int GetAnimationIndex()
    {
        return index;
    }

    public string GetAnimationState()
    {
        string state = "";

        if (curAnim == stand)
            state =  "stand";
        else if (curAnim == walk)
            state = "walk";
        else if (curAnim == jump)
            state = "jump";
        else if (curAnim == flip)
            state = "flip";
        else if (curAnim == down)
            state = "down";

        return state;
    }

    public void SetChangedState(string State, int Index)
    {
        if (State == "stand")
            renderer.sprite = stand[0];
        else if (State == "walk")
            renderer.sprite = walk[index];
        else if (State == "jump")
            renderer.sprite = jump[0];
        else if (State == "flip")
            renderer.sprite = flip[0];
        else if (State == "down")
            renderer.sprite = down[0];
    }
}
