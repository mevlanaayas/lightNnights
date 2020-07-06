using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Analytics;
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
    private bool _firstPointCollected = false;


    [SerializeField] private float lightLosePerSecond = 0.15f;
    [SerializeField] private float scrollSpeed = -1.5f;
    [SerializeField] private float spawnRate = 3.0f;

    // TODO: it is better to make this array
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject gameOverMenuCanvas;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text gameOverScoreText;
    
    private Dictionary<string, object> _customEventParams = new Dictionary<string, object>();
    
    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        
        _customEventParams.Add("CorrelationId", new Guid());
        
        GamePlayed();
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
        if (_timeSinceLastLightLose >= 1 && _firstPointCollected)
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

        _firstPointCollected = true;
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
        
        GameStarted();
    }

    private void Pause()
    {
        PlayButtonMusic();
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0.0f;
        _previousGameState = _gameState;
        _gameState = GameState.Paused;
        GamePaused();
    }

    public void Resume()
    {
        PlayButtonMusic();
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = _gameSpeed;
        _gameState = _previousGameState;
        
        GameResumed();
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
        
        GameLoaded();
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
        
        GameSaved();
    }

    public void Lose()
    {
        _gameState = GameState.Over;
        gameOverScoreText.text = $"Your Score {_score}";
        gameOverMenuCanvas.SetActive(true);
        gameCanvas.SetActive(false);

        GameOver();
    }

    public void Win()
    {
        _gameState = GameState.Over;
        gameOverScoreText.text = $"Never mind! You have won. Score {_score}";
        gameOverMenuCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        
        GameFinished();
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
        
        GameRestarted();
    }

    public void Quit()
    {
        GameExited();
        PlayButtonMusic();
        Application.Quit();
        _customEventParams.Clear();
    }
    
    private void GameStarted()
    {
        _customEventParams.Clear();
        _customEventParams.Add("level", 1);
        _customEventParams.Add("player", "mevlana");

        AnalyticsEvent.LevelStart("game_started", _customEventParams);
    }

    private void GamePaused()
    {
        _customEventParams.Clear();
        _customEventParams.Add("level", 1);
        _customEventParams.Add("player", "mevlana");

        AnalyticsEvent.LevelComplete("game_paused", _customEventParams);
    }
    
    private void GameResumed()
    {
        _customEventParams.Clear();
        _customEventParams.Add("level", 1);
        _customEventParams.Add("player", "mevlana");

        AnalyticsEvent.LevelComplete("game_paused", _customEventParams);
    }
    
    private void GameOver()
    {
        _customEventParams.Clear();
        _customEventParams.Add("level", 1);
        _customEventParams.Add("player", "mevlana");

        AnalyticsEvent.LevelComplete("game_over", _customEventParams);
    }
    
    private void GameFinished()
    {
        _customEventParams.Clear();
        _customEventParams.Add("level", 1);
        _customEventParams.Add("player", "mevlana");

        AnalyticsEvent.LevelComplete("game_finished", _customEventParams);
    }
    
    private void GameSaved()
    {
        _customEventParams.Clear();
        _customEventParams.Add("level", 1);
        _customEventParams.Add("player", "mevlana");

        AnalyticsEvent.LevelComplete("game_saved", _customEventParams);
    }
    
    private void GameLoaded()
    {
        _customEventParams.Clear();
        _customEventParams.Add("level", 1);
        _customEventParams.Add("player", "mevlana");

        AnalyticsEvent.LevelComplete("game_loaded", _customEventParams);
    }
    
    private void GameExited()
    {
        _customEventParams.Clear();
        _customEventParams.Add("level", 1);
        _customEventParams.Add("player", "mevlana");

        AnalyticsEvent.LevelComplete("game_exited", _customEventParams);
    }
    
    private void GamePlayed()
    {
        _customEventParams.Clear();
        _customEventParams.Add("level", 1);
        _customEventParams.Add("player", "mevlana");

        AnalyticsEvent.LevelComplete("game_played", _customEventParams);
    }
    
    private void GameRestarted()
    {
        _customEventParams.Clear();
        _customEventParams.Add("level", 1);
        _customEventParams.Add("player", "mevlana");

        AnalyticsEvent.LevelComplete("game_restarted", _customEventParams);
    }

}