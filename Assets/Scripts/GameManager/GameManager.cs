using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { MainMenu, Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; }

    [SerializeField] private GameObject tankPrefab;

    [SerializeField] private Transform spawnPoint;

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
        //Update all AI Target
        foreach (var ai in FindObjectsByType<TankController>(FindObjectsInactive.Exclude,FindObjectsSortMode.None))
        {
            if (ai.TankData.movement_strategy == TankControllers.AI)
            {
                ai.SetAITarget(tankObj.transform);
            }
        }
        RegisterPlayer(1);
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
