using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public class Portal : MonoBehaviour
{
    public EGameMode _gameMode;
    public ESpeed _speed;
    public bool _gravity;
    public int _state;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        try
        {
            Movement movement = collision.gameObject.GetComponent<Movement>();

            movement.ChangeThroughPortal(_gameMode, _speed, _gravity ? 1 : -1, _state, transform.position.y);
        }
        catch
        {

        }
    }
}
