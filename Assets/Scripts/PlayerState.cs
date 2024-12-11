﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGGE.Patterns;

public enum PlayerStateType
{
    MOVEMENT = 0,
    ATTACK,
    RELOAD,
}

public class PlayerState : FSMState
{
    protected Player mPlayer = null;

    public PlayerState(Player player) 
        : base()
    {
        mPlayer = player;
        mFsm = mPlayer.mFsm;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}

public class PlayerState_MOVEMENT : PlayerState
{
    public PlayerState_MOVEMENT(Player player) : base(player)
    {
        mId = (int)(PlayerStateType.MOVEMENT);
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        mPlayer.Move();

        for (int i = 0; i < mPlayer.mAttackButtons.Length; ++i)
        {
            if (mPlayer.mAttackButtons[i])
            {
                if (mPlayer.mBulletsInMagazine > 0)
                {
                    PlayerState_ATTACK attack =
                  (PlayerState_ATTACK)mFsm.GetState(
                            (int)PlayerStateType.ATTACK);

                    attack.AttackID = i;
                    mPlayer.mFsm.SetCurrentState(
                        (int)PlayerStateType.ATTACK);
                }
                else
                {
                    Debug.Log("No more ammo left");
                }
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}

public class PlayerState_ATTACK : PlayerState
{
    private int mAttackID = 0;
    private string mAttackName;

    public int AttackID
    {
        get
        {
            return mAttackID;
        }
        set
        {
            mAttackID = value;
            mAttackName = "Attack" + (mAttackID + 1).ToString();
        }
    }

    public PlayerState_ATTACK(Player player) : base(player)
    {
        mId = (int)(PlayerStateType.ATTACK);
    }

    public override void Enter()
    {
        mPlayer.mAnimator.SetBool(mAttackName, true);
    }
    public override void Exit()
    {
        mPlayer.mAnimator.SetBool(mAttackName, false);
    }
    public override void Update()
    {
        base.Update();


        if (mPlayer.mBulletsInMagazine == 0 && mPlayer.mAmunitionCount > 0)
        {
            mPlayer.mFsm.SetCurrentState((int)PlayerStateType.RELOAD);
            return;
        }

        if (mPlayer.mAmunitionCount <= 0 && mPlayer.mBulletsInMagazine <= 0)
        {
            mPlayer.mFsm.SetCurrentState((int)PlayerStateType.MOVEMENT);
            mPlayer.NoAmmo();
            return;
        }

        if (mPlayer.mAttackButtons[mAttackID])
        {
            mPlayer.mAnimator.SetBool(mAttackName, true);
            mPlayer.Fire(AttackID);
        }
        else
        {
            mPlayer.mAnimator.SetBool(mAttackName, false);
            mPlayer.mFsm.SetCurrentState((int)PlayerStateType.MOVEMENT);
        }

    }
}

public class PlayerState_RELOAD : PlayerState
{
    public float ReloadTime = 3.0f;
    float dt = 0.0f;
    public int previousState;
    public PlayerState_RELOAD(Player player) : base(player)
    {
        mId = (int)(PlayerStateType.RELOAD);
    }

    public override void Enter()
    {
        mPlayer.mAnimator.SetTrigger("Reload");
        mPlayer.Reload();
        dt = 0.0f;
    }
    public override void Exit()
    {
        if (mPlayer.mAmunitionCount > mPlayer.mMaxAmunitionBeforeReload)
        {
            mPlayer.mBulletsInMagazine += mPlayer.mMaxAmunitionBeforeReload;
            mPlayer.mAmunitionCount -= mPlayer.mBulletsInMagazine;
        }
        else if (mPlayer.mAmunitionCount > 0 && mPlayer.mAmunitionCount < mPlayer.mMaxAmunitionBeforeReload)
        {
            mPlayer.mBulletsInMagazine += mPlayer.mAmunitionCount;
            mPlayer.mAmunitionCount = 0;
        }
    }

    public override void Update()
    {
        dt += Time.deltaTime;
        if (dt >= ReloadTime)
        {
            mPlayer.mFsm.SetCurrentState((int)PlayerStateType.MOVEMENT);
        }
    }

    public override void FixedUpdate()
    {
    }
}
