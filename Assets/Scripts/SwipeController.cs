using System;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public float multiplier = 100.0f;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Camera _camera;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // I am only interested in with the first touch
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _startPosition = _camera.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _endPosition = _camera.ScreenToWorldPoint(touch.position);

                var angle = GetDirection(_startPosition, _endPosition);
                var force = Vector2.zero;
                switch (angle)
                {
                    case SwipeDirection.Up:
                        force = Vector2.up;
                        break;
                    case SwipeDirection.Right:
                        force = Vector2.right;
                        break;
                    case SwipeDirection.Down:
                        force = Vector2.down;
                        break;

                    case SwipeDirection.Left:
                        force = Vector2.left;
                        break;
                }

                _rigidbody.AddForce(force * multiplier);
            }
        }

        Debug.DrawLine(_endPosition, _startPosition, Color.magenta);
    }

    private static SwipeDirection GetDirection(Vector3 from, Vector3 to)
    {
        var normalized = to - from;
        var angle = Math.Atan((from.y - to.y) / (from.x - to.x)) * 180 / Math.PI;
        if (normalized.x > 0 && normalized.y > 0)
        {
            // 1. zone
            if (angle > 45 && angle <= 90)
            {
                return SwipeDirection.Up;
            }

            if (angle > 0 && angle <= 45)
            {
                return SwipeDirection.Right;
            }
        }
        else if (normalized.x > 0 && normalized.y < 0)
        {
            // 2. zone
            if (angle <= 0 && angle > -45)
            {
                return SwipeDirection.Right;
            }

            if (angle <= -45 && angle > -90)
            {
                return SwipeDirection.Down;
            }
        }
        else if (normalized.x < 0 && normalized.y < 0)
        {
            // 3. zone
            if (angle > 45 && angle <= 90)
            {
                return SwipeDirection.Down;
            }

            if (angle > 0 && angle <= 45)
            {
                return SwipeDirection.Left;
            }
        }
        else if (normalized.x < 0 && normalized.y > 0)
        {
            // 4.zone
            if (angle > -45 && angle <= 0)
            {
                return SwipeDirection.Left;
            }

            if (angle <= -45 && angle > -90)
            {
                return SwipeDirection.Up;
            }
        }

        return SwipeDirection.Undefined;
    }
}