using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { MainMenu, Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; }

    [SerializeField] private GameObject tankPrefab;

    [SerializeField] private Transform spawnPoint;

    [SerializeField] private ProceduralWorldGenerator worldGenerator;

    public void EndGame() { }

    public void TogglePause() { }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        StartGame();
    }
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        SpawnPlayer();
    }
    public void SpawnPlayer()
    {
        GameObject tankObj = Instantiate(tankPrefab, spawnPoint.position, spawnPoint.rotation);
        
        //Register Player
        RegisterPlayer(1);

        // Notify ProceduralWorldGenerator
        if (worldGenerator != null)
        {
            worldGenerator.SetPlayer(tankObj.transform);
        }
    }
    void RegisterPlayer(int playerID)
    {
        ScoreManager.Instance.RegisterPlayer(playerID);
    }
    public PlayerScore GetPlayerScore(int playerID)
    {
        return ScoreManager.Instance.GetScore(playerID);
    }
}
