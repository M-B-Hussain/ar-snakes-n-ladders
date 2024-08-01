using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;

public class NetworkManagerUI : MonoBehaviour
{
    public Button startHostButton;
    public Button joinClientButton;
    public Button offline;

    public string onlineSceneName = "OnlineMultiplayer"; // Name of your multiplayer scene

    private bool isNetworkManagerInitialized = false;

    private void Start()
    {
        // Set up button listeners
        startHostButton.onClick.AddListener(StartHost);
        joinClientButton.onClick.AddListener(JoinClient);
       
    }

    public void StartHost()
    {
        Console.WriteLine(1);

        // Initialize NetworkManager if not already initialized
        if (!isNetworkManagerInitialized)
        {
            NetworkManager.Singleton.StartHost();
            isNetworkManagerInitialized = true; // Mark as initialized
        }
    }

    public void JoinClient()
    {
        // Ensure NetworkManager is initialized and running
        if (isNetworkManagerInitialized)
        {
            Console.WriteLine("dead");
            NetworkManager.Singleton.StartClient();

        }
    }

    private void LoadOnlineScene()
    {
        // Load the multiplayer scene
        SceneManager.LoadScene(onlineSceneName);
    }
}
