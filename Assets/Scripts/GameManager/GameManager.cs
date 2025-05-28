using UnityEngine;

// Game States
public interface IGameState
{
    void Enter(GameManager gameManager);
    void Update(GameManager gameManager);
    void Exit(GameManager gameManager);
    void HandleInput(GameManager gameManager);
}

// Updated GameManager
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public IGameState CurrentState { get; private set; }

    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private WorldGenerationIntegrator worldGeneratorIntegrator;

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
        // Start with Main Menu state
        ChangeState(new PlayingState());
    }

    private void Update()
    {
        CurrentState?.Update(this);
        CurrentState?.HandleInput(this);
    }

    public void ChangeState(IGameState newState)
    {
        CurrentState?.Exit(this);
        CurrentState = newState;
        CurrentState?.Enter(this);

        if (worldGeneratorIntegrator != null)
        {
            worldGeneratorIntegrator.OnGameStateChanged(newState);
        }
    }

    // Public methods to change states (can be called from UI buttons, etc.)
    public void StartGame()
    {
        ChangeState(new PlayingState());
    }

    public void EndGame()
    {
        ChangeState(new GameOverState());
    }

    public void TogglePause()
    {
        if (CurrentState is PlayingState)
        {
            ChangeState(new PausedState());
        }
        else if (CurrentState is PausedState)
        {
            ChangeState(new PlayingState());
        }
    }

    public void ReturnToMainMenu()
    {
        ChangeState(new MainMenuState());
    }

    public void SpawnPlayer()
    {
        GameObject tankObj = Instantiate(tankPrefab, spawnPoint.position, spawnPoint.rotation);

        // Register Player
        RegisterPlayer(1);

        // Notify ProceduralWorldGenerator
        if (worldGeneratorIntegrator.WorldGenerator != null)
        {
            worldGeneratorIntegrator.WorldGenerator.SetPlayer(tankObj.transform);
        }
    }

    private void RegisterPlayer(int playerID)
    {
        ScoreManager.Instance.RegisterPlayer(playerID);
    }

    public PlayerScore GetPlayerScore(int playerID)
    {
        return ScoreManager.Instance.GetScore(playerID);
    }

    public bool IsInState<T>() where T : IGameState
    {
        return CurrentState is T;
    }
}