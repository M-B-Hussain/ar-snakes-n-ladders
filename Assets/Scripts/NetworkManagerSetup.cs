using Unity.Netcode;
using UnityEngine;

public class NetworkManagerSetup : MonoBehaviour
{
    void Start()
    {
        NetworkManager.Singleton.StartHost();
    }
}
