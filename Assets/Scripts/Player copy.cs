// using UnityEngine;
// using Unity.Netcode;

// public class Player : NetworkBehaviour
// {
//     // Offline game variables
//     public int currentPosition { get; set; }
//     public int diceRollResult { get; set; }
//     public bool isEnemyEncounter { get; set; }

//     // Online multiplayer variables
//     public NetworkVariable<int> playerId = new NetworkVariable<int>();
//     public NetworkVariable<string> playerName = new NetworkVariable<string>();
//     public bool isOnline { get; set; }
//     public bool isHost { get; set; }

//     private GameManager gameManager;

//     private void Start()
//     {
//         // Initialize offline game variables
//         currentPosition = 0;
//         diceRollResult = 0;
//         isEnemyEncounter = false;

//         // Initialize online multiplayer variables
//         playerId.Value = -1;
//         playerName.Value = "";
//         isOnline = false;
//         isHost = false;

//         // Find the GameManager in the scene
//         gameManager = FindObjectOfType<GameManager>();
//     }

//     // Offline game methods
//     public void RollDice()
//     {
//         // Roll the dice
//         diceRollResult = Random.Range(1, 7);

//         // Move to a new position
//         currentPosition += diceRollResult;

//         // Check for enemy encounter
//         if (currentPosition >= 10)
//         {
//             isEnemyEncounter = true;
//         }

//         // Update the game state on the server
//         if (IsServer)
//         {
//             UpdateGameState();
//         }
//         else
//         {
//             CmdRollDiceServerRpc();
//         }
//     }

//     public void MoveToPosition(int newPosition)
//     {
//         // Move to a new position
//         currentPosition = newPosition;

//         // Update the game state on the server
//         if (IsServer)
//         {
//             UpdateGameState();
//         }
//         else
//         {
//             CmdMoveToPositionServerRpc(newPosition);
//         }
//     }

//     [ServerRpc]
//     public void CmdRollDiceServerRpc(ServerRpcParams rpcParams = default)
//     {
//         RollDice();
//     }

//     [ServerRpc]
//     public void CmdMoveToPositionServerRpc(int newPosition, ServerRpcParams rpcParams = default)
//     {
//         MoveToPosition(newPosition);
//     }

//     private void UpdateGameState()
//     {
//         // Update the game state on the server
//         gameManager.UpdateGameState(playerId.Value, currentPosition, diceRollResult, isEnemyEncounter);
//     }

//     public void StartOnlineMultiplayerGame()
//     {
//         if (IsServer)
//         {
//             // Initialize online game settings
//             isOnline = true;
//             isHost = true;
//             playerId.Value = (int)NetworkManager.Singleton.LocalClientId; // Cast ulong to int
//             playerName.Value = "HostPlayer"; // This should be set to the actual player's name

//             // Additional setup for the game manager
//             gameManager.InitializeOnlineGame(2);
//         }
//     }

//     public void JoinOnlineMultiplayerGame()
//     {
//         if (IsClient && !IsHost)
//         {
//             // Initialize online game settings
//             isOnline = true;
//             playerId.Value = (int)NetworkManager.Singleton.LocalClientId; // Cast ulong to int
//             playerName.Value = "ClientPlayer"; // This should be set to the actual player's name

//             // Additional setup for the game manager
//             gameManager.InitializeOnlineGame(2);
//         }
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode; // Import the Netcode namespace

public class Player : NetworkBehaviour
{
    public float moveSpeed = 2f; // Speed at which the player moves
    private Animator animator;
    private Vector3 targetPosition;
    public int currentPosition = 1; // Starting position (1 to 24)
    private Transform boardTransform;
    private AudioSource source;
    private int newPosition;

    public List<GameObject> enemies; // List of enemy GameObjects

    private void OnEnable()
    {
        Dice.OnDiceResult += MovePlayer;
    }

    private void OnDisable()
    {
        Dice.OnDiceResult -= MovePlayer;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        targetPosition = transform.localPosition; // Initialize target position in local space
        boardTransform = transform.parent; // Assume the player is a child of the board
    }

    private void MovePlayer(int diceIndex, int spacesToMove)
    {
        if (IsLocalPlayer) // Ensure this is only run on the local player
        {
            // Calculate the new position based on dice roll
            newPosition = currentPosition + spacesToMove;

            if (newPosition > 24)
            {
                newPosition = 24; // Ensure the position doesn't exceed the board limit
            }

            // Start the movement coroutine
            StartCoroutine(MoveToTarget(newPosition));
        }
    }

    private IEnumerator MoveToTarget(int newPosition)
    {
        // Set animation to walk
        animator.SetBool("isWalking", true);

        // Set audio source with walking sound to true
        source.enabled = true;

        // Move through each position sequentially
        while (currentPosition < newPosition)
        {
            // Calculate the next target position based on the current position
            targetPosition = GetPositionOnBoard(currentPosition + 1);

            // Check if the next move is a vertical move between rows
            if (ShouldMoveVertically(currentPosition + 1))
            {
                // Move vertically to the next row
                targetPosition = new Vector3(transform.localPosition.x, 0, targetPosition.z);

                // Adjust rotation to face the right direction based on row
                AdjustRotationForVerticalMove(currentPosition + 1);
            }
            else
            {
                // Adjust rotation to face the right direction based on row
                AdjustRotationForHorizontalMove(currentPosition + 1);
            }

            // Move towards the target position in local space
            while (Vector3.Distance(transform.localPosition, targetPosition) > 0.1f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);

                // Check if the player lands on an enemy position
                //CheckEnemyEncounter(currentPosition + 1);

                yield return null;
            }

            // Update the current position after reaching the target position
            currentPosition++;

            // Ensure currentPosition does not exceed newPosition
            currentPosition = Mathf.Min(currentPosition, newPosition);
        }

