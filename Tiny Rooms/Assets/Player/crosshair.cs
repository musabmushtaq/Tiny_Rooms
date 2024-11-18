using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public GameObject crosshair; // Reference to the UI crosshair

    private void Start()
    {
        // Automatically find the crosshair if it's not assigned
        if (crosshair == null)
        {
            crosshair = GameObject.Find("crosshair"); // Replace with the name of your crosshair GameObject
            if (crosshair == null)
            {
                Debug.LogError("Crosshair GameObject not found! Please assign it in the Inspector.");
            }
        }
    }

    private void Update()
    {
        if (crosshair != null)
        {
            crosshair.SetActive(true); // Example: Enable the crosshair
        }
    }
}
