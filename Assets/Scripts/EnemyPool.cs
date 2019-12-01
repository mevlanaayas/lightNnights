using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private int enemyPoolSize = 10;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector2 objectPoolPosition = new Vector2(-15f, -25f);
    [SerializeField] private float timeSinceLastSpawned;
    [SerializeField] private float spawnRate = 4f;
    [SerializeField] private float bottom = -4.5f;
    [SerializeField] private float top = 4.5f;
    [SerializeField] private float left = -8.2f;
    [SerializeField] private float right = 8.2f;
    [SerializeField] private int currentEnemy = 0;
    
    // all caching in awake
    // all configs in start
    
    private void Start()
    {
        enemies = new GameObject[enemyPoolSize];

        for (var i = 0; i < enemyPoolSize; i++)
        {
            enemies[i] = Instantiate(enemyPrefab, objectPoolPosition, Quaternion.identity);
            enemies[i].transform.parent = transform;
        }
    }

    private void Update()
    {
        timeSinceLastSpawned += Time.deltaTime;
        if (!(timeSinceLastSpawned >= spawnRate)) return; // Guard 
        timeSinceLastSpawned = 0;
        var spawnYPosition = Random.Range(bottom, top);
        var spawnXPosition = Random.Range(left, right);
        spawnXPosition = spawnXPosition > 0 ? right : left;
        enemies[currentEnemy].transform.position = new Vector2(spawnXPosition, spawnYPosition);
        currentEnemy++;
        if (currentEnemy >= enemyPoolSize)
        {
            currentEnemy = 0;
        }
    }
}
