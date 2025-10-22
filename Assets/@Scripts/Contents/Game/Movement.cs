using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Movement : MonoBehaviour
{
    public EGameMode _currentGameMode;
    public ESpeed _currentSpeed;
    public float _groundCheckRadius;
    [System.NonSerialized] public float _yLastPortal = 2.3f;
    public LayerMask _groundMask;
    public Transform _sprite;
    public ParticleSystem _moveParticle;
    Rigidbody2D _rb;

    public int _gravity = 1;
    public bool _isClickProcessed = false;
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        transform.position += Vector3.right * SPEED_VALUE[(int)_currentSpeed] * Time.deltaTime;
        Invoke(_currentGameMode.ToString(), 0);
    }



    public bool OnGround()
    {
        return Physics2D.OverlapBox(transform.position + Vector3.down * _gravity * 0.5f, Vector2.right * 1.1f + Vector2.up * _groundCheckRadius, 0, _groundMask);
    }

    bool TouchingWall()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + (Vector2.right * 0.55f), Vector2.up * 0.8f + (Vector2.right * _groundCheckRadius), 0, _groundMask);
    }


    #region PlayerState
    void Cube()
    {
        Util.CreateGamemode(_rb, this, true, 19.5269f, 9.057f, true, false, 409.1f);
    }

    void Ship()
    {
        _sprite.rotation = Quaternion.Euler(0, 0, _rb.velocity.y * 2);

        if (Input.GetMouseButton(0))
            _rb.gravityScale = -4.314969f;
        else
            _rb.gravityScale = 4.314969f;

        _rb.gravityScale = _rb.gravityScale * _gravity;
    }
    void Wave()
    {
        _rb.gravityScale = 0;
        _rb.velocity = new Vector2(0, SPEED_VALUE[(int)_currentSpeed] * (Input.GetMouseButton(0) ? 1 : -1) * _gravity);
    }
    void Ball()
    {
        Util.CreateGamemode(_rb, this, true, 0, 6.2f, false, true);
    }

    void UFO()
    {
        Util.CreateGamemode(_rb, this, false, 10.841f, 4.1483f, false, false, 0, 10.841f);
    }

    void Spider()
    {
        Util.CreateGamemode(_rb, this, true, 238.29f, 6.2f, false, true, 0, 238.29f);
    }

    #endregion
    public void ChangeThroughPortal(EGameMode gameMode, ESpeed speed, int gravity, int State, float yPortal)
    {
        switch (State)
        {
            case 0:
                _currentSpeed = speed;
                break;
            case 1:
                yPortal = _yLastPortal;
                _currentGameMode = gameMode;
                UpdateEffects();

                break;
            case 2:
                _gravity = gravity;
                _rb.gravityScale = Mathf.Abs(_rb.gravityScale) * gravity;
                MoveParticleOffset(_gravity);

                break;
        }

    }


    private void UpdateEffects()
    {
        var trail = _sprite.gameObject.GetComponent<TrailRenderer>();

        if (_currentGameMode == EGameMode.Ship)
        {
            trail.enabled = true;
            _moveParticle.gameObject.SetActive(false);
        }
        else if (_currentGameMode == EGameMode.Cube)
        {
            trail.enabled = false;
            _moveParticle.gameObject.SetActive(true);
        }
        else
        {
            // 다른 모드가 생길 때 기본값
            trail.enabled = false;
            _moveParticle.gameObject.SetActive(false);
        }
    }


    void MoveParticleOffset(int gravity) //파티클 오프셋 설정
    {
        var shape = _moveParticle.shape;
        if (_gravity == 1)
            shape.position = new Vector3(shape.position.x, -0.5f, shape.position.z);
        else
            shape.position = new Vector3(shape.position.x, 0.5f, shape.position.z);
    }
}


