
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class UI_GameScene : UI_Scene
{
 
    #region Enum
    enum GameObjects
    {
        GameHead
    }
    enum Buttons
    {


    }
    enum Texts
    {
        TimerText
    }

    enum Images
    {

    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        #endregion

        return true;
    }


    private void Awake()
    {
    
        Init();
    }


}
