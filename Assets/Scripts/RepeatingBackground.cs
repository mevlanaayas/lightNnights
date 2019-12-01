using UnityEngine;

public class RepeatingBackground : MonoBehaviour
{
    [SerializeField] public float groundHorizontalLength;
    private Transform _transform1;

    private void Start()
    {
        _transform1 = transform;
    }

    private void Update()
    {
        if (transform.position.x < -groundHorizontalLength - 5)
        {
            RepositionBackground();
        }
    }

    private void RepositionBackground()
    {
        var groundOffset = new Vector3(groundHorizontalLength * 2f, 0, 0);
        _transform1.position += groundOffset;
    }
}