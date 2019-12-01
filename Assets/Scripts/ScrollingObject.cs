using System;
using UnityEngine;

public class ScrollingObject : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _velocity;
    private float _scrollSpeed;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _scrollSpeed = GameController.Instance.GetScrollSpeed();
        _velocity = new Vector3(_scrollSpeed, 0, 0);
        }

    private void Update()
    {
        // TODO: replace following lines with better solution
        if (GameController.Instance.GetGameState() == GameState.Started)
        {
            if (Math.Abs(_scrollSpeed - GameController.Instance.GetScrollSpeed()) > 0)
            {
                _velocity = new Vector3(GameController.Instance.GetScrollSpeed(), 0, 0);
            }
            _rigidbody.velocity = _velocity;
        }
        if (GameController.Instance.GetGameState() == GameState.Over)
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }
}