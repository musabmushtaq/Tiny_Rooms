using UnityEngine;


//This script is to display a crosshair on the screen 

public class CrosshairController : MonoBehaviour
{
    public GameObject crosshair; // Reference to the UI crosshair

    private void Start()
    {
        // Automatically find the crosshair if it's not assigned
        if (crosshair == null)
        {
            crosshair = GameObject.Find("crosshair"); 
            if (crosshair == null)
            {
                Debug.LogError("Crosshair object terey peyo ney add kerna??");
            }
        }
    }

    private void Update()
    {
        if (crosshair != null)
        {
            crosshair.SetActive(true); // To enable the crosshair
        }
    }
}
