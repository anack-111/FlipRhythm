using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Transform _firePos;
    public Transform FirePos
    {
        get { return _firePos; }
    }



    public void Init(Transform transform)
    {
        _firePos = transform;
    }

    public void FireBlock()
    {
        Managers.Object.Spawn<BlockController>(_firePos.transform.position, "Block");
    }
}
