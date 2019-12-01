using System;

[Serializable]
public class PlayerData
{
    private string _username;
    private int _score;

    public PlayerData(int score)
    {
        _score = score;
    }
    
    public PlayerData(string username, int score)
    {
        _username = username;
        _score = score;
    }

    public int Score
    {
        get => _score;
        set => _score = value;
    }

    public string Username
    {
        get => _username;
        set => _username = value;
    }
}