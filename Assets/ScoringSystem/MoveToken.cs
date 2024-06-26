void MoveToken(int roll)
{
    // Get the current player's token
    GameObject token = players[currentPlayerIndex].GetComponent<TokenManager>().token;

    // Move the token to the new position
    token.transform.position += new Vector3(roll, 0, 0);

    // Check for snakes and ladders
    CheckForSnakesAndLadders(token);

    // Update the score
    ScoreManager scoreManager = GameObject.FindObjectOfType<ScoreManager>();
    scoreManager.UpdateScore(currentPlayerIndex, roll);
}