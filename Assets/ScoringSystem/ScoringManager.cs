using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public GameManager gameManager;

    private string[] playerNames;
    private int[] playerScores;

    void Start()
    {
        // Initialize player names and scores
        playerNames = new string[gameManager.maxPlayers];
        playerScores = new int[gameManager.maxPlayers];

        // Set up the score text
        scoreText.text = "";
    }

    public void UpdateScore(int playerId, int score)
    {
        // Update the player's score
        playerScores[playerId] = score;

        // Update the score text
        string scoreString = "";
        for (int i = 0; i < gameManager.players.Count; i++)
        {
            scoreString += playerNames[i] + ": " + playerScores[i] + "\n";
        }
        scoreText.text = scoreString;
    }

    public void SetPlayerName(int playerId, string playerName)
    {
        // Set the player's name
        playerNames[playerId] = playerName;
    }

    public void CheckForMinimumPlayers()
    {
        // Check if there are at least 2 players in the lobby
        if (gameManager.players.Count < 2)
        {
            // If not, disable the game
            gameManager.enabled = false;
            scoreText.text = "Not enough players. Waiting for more players to join...";
        }
        else
        {
            // If there are at least 2 players, enable the game
            gameManager.enabled = true;
        }
    }
}