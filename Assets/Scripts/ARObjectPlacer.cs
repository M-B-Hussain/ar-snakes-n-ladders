using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Netcode;
using System.Collections.Generic;

public class ARObjectPlacer : NetworkBehaviour
{
    public GameObject car1;
    private ARRaycastManager arRaycastManager;

    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();

        // Check if ARRaycastManager is attached
        if (arRaycastManager == null)
        {
            Debug.LogError("ARRaycastManager is not attached to AR Session Origin GameObject.");
        }
    }

    void Update()
    {
        // Check for touch input on mobile devices
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                PlaceObject(touch.position);
            }
        }

        // Check for mouse input for testing on laptops
        if (Input.GetMouseButtonDown(0))
        {
            PlaceObject(Input.mousePosition);
        }
    }

    void PlaceObject(Vector2 screenPosition)
    {
        // Ensure arRaycastManager is not null
        if (arRaycastManager == null)
        {
            Debug.LogError("ARRaycastManager is not set. Cannot place objects.");
            return;
        }

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            if (hits.Count > 0)
            {
                Pose hitPose = hits[0].pose;
                if (NetworkManager.Singleton.IsServer)
                {
                    GameObject placedObject = Instantiate(car1, hitPose.position, hitPose.rotation);
                    placedObject.GetComponent<NetworkObject>().Spawn();
                }
            }
        }
    }
}
