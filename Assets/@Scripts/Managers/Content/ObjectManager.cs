
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
//using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public PlayerController Player { get; private set; }


    public T Spawn<T>(Vector3 position, int templateID = 0, string prefabName = "") where T : BaseController
    {
        System.Type type = typeof(T);

        //if (type == typeof(PlayerController))
        //{
        //    GameObject go = Managers.Resource.Instantiate(Managers.Data.CreatureDic[templateID].PrefabLabel);
        //    go.transform.position = position;
        //    PlayerController pc = go.GetOrAddComponent<PlayerController>();
        //    pc.SetInfo(templateID);
        //    Player = pc;

        //    return pc as T;
        //}
        //else if (type == typeof(MonsterController))
        //{
        //    Data.CreatureData cd = Managers.Data.CreatureDic[templateID];
        //    GameObject go = Managers.Resource.Instantiate($"{cd.PrefabLabel}", pooling: true);
        //    MonsterController mc = go.GetOrAddComponent<MonsterController>();
        //    go.transform.position = position;
        //    mc.SetInfo(templateID);
        //    Enemy = mc;
        //    return mc as T;
        //}

        return null;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        System.Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            // ?
        }

    }


    public void ShowJudgeFont(Vector3 pos, string text)
    {
        string prefabName = "UI_Judge";
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);

        UI_Judge judge = go.GetOrAddComponent<UI_Judge>();
        judge.Show(text, pos);
    }

    public void Clear()
    {

    }

}
