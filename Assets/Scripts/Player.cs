using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Player : MonoBehaviour
{
    // Offline game variables
    public int currentPosition { get; set; }
    public int diceRollResult { get; set; }
    public bool isEnemyEncounter { get; set; }

    // Online multiplayer variables
   public int playerId { get; set; }
    public string playerName { get; set; }
    public bool isOnline { get; set; }
    public bool isHost { get; set; }
    public PhotonView photonView { get; set; }

    private void Start()
    {
        // Initialize offline game variables
        currentPosition = 0;
        diceRollResult = 0;
        isEnemyEncounter = false;

        // Initialize online multiplayer variables
        playerId = -1;
        playerName = "";
        isOnline = false;
        isHost = false;
    }

    // Offline game methods
    public void RollDice()
    {
        // Roll the dice
        diceRollResult = Random.Range(1, 7);

        // Move to a new position
        currentPosition += diceRollResult;

        // Check for enemy encounter
        if (currentPosition >= 10)
        {
            isEnemyEncounter = true;
        }
    }

    public void MoveToPosition(int newPosition)
    {
        // Move to a new position
        currentPosition = newPosition;
    }

    // Online multiplayer methods
    public void StartOnlineMultiplayerGame()
    {
        // Create a new network identity
        networkIdentity = GetComponent<NetworkIdentity>();

        // Set up the game manager
        gameManager = GameObject.FindObjectOfType<GameManager>();
        gameManager.InitializeOnlineGame(maxPlayers);

        // Set up the cloud anchor manager
        cloudAnchorManager = GameObject.FindObjectOfType<CloudAnchorManager>();
        cloudAnchorManager.InitializeOnlineGame(maxPlayers);

        // Start the online game
        isOnline = true;
        isHost = true;
        networkIdentity.server.AddPlayer(playerId, playerName);
    }

    public void JoinOnlineMultiplayerGame(string ipAddress, int port)
    {
        // Create a new network identity
        networkIdentity = GetComponent<NetworkIdentity>();

        // Connect to the server
        networkIdentity.client.Connect(ipAddress, port);

        // Set up the game manager
        gameManager = GameObject.FindObjectOfType<GameManager>();
        gameManager.InitializeOnlineGame(maxPlayers);

        // Set up the cloud anchor manager
        cloudAnchorManager = GameObject.FindObjectOfType<CloudAnchorManager>();
        cloudAnchorManager.InitializeOnlineGame(maxPlayers);

        // Join the online game
        isOnline = true;
        networkIdentity.client.AddPlayer(playerId, playerName);
    }

    // Command to roll the dice
    [Command]
    public void CmdRollDice()
    {
        // Roll the dice
        diceRollResult = Random.Range(1, 7);

        // Move to a new position
        currentPosition += diceRollResult;

        // Check for enemy encounter
        if (currentPosition >= 10)
        {
            isEnemyEncounter = true;
        }

        // Update the game state on the server
        gameManager.UpdateGameState(playerId, currentPosition, diceRollResult, isEnemyEncounter);
    }

    // Command to move to a new position
    [Command]
    public void CmdMoveToPosition(int newPosition)
    {
        // Move to a new position
        currentPosition = newPosition;

        // Update the game state on the server
        gameManager.UpdateGameState(playerId, currentPosition, diceRollResult, isEnemyEncounter);
    }

    // Command to encounter an enemy
    [Command]
    public void CmdEncounterEnemy()
    {
        // Encounter an enemy
        isEnemyEncounter = true;

        // Update the game state on the server
        gameManager.UpdateGameState(playerId, currentPosition, diceRollResult, isEnemyEncounter);
    }
}