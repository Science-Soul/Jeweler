using UnityEngine;

public class CameraEventMask : MonoBehaviour
{
    public LayerMask interactiveLayer; // Assign this in the Inspector

    void Start()
    {
        Camera.main.eventMask = interactiveLayer;
    }
}