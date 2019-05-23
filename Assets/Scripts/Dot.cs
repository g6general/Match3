﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3Engine;

public class Dot : MonoBehaviour
{
    private Board mBoard;

    private float mEpsilon;
    private float mMoveVelocity;
    private float mFallVelocity;

    private GeometryPoint mTargetPos;
    private M3Settings.eState mState;

    public delegate void Handler();
    public event Handler StepCompleted;
    public event Handler FallCompleted;

    void Start()
    {   
        mBoard = FindObjectOfType<Board>();
        mEpsilon = 0.05f;
        mMoveVelocity = 0.2f;
        mFallVelocity = 0.3f;

        mState = M3Settings.eState.WAITING;
        mTargetPos = new GeometryPoint();
    }

    void Update()
    {
        if (!mTargetPos.IsExist() || M3Settings.eState.WAITING == mState)
            return;

        if (M3Settings.eState.MOVE == mState)
        {
            Vector2 targetPos = new Vector2(mTargetPos.X(), mTargetPos.Y());

            if (transform.position.x == mTargetPos.X()) //vertical
            {
                if (Mathf.Abs(mTargetPos.Y() - transform.position.y) > mEpsilon)
                {
                    //move towards the target
                    transform.position = Vector2.Lerp(transform.position, targetPos, mMoveVelocity);
                }
                else
                {
                    //directly set position
                    transform.position = targetPos;
                    mBoard.mFigures[mTargetPos.I(), mTargetPos.J()] = gameObject;

                    ResetTargetPosition();
                    mState = M3Settings.eState.WAITING;

                    if (StepCompleted != null)
                        StepCompleted();
                }
            }
            else if (transform.position.y == mTargetPos.Y()) //horizontal
            {
                if (Mathf.Abs(mTargetPos.X() - transform.position.x) > mEpsilon)
                {
                    //move towards the target
                    transform.position = Vector2.Lerp(transform.position, targetPos, mMoveVelocity);
                }
                else
                {
                    //directly set position 
                    transform.position = targetPos;
                    mBoard.mFigures[mTargetPos.I(), mTargetPos.J()] = gameObject;

                    ResetTargetPosition();
                    mState = M3Settings.eState.WAITING;
                    
                    if (StepCompleted != null)
                        StepCompleted();
                }
            }
        }
        else if (M3Settings.eState.FALL == mState)
        {
            Vector2 targetPos = new Vector2(mTargetPos.X(), mTargetPos.Y());
            
            if (Mathf.Abs(mTargetPos.Y() - transform.position.y) > mEpsilon)
            {
                //move towards the target
                transform.position = Vector2.Lerp(transform.position, targetPos, mFallVelocity);
            }
            else
            {
                //directly set position
                transform.position = targetPos;
                mBoard.mFigures[mTargetPos.I(), mTargetPos.J()] = gameObject;

                ResetTargetPosition();
                mState = M3Settings.eState.WAITING;
                
                if (FallCompleted != null)
                    FallCompleted();
            }            
        }
    }

    public void SetAction(M3Settings.eState state, M3Position targetPos)
    {
        mState = state;
        mTargetPos.SetX(targetPos.X());
        mTargetPos.SetY(targetPos.Y());
    }
    
    private void SetTargetPosition(int x, int y)
    {
        mTargetPos.SetX(x);
        mTargetPos.SetX(y);
    }
    
    private void ResetTargetPosition()
    {
        SetTargetPosition(-1, -1);
    }
   
    private void OnMouseUp()
    {
        if (!mBoard.IsGameCompleted())
        {
            var currentX = (int)((transform.position.x - GeometryPoint.mOffsetX) / GeometryPoint.mScaleX); //(int) transform.position.x;
            var currentY = (int)((transform.position.y - GeometryPoint.mOffsetY) / GeometryPoint.mScaleY); //(int) transform.position.y;

            if (mBoard.IsSecondTouch())
            {
                M3Position firstPos = new M3Position(currentX, currentY);
                M3Position secondPos = new M3Position(mBoard.mFirstTouchPos.X(), mBoard.mFirstTouchPos.Y());

                mBoard.mEngine.Move(firstPos, secondPos);
                mBoard.ResetFirstTouch();
            }
            else
            {
                mBoard.mFirstTouchPos.SetX(currentX);
                mBoard.mFirstTouchPos.SetY(currentY);
            }
        }
    }
}
