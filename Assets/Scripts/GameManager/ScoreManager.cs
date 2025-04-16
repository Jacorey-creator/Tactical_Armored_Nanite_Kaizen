using System.Collections.Generic;
using UnityEngine;

public class ScoreManager
{
    private static ScoreManager instance;
    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
            { 
              instance = new ScoreManager();
            }
            return instance;
        }
    }
    private Dictionary<int, PlayerScore> playerScores = new();
   
    public void RegisterPlayer(int playerID)
    {
        if (!playerScores.ContainsKey(playerID)) 
        { 
            playerScores.Add(playerID, new PlayerScore());
        }
    }
    public void AddKill(int playerID)
    {
        if (playerScores.TryGetValue(playerID, out var score))
        {
            score.kills++;
        }
    }
    public void AddDeath(int playerID)
    {
        if (playerScores.TryGetValue(playerID, out var score))
        {
            score.deaths++;
        }
    }
    public PlayerScore GetScore(int playerID)
    {
        if (playerScores.TryGetValue(playerID, out var score)) return score;
        return null;
    }
    public Dictionary<int, PlayerScore> GetAllScores()
    {
        return playerScores;
    }
    public void ResetScores()
    {
        playerScores.Clear();
    }
}

public class PlayerScore 
{
    public int kills = 0;
    public int deaths = 0;
    public int wavesDefeated = 0;
}