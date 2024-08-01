using UnityEngine;
using System;

public class TouchInputHandler : MonoBehaviour
{
    public ARManager arManager;

    private void Update()
    {
        // Check if there are any touches on the screen
        if (Input.touchCount > 0)
        {
            Debug.Log("Touch detected");

            // Safely access the first touch
            try
            {
                Touch touch = Input.GetTouch(0);

                // Check if the touch phase is Began (i.e., just started)
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("Touch phase began at position: " + touch.position);
                    // Pass the touch position to the ARManager
                    arManager.HandleTouchInput(touch.position);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception accessing touch input: " + ex.Message);
            }
        }
    }
}
