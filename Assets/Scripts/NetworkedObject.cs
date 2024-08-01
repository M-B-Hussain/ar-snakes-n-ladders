using Unity.Netcode;
using UnityEngine;

public class NetworkedObject : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Destroy(GetComponent<ARObjectPlacer>());
        }
    }
}
