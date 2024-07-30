using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private string appId = "4b380d04-b821-4f37-92d9-2b5fb20cac75";

    void Start()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdFusion = appId;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master");
    }
}