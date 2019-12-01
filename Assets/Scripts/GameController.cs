using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    private GameState _gameState = GameState.Entry;
    private GameState _previousGameState = GameState.Entry;
    private float _gameSpeed;
    private int _score = 0;
    private float _timeSinceLastLightLose;
    private float _timeSinceLastSpeedLose;
    private PlayerController _playerController;
    private AudioSource _audioSource;

    
    [SerializeField] private float lightLosePerSecond = 0.1f;
    [SerializeField] private float scrollSpeed = -1.5f;
    [SerializeField] private float spawnRate = 3.0f;

    // TODO: it is better to make this array
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject gameOverMenuCanvas;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text gameOverScoreText;
    
    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _gameSpeed = Time.timeScale;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) || _gameState == GameState.Over || _gameState == GameState.Entry) return;

        if (_gameState == GameState.Paused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    private void LateUpdate()
    {
        if (_gameState != GameState.Started) return;
        _timeSinceLastLightLose += Time.deltaTime;
        _timeSinceLastSpeedLose += Time.deltaTime;
        if (_timeSinceLastLightLose >= 1)
        {
            _timeSinceLastLightLose = 0;
            _playerController.LoseLight(lightLosePerSecond);
        }

        if (_timeSinceLastSpeedLose >= 10)
        {
            _timeSinceLastSpeedLose = 0;
            /* Reduce challenge */
            scrollSpeed += 0.1f;
        }
    }

    public void Collected()
    {
        if (_gameState == GameState.Over)
        {
            return;
        }

        _score++;
        scoreText.text = $"Score: {_score.ToString()}";

        /* Increase Challenge */
        scrollSpeed -= 0.05f;
    }

    public GameState GetGameState()
    {
        return _gameState;
    }

    public float GetScrollSpeed()
    {
        return scrollSpeed;
    }

    public float GetSpawnRate()
    {
        return spawnRate;
    }

    public void StartGame()
    {
        PlayButtonMusic();
        _gameState = GameState.Started;
        mainMenuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }

    private void Pause()
    {
        PlayButtonMusic();
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0.0f;
        _previousGameState = _gameState;
        _gameState = GameState.Paused;
    }

    public void Resume()
    {
        PlayButtonMusic();
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = _gameSpeed;
        _gameState = _previousGameState;
    }


    public void LoadGame()
    {
        PlayButtonMusic();
        if (!File.Exists($"{Application.persistentDataPath}/playerInfo.dat")) return;
        var bf = new BinaryFormatter();
        var file = File.Open($"{Application.persistentDataPath}/playerInfo.dat", FileMode.Open);

        var playerData = (PlayerData) bf.Deserialize(file);
        file.Close();

        _score = playerData.Score;
        scoreText.text = $"Score: {_score.ToString()}";
    }


    public void SaveGame()
    {
        PlayButtonMusic();
        var bf = new BinaryFormatter();
        var file = File.Create($"{Application.persistentDataPath}/playerInfo.dat");
        // TODO: you can use DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss")	 to use multiple save files.
        // needs to be new load scene btw

        var playerData = new PlayerData(_score);

        bf.Serialize(file, playerData);
        file.Close();
    }

    public void GameOver()
    {
        _gameState = GameState.Over;
        gameOverScoreText.text = $"Your Score {_score}";
        gameOverMenuCanvas.SetActive(true);
        gameCanvas.SetActive(false);
    }

    public void Submit()
    {
    }


    public void HighScoreList()
    {
    }

    private void PlayButtonMusic()
    {
        _audioSource.Play();
    }

    public void RestartGame()
    {
        PlayButtonMusic();
        Time.timeScale = _gameSpeed;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        PlayButtonMusic();
        Application.Quit();
    }
}