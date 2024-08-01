using UnityEngine;
using Unity.Netcode;

public class NetworkARObject : NetworkBehaviour
{
    // Network variable for AR object position
    public NetworkVariable<Vector3> objectPosition = new NetworkVariable<Vector3>();

    private void Start()
    {
        if (IsServer)
        {
            // Initialize the object's position
            objectPosition.Value = transform.position;
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            // Update the object's position on the server
            objectPosition.Value = transform.position;
        }
        else
        {
            // Sync the object's position with the server
            transform.position = objectPosition.Value;
        }
    }

    [ServerRpc]
    public void SetObjectPositionServerRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
        objectPosition.Value = newPosition;
    }
}
