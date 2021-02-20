using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float aniSpeed = 1f;
    public float speed = 1f;
    public float jumpHeight = 0f;

    protected float direction = 0f;
    protected float curTIme = 0f;
    protected float shellTime = 0f;

    protected int index = 0;

    protected bool isShell = false;
    protected bool isShellMoving = false;

    protected bool isDead = false;
    protected bool isGround = false;

    public virtual void SetDead(bool Die = true)
    {
        isDead = Die;
    }

    public virtual void SetPumpDead(bool isPump = true)
    {
        isDead = isPump;
    }

    public void SetShell(bool Shell = false)
    {
        isShell = Shell;
    }

    public void SetShellMove(bool Move = false)
    {
        if (false == isShell)
            return;

        isShellMoving = Move;
    }

    public bool GetDead()
    {
        return isDead;
    }

    public bool GetShell()
    {
        return isShell;
    }

    public bool GetShellMove()
    {
        return isShellMoving;
    }

    public void ReverseDirection()
    {
        direction *= -1f;
    }
}
