using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Dictionary<int, Player> players { get; set; }
    public int maxPlayers { get; set; }

    private void Start()
    {
        players = new Dictionary<int, Player>();
    }

    public void InitializeOnlineGame(int maxPlayers)
    {
        this.maxPlayers = maxPlayers;
    }

    public void UpdateGameState(int playerId, int currentPosition, int diceRollResult, bool isEnemyEncounter)
    {
        // Update the game state for the player
        if (players.TryGetValue(playerId, out Player player))
        {
            player.currentPosition = currentPosition;
            player.diceRollResult = diceRollResult;
            player.isEnemyEncounter = isEnemyEncounter;

            // Update the cloud anchor for the player
            if (player.cloudAnchorManager != null)
            {
                player.cloudAnchorManager.UpdateCloudAnchor(playerId, player.transform.position, player.transform.rotation);
            }
        }
    }

    public void OnPlayerJoined(int playerId, string playerName)
    {
        // Create a new player object
        Player player = new GameObject("Player").AddComponent<Player>();
        player.playerId = playerId;
        player.playerName = playerName;
        player.gameManager = this;
        player.cloudAnchorManager = GameObject.FindObjectOfType<CloudAnchorManager>();

        // Add the player to the dictionary
        players.Add(playerId, player);

        // Create a new cloud anchor for the player
        player.cloudAnchorManager.OnPlayerJoined(playerId);
    }

    public void OnPlayerLeft(int playerId)
    {
        // Remove the player from the dictionary
        if (players.TryGetValue(playerId, out Player player))
        {
            Destroy(player.gameObject);
            players.Remove(playerId);

            // Remove the cloud anchor for the player
            player.cloudAnchorManager.OnPlayerLeft(playerId);
        }
    }
}