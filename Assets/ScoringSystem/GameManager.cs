public class GameManager : MonoBehaviour
{
    public int maxPlayers = 4;
    public List<Player> players = new List<Player>();
    public DiceRoller diceRoller;
    public ScoreManager scoreManager;
    public CloudAnchorManager cloudAnchorManager;
    public TokenManager tokenManager;

    void Start()
    {
        // Initialize game state
        InitializeGameState();
    }

    void Update()
    {
        // Update game state based on player actions
        UpdateGameState();
    }

    void InitializeGameState()
    {
        // Initialize player positions, scores, etc.
        for (int i = 0; i < maxPlayers; i++)
        {
            Player player = new Player();
            player.playerId = i;
            player.playerName = "Player " + (i + 1);
            player.score = 0;
            player.tokenPosition = 0; 
            players.Add(player);
        }
    }

    void UpdateGameState()
    {
        // Update player positions, scores, etc. based on dice rolls and game logic
        foreach (Player player in players)
        {
            if (player.turn)
            {
                // Roll the dice and update the game state
                int roll = diceRoller.RollDice();
                player.score += roll;
                // Move the token
                MoveToken(player, roll);
                // Update the scoreboard
                scoreManager.UpdateScore(player.playerId, player.score);
                // Switch to the next player's turn
                player.turn = false;
                int nextPlayerIndex = (player.playerId + 1) % maxPlayers;
                players[nextPlayerIndex].turn = true;
            }
        }
    }
    void MoveToken(Player player, int roll)
    {
       // Move the token based on the dice roll
        player.tokenPosition += roll;
        tokenManager.MoveToken(player.playerId, player.tokenPosition);
    }

    public void OnPlayerJoined(int playerId, string playerName)
    {
        // Add the new player to the game
        Player player = players[playerId];
        player.playerName = playerName;
        scoreManager.SetPlayerName(playerId, playerName);
        cloudAnchorManager.OnPlayerJoined(playerId);
        scoreManager.CheckForMinimumPlayers();
    }

    public void OnPlayerLeft(int playerId)
    {
        // Remove the player from the game
        players[playerId].turn = false;
        cloudAnchorManager.OnPlayerLeft(playerId);
        scoreManager.CheckForMinimumPlayers();
    }
}