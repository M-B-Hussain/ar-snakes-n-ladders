using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using Unity.Netcode;
using System.Collections.Generic; // For List<T>

public class ARManager : MonoBehaviour
{
    private XROrigin xrOrigin; // Updated to XROrigin
    public GameObject placeObject;
    private GameObject currentBoard;

    private ARRaycastManager arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private ARSession arSession;

    private void Awake()
    {
        // Retrieve AR components from XROrigin
        arRaycastManager = xrOrigin.GetComponent<ARRaycastManager>();
        arPlaneManager = xrOrigin.GetComponent<ARPlaneManager>();
        arSession = xrOrigin.GetComponent<ARSession>();
    }

    private void Start()
    {
        // Ensure AR session is running
        if (arSession != null)
        {
            arSession.Reset();
        }

        // Optionally configure AR plane detection
        if (arPlaneManager != null)
        {
            arPlaneManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
        }
    }

     public void PlaceBoard(Vector3 position)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (currentBoard != null)
            {
                Destroy(currentBoard);
            }

            currentBoard = Instantiate(placeObject, position, Quaternion.identity);
            var networkObject = currentBoard.GetComponent<NetworkObject>();
            networkObject.Spawn();

            currentBoard.GetComponent<NetworkARObject>().SetObjectPositionServerRpc(position);
        }
    }

    public void HandleTouchInput(Vector2 touchPosition)
    {
        // Perform a raycast to detect the surface
        if (arRaycastManager != null)
        {
            var hits = new List<ARRaycastHit>(); // List<T> now correctly recognized
            if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                PlaceBoard(hitPose.position);
            }
        }
    }
    private void PlaceObjectAtPosition(Vector3 position)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Instantiate(placeObject, position, Quaternion.identity);
        }
    }
}
