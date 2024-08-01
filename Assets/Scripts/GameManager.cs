using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    // Network variables for game state
    public NetworkVariable<int> currentPlayerId = new NetworkVariable<int>();
    public NetworkVariable<int> currentPosition = new NetworkVariable<int>();
    public NetworkVariable<int> diceRollResult = new NetworkVariable<int>();
    public NetworkVariable<bool> isEnemyEncounter = new NetworkVariable<bool>();

    public void InitializeOnlineGame(int maxPlayers)
    {
        // Initialize any game state variables or configurations here
        // This method can be expanded based on your game's requirements
    }

    public void UpdateGameState(int playerId, int newPosition, int diceResult, bool enemyEncounter)
    {
        // Update the networked game state
        currentPlayerId.Value = playerId;
        currentPosition.Value = newPosition;
        diceRollResult.Value = diceResult;
        isEnemyEncounter.Value = enemyEncounter;
    }

    // Method to handle state updates on clients
    private void OnEnable()
    {
        currentPlayerId.OnValueChanged += OnGameStateChanged;
        currentPosition.OnValueChanged += OnGameStateChanged;
        diceRollResult.OnValueChanged += OnGameStateChanged;
        isEnemyEncounter.OnValueChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        currentPlayerId.OnValueChanged -= OnGameStateChanged;
        currentPosition.OnValueChanged -= OnGameStateChanged;
        diceRollResult.OnValueChanged -= OnGameStateChanged;
        isEnemyEncounter.OnValueChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(int oldValue, int newValue)
    {
        // Handle game state changes here
        // This can be expanded to update UI or other game elements
    }

    private void OnGameStateChanged(bool oldValue, bool newValue)
    {
        // Handle game state changes here
        // This can be expanded to update UI or other game elements
    }
}
