
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

    }
    enum Buttons
    {
        TouchButton

    }
    enum Texts
    {
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

        GetButton((int)Buttons.TouchButton).gameObject.BindEvent(OnClickTouchButton);
        return true;
    }

    private void OnClickTouchButton()
    {
        
    }

    private void Awake()
    {
        Init();
    }



}
