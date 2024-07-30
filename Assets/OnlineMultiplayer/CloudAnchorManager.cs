using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
//using AzureSpatialAnchors;

namespace CloudAnchor {
    
}

public class CloudAnchorManager : MonoBehaviour
{
    public GameManager gameManager { get; set; }
    public Dictionary<int, CloudAnchor> cloudAnchors { get; set; }

    private void Start()
    {
        cloudAnchors = new Dictionary<int, CloudAnchor>();
    }

    public void InitializeOnlineGame(int maxPlayers)
    {
        // Create a new cloud anchor for each player
        for (int i = 0; i < maxPlayers; i++)
        {
            CloudAnchor cloudAnchor = CreateCloudAnchor(i);
            cloudAnchors.Add(i, cloudAnchor);
        }
    }

    public CloudAnchor CreateCloudAnchor(int playerId)
    {
        // Create a new cloud anchor
        CloudAnchor cloudAnchor = new GameObject("CloudAnchor").AddComponent<CloudAnchor>();
        cloudAnchor.anchorId = Guid.NewGuid().ToString();
        cloudAnchor.playerId = playerId;

        return cloudAnchor;
    }

    public void OnPlayerJoined(int playerId)
    {
        // Create a new cloud anchor for the player
        CloudAnchor cloudAnchor = CreateCloudAnchor(playerId);
        cloudAnchors.Add(playerId, cloudAnchor);
    }

    public void OnPlayerLeft(int playerId)
    {
        // Remove the cloud anchor for the player
        if (cloudAnchors.TryGetValue(playerId, out CloudAnchor cloudAnchor))
        {
            Destroy(cloudAnchor.gameObject);
            cloudAnchors.Remove(playerId);
        }
    }

    public void UpdateCloudAnchor(int playerId, Vector3 position, Quaternion rotation)
    {
        // Update the cloud anchor for the player
        if (cloudAnchors.TryGetValue(playerId, out CloudAnchor cloudAnchor))
        {
            cloudAnchor.transform.position = position;
            cloudAnchor.transform.rotation = rotation;
        }
    }
}