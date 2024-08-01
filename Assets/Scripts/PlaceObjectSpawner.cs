using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using System.Collections.Generic;

public class PlaceObjectSpawner : MonoBehaviour
{
    public ARManager arManager; // Reference to the ARManager script
    public GameObject placeObject; // The prefab to spawn

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Call ARManager's handle touch input method
                arManager.HandleTouchInput(touch.position);
            }
        }
        else if (Input.GetMouseButtonDown(0)) // For testing on desktop
        {
            // Call ARManager's handle touch input method
            arManager.HandleTouchInput(Input.mousePosition);
        }
    }
}