        // Ensure the final position is exactly the target position
        transform.localPosition = targetPosition;

        // Set animation back to idle
        animator.SetBool("isWalking", false);

        // Set audio source with walking sound to true
        source.enabled = false;

        StopAllCoroutines();

        // Check enemy encounter for the final position
        CheckEnemyEncounter(newPosition);

        // Notify the server about the move
        if (IsServer)
        {
            UpdateGameStateServerRpc(currentPosition, newPosition);
        }
        else
        {
            UpdateGameStateClientRpc(currentPosition, newPosition);
        }
    }

    private Vector3 GetPositionOnBoard(int position)
    {
        // Calculate the x and z coordinates based on the board layout in local space
        int row = (position - 1) / 6;
        int col = (position - 1) % 6;

        float x, z;

        if (row % 2 == 0)
        {
            // Even row (0, 2): moves in positive x direction
            x = col * 10f;
        }
        else
        {
            // Odd row (1, 3): moves in negative x direction
            x = (5 - col) * 10f;
        }

        z = row * 10f;

        return new Vector3(x, 0, z);
    }

    private bool ShouldMoveVertically(int position)
    {
        // Check if the next position is at the end of a row and requires moving up to the next row
        return position % 6 == 1 && position != 1;
    }

    private void AdjustRotationForVerticalMove(int position)
    {
        // Adjust rotation to face the right direction for vertical moves
        int row = (position - 1) / 6;

        if (row % 2 != 0)
        {
            // Odd row (1, 3): face in the negative x direction (left)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // Even row (0, 2): face in the positive x direction (right)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void AdjustRotationForHorizontalMove(int position)
    {
        // Adjust rotation to face the right direction for horizontal moves
        int row = (position - 1) / 6;

        if (row % 2 != 0)
        {
            // Odd row (1, 3): face in the negative x direction (left)
            transform.localRotation = Quaternion.Euler(0, 270f, 0);
        }
        else
        {
            // Even row (0, 2): face in the positive x direction (right)
            transform.localRotation = Quaternion.Euler(0, 90f, 0);
        }
    }

    private void CheckEnemyEncounter(int position)
    {
        // Check if the player lands on an enemy position (4, 10, 14, 18, 23)
        if (position == 4 || position == 10 || position == 18 || position == 23)
        {
            // Play death animation for the player
            StartCoroutine(PlayerDeath());

            if (position == 4) 
            { 
                Animator enemyAnimator = enemies[0].GetComponent<Animator>();
                enemyAnimator.SetTrigger("Attack");
            }
            else if (position == 10)
            {
                Animator enemyAnimator = enemies[1].GetComponent<Animator>();
                enemyAnimator.SetTrigger("Attack");
            }
            else if (position == 18)
            {
                Animator enemyAnimator = enemies[2].GetComponent<Animator>();
                enemyAnimator.SetTrigger("Attack");
            }
            else
            {
                Animator enemyAnimator = enemies[3].GetComponent<Animator>();
                enemyAnimator.SetTrigger("Attack");
            }

            // Teleport the player back 3 places
            newPosition = currentPosition - 3;
            currentPosition = currentPosition - 3;
            if (newPosition < 1)
            {
                newPosition = 1; // Ensure the player does not go below position 1
            }

            // Teleport back
            StartCoroutine(Teleport());
        }
        else if (position == 3 || position == 14 || position == 9)
        {
            // Teleport the player forward 2 places
            newPosition = currentPosition + 2;
            currentPosition = currentPosition + 2;
            if (newPosition > 24)
            {
                newPosition = 24; // Ensure the player does not go above position 24
            }

            // Teleport forward
            StartCoroutine(Teleport());
        }
    }

    private IEnumerator Teleport()
    {
        yield return new WaitForSeconds(1);

        if (newPosition == 1) 
        { 
            gameObject.transform.localPosition = new Vector3(0, 0, 0); 
        }
        else if (newPosition == 7) 
        { 
            gameObject.transform.localPosition = new Vector3(50, 0, 10); 
        }
        else if (newPosition == 15) 
        { 
            gameObject.transform.localPosition = new Vector3(20, 0, 20); 
        }
        else if (newPosition == 20) 
        { 
            gameObject.transform.localPosition = new Vector3(40, 0, 30); 
        }
    }

    private IEnumerator PlayerDeath()
    {
        // Play death animation for the player
        animator.SetTrigger("death");

        // Wait for death animation to complete
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Additional logic after death animation (if needed)
    }

    [ServerRpc]
    private void UpdateGameStateServerRpc(int currentPosition, int newPosition, ServerRpcParams rpcParams = default)
    {
        UpdateGameState(currentPosition, newPosition);
    }

    [ClientRpc]
    private void UpdateGameStateClientRpc(int currentPosition, int newPosition, ClientRpcParams rpcParams = default)
    {
        UpdateGameState(currentPosition, newPosition);
    }

    private void UpdateGameState(int currentPosition, int newPosition)
    {
        // Update game state logic here
        // For example, you might want to synchronize the player's position with other clients
    }
}
