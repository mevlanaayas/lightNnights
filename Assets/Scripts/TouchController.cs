using UnityEngine;

public class TouchController : MonoBehaviour
{
    private float _height;

    private Vector3 _position;

    private float _width;

    // these must be device relative
    // for iphone se these values are true
    private const float XMultiplier = 3.50f;
    private const float YMultiplier = 6.75f;

    private void Awake()
    {
        _width = Screen.width / 2.0f;
        _height = Screen.height / 2.0f;
        _position = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void Update()
    {
        if (Input.touchCount <= 0) return; // Guard
        
        var touch = Input.GetTouch(0);
        var pos = touch.position;
        pos.x = (pos.x - _width) / _width;
        pos.y = (pos.y - _height) / _height;
        _position = new Vector3(pos.x * XMultiplier, pos.y * YMultiplier, 0.0f);
        transform.position = _position;
    }
}