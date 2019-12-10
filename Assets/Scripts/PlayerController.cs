using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Light light;
    [SerializeField] private float movementSpeed = 3.0f;

    [SerializeField] private AudioClip collect;
    [SerializeField] private AudioClip moveLeft;
    [SerializeField] private AudioClip moveRight;
    
    private Vector3 _position;
    private bool _locationFlag = true; // if true topac on right side, on left side otherwise 
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _position = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && _locationFlag)
        {
            MoveLeft();
        }
        else if (Input.GetKey(KeyCode.S) && !_locationFlag)
        {
            MoveRight();
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            EnableCollectMode();
        }
        else
        {
            DisableCollectMode();
        }
    }

    private void MoveLeft()
    {
        _locationFlag = false;
        _position = transform.position;
        _position = new Vector3(_position.x, _position.y, -1.5f);
        
        _audioSource.clip = moveLeft;
        _audioSource.Play();
    }

    private void MoveRight()
    {
        _locationFlag = true;
        _position = transform.position;
        _position = new Vector3(_position.x, _position.y, -1.8f);
        
        _audioSource.clip = moveRight;
        _audioSource.Play();
    }

    private void EnableCollectMode()
    {
        var step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(_position.x, 0.2f, _position.z), step);
        
        _audioSource.clip = collect;
        _audioSource.Play();
    }

    private void DisableCollectMode()
    {
        var step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(_position.x, 0.5f, _position.z), step);
    }

    public void LoseLight(float value)
    {
        light.intensity -= value;
        if (light.intensity <= 0)
        {
            GameController.Instance.GameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        light.intensity += 0.2f;
    }
}