using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class GameScene : BaseScene
{
    private void Awake()
    {
        Init();

    }

    private void Start()
    {
       // Managers.Game.Init(GameObject.Find("FirePos").transform);
    }
    protected override void Init()
    {
        base.Init();
        SceneType = Define.EScene.GameScene;

        Managers.UI.ShowSceneUI<UI_GameScene>();    

    }


    private void Update()
    {

    }

    void EndGame()
    {

    }

    public override void Clear()
    {

    }
}
