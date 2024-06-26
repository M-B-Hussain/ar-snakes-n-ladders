using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // The UI text components to display the scores
    public Text[] playerScores;

    // The current scores for each player
    int[] scores = new int[4];

    // The GameManager script
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    // Update the scores when a player moves
    public void UpdateScore(int playerIndex, int score)
    {
        scores[playerIndex] += score;
        UpdateScoreUI(playerIndex);
    }

    // Update the UI with the new score
    void UpdateScoreUI(int playerIndex)
    {
        playerScores[playerIndex].text = "Player " + (playerIndex + 1) + ": " + scores[playerIndex];
    }

    // Reset the scores when the game starts
    public void ResetScores()
    {
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = 0;
            UpdateScoreUI(i);
        }
    }
}