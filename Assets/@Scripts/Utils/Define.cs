using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public static float PLAYER_JUMPFORCE = 26.6581f;
    public static float[] SPEED_VALUE = { 8.6f, 10.4f, 12.96f, 15.6f, 19.27f };
    public static int[] SCREEN_HEIGHT_VALUES = { 11, 10, 8, 10, 10, 9 };  //11 = Cude, 10 = Ship, 8 = Ball, 10 = UFO, 10 = Wave, 9 = Spider

    [System.Serializable]
    public class MapObject
    {
        public float time;
    }
    public enum EObjectType
    {
        None,
        Player,
        Obstacle,
        Portal,
        Item,
        Ground,
        UI,
    }

    public enum ETileType
    {
        
        Block,
        Block_1x5,
        Block_1x10,
        Block_2x1,
        Block_2x2,
        Block_2x5,
        Block_2x10,
        Block_5x1,
        Spike,
        PT_Ball,
        PT_Cube,
        PT_Gravityfalse,
        PT_GravityOn,
        PT_Ship,
        PT_Spider,
        PT_UFO,
        PT_Wave,
        Block_10x2,
        Block_10x1

    }


    public enum ESpeed
    {
        Slow = 0,
        Normal,
        Fast,
        Faster,
        Fastest,
    }

    public enum EGameMode
    {
        Cube = 0,
        Ship,
        Ball,
        UFO,
        Wave,
        Spider,
    }

    public enum EGravity
    {
        Upright = 1,
        Upsidedown = -1
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

    public enum ESound
    {
        None,
        Bgm,
        SubBgm,
        Effect,
        Max,
    }
}
