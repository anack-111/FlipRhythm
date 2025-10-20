using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum ENoteType
    {
        None,
        Tap,
        Slide
    }



    public enum EObjectType
    {
        None,
        Player,
        Enemy,
        Note,
    }

    public enum EState
    {

        None,
        Idle,
        Click,
        Attack,
        Dead
    }
    public enum EScene
    {
        None,
        TitleScene,
        GameScene
    }

    public enum EUIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
        BeginDrag,
        Drag,
        EndDrag,
    }


    public enum EJudgement
    {
        None,
        Perfect,
        Great,
        Miss
    }


    public enum ENoteColor
    {
        None,
        Red,
        Cyan,
        Yellow,
    }

    public enum ENoteRule
    {
        None,
        Avoid,
        Jump,
        Free
    }
    public enum ESound
    {
        None,
        Bgm,
        SubBgm,
        Effect,
        Max,
    }
}
