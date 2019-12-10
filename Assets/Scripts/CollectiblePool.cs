using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollectiblePool : MonoBehaviour
{
    [SerializeField] private int collectiblePoolSize = 20;
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private Vector3 objectPoolPosition = new Vector3(-15f, 0.0f, 0.0f);

    // Example point for collectible is (0.3 * t, 0.2, -1.75)
    [SerializeField] private float spawnZPositionRight = -1.80f;
    [SerializeField] private float spawnZPositionLeft = -1.50f;
    [SerializeField] private float spawnYPositionBottom = 0.2f;
    [SerializeField] private float spawnYPositionTop = 0.5f;
    [SerializeField] private float spawnXPositionMultiplier = 0.3f;

    private float _lastSpawnPosition = 0.0f;


    [SerializeField] private GameObject[] collectibles;
    [SerializeField] private float timeSinceLastSpawned;
    [SerializeField] private int currentCollectible = 0;

    private void Start()
    {
        collectibles = new GameObject[collectiblePoolSize];

        for (var i = 0; i < collectiblePoolSize; i++)
        {
            collectibles[i] = Instantiate(collectiblePrefab, objectPoolPosition, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (GameController.Instance.GetGameState() != GameState.Started) return;
        timeSinceLastSpawned += Time.deltaTime;
        if (timeSinceLastSpawned < GameController.Instance.GetSpawnRate()) return;
        timeSinceLastSpawned = 0;

        // if game started and time to spawn has come, spawn new collectible group
        // throw dice and decide how much collectible will be in the spawning group.
        var collectibleCountInGroup = Random.Range(1, 6);
        for (var i = 0; i < collectibleCountInGroup; i++)
        {
            // transform collectibles
            var leftRightDecider = Random.Range(-1, 1);
            var topBottomDecider = Random.Range(-1, 1);

            var zPosition = leftRightDecider < 0 ? spawnZPositionLeft : spawnZPositionRight;
            var yPosition = topBottomDecider < 0 ? spawnYPositionBottom : spawnYPositionTop;

            collectibles[currentCollectible].transform.position =
                new Vector3(
                    i * (spawnXPositionMultiplier + (0 - GameController.Instance.GetScrollSpeed()) * 0.2f) + _lastSpawnPosition,
                    yPosition,
                    zPosition
                );
            currentCollectible++;
            if (currentCollectible >= collectiblePoolSize)
            {
                currentCollectible = 0;
            }
        }

        try
        {
            _lastSpawnPosition += 0.3f * collectibleCountInGroup;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            GameController.Instance.Win();
        }

        /* need optimization here or it can be feature. reason is we are increasing _lastSpawnPosition infinitely
        // for now it is a feature
        if (_lastSpawnPosition >= maxXPosition)
        {
            _lastSpawnPosition = 0.0f;
        }
        */
    }
}