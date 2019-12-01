using UnityEngine;

public class Collectible : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip collectibleTopLeft;
    [SerializeField] private AudioClip collectibleBottomLeft;
    [SerializeField] private AudioClip collectibleTopRight;
    [SerializeField] private AudioClip collectibleBottomRight;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameController.Instance.Collected();
        
        // check position
        // we want to play different sounds based on collectibles position 
        if (transform.position.z > -1.6f)
        {
            // this means right
            if (transform.position.y > 0.4)
            {
                // this means top
                _audioSource.clip = collectibleTopRight;
                _audioSource.Play();
            }
            else
            {
                // this means bottom
                _audioSource.clip = collectibleBottomRight;
                _audioSource.Play();
            }
        }
        else
        {
            // this means left
            if (transform.position.y > 0.4)
            {
                // this means top
                _audioSource.clip = collectibleTopLeft;
                _audioSource.Play();
            }
            else
            {
                // this means bottom
                _audioSource.clip = collectibleBottomLeft;
                _audioSource.Play();
            }
        }

        transform.position = new Vector3(-15.0f, 0.0f, 0.0f);
    }
}