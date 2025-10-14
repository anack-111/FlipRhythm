
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
    //public PlayerController Player { get; private set; }
    //public MonsterController Enemy { get; private set; }


    //public T Spawn<T>(Vector3 position, int templateID = 0, string prefabName = "") where T : BaseController
    //{
    //    System.Type type = typeof(T);

    //    if (type == typeof(PlayerController))
    //    {
    //        GameObject go = Managers.Resource.Instantiate(Managers.Data.CreatureDic[templateID].PrefabLabel);
    //        go.transform.position = position;
    //        PlayerController pc = go.GetOrAddComponent<PlayerController>();
    //        pc.SetInfo(templateID);
    //        Player = pc;

    //        return pc as T;
    //    }
    //    else if (type == typeof(MonsterController))
    //    {
    //        Data.CreatureData cd = Managers.Data.CreatureDic[templateID];
    //        GameObject go = Managers.Resource.Instantiate($"{cd.PrefabLabel}", pooling: true);
    //        MonsterController mc = go.GetOrAddComponent<MonsterController>();
    //        go.transform.position = position;
    //        mc.SetInfo(templateID);
    //        Enemy = mc;
    //        return mc as T;
    //    }

    //    return null;
    //}

    //public void Despawn<T>(T obj) where T : BaseController
    //{
    //    System.Type type = typeof(T);

    //    if (type == typeof(PlayerController))
    //    {
    //        // ?
    //    }
    //    else if (type == typeof(MonsterController))
    //    {
    //        Managers.Resource.Destroy(obj.gameObject);
    //    }

    //}


    //public void ShowDamageFont(Vector2 pos, float damage, float healAmount, Transform parent, bool isCritical = false)
    //{
    //    //string prefabName = "DamageFont";

    //    //GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
    //    //DamageFont damageText = go.GetOrAddComponent<DamageFont>();
    //    //damageText.SetInfo(pos, damage, healAmount, parent, isCritical);
    //}

    public void Clear()
    {

    }

}
