public class CloudAnchorManager : MonoBehaviour
{
    public GameManager gameManager;
    private Dictionary<int, CloudAnchor> cloudAnchors = new Dictionary<int, CloudAnchor>();

    void Start()
    {
        // Initialize cloud anchor manager
        InitializeCloudAnchors();
    }

    void InitializeCloudAnchors()
    {
        // Initialize cloud anchors for each player
        for (int i = 0; i < gameManager.maxPlayers; i++)
        {
            // Create a new cloud anchor for each player
            CreateCloudAnchor(i);
        }
    }

    void CreateCloudAnchor(int playerId)
    {
        // Create a new cloud anchor for the player
        CloudAnchor cloudAnchor = gameManager.tokenParent.GetComponentInChildren<CloudAnchor>();
        if (cloudAnchor == null)
        {
            cloudAnchor = gameManager.tokenPrefab.GetComponent<CloudAnchor>();
        }

        // Set the cloud anchor's ID and player ID
        cloudAnchor.anchorId = Guid.NewGuid().ToString();
        cloudAnchor.playerId = playerId;

        // Add the cloud anchor to the dictionary
        cloudAnchors.Add(playerId, cloudAnchor);
    }

    public void OnPlayerJoined(int playerId)
    {
        // Create a new cloud anchor for the player
        CreateCloudAnchor(playerId);
    }

    public void OnPlayerLeft(int playerId)
    {
        // Remove the cloud anchor for the player
        RemoveCloudAnchor(playerId);
    }
   void RemoveCloudAnchor(int playerId)
    {
        // Remove the cloud anchor for the player
       if (cloudAnchors.TryGetValue(playerId, out CloudAnchor cloudAnchor))
        {
         Destroy(cloudAnchor.gameObject);

            // Remove the cloud anchor from the dictionary
            cloudAnchors.Remove(playerId);
        }
    }
}